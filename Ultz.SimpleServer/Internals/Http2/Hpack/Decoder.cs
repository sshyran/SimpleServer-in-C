#region

using System;
using System.Buffers;
using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Internals.Http2.Hpack
{
    /// <summary>
    ///     HPACK decoder
    /// </summary>
    public class Decoder : IDisposable
    {
        private readonly HeaderTable _headerTable;
        private readonly IntDecoder _intDecoder = new IntDecoder();

        // Tasks that should be sequentially processed
        // We need at most 3, so a fixed size array is ok
        private readonly Task[] _tasks =
        {
            new Task {Type = TaskType.None},
            new Task {Type = TaskType.None},
            new Task {Type = TaskType.None}
        };

        // Flags that determine what to do with the decoded header
        private bool _addToTable;
        private int _curTask = -1;
        private bool _sensitive;
        private StringDecoder _stringDecoder;

        /// <summary>
        ///     Controls whether decoding a table update is valid at the next Decode
        ///     call. This should be set to true at the beginning of each header block.
        /// </summary>
        public bool AllowTableSizeUpdates = true;

        /// <summary>
        ///     Whether decoding was completed.
        ///     This is set after a call to Decode().
        ///     If a complete header field could be decoded from the input buffer
        ///     the value is true.
        ///     In this case the HeaderField will be stored in the HeaderField
        ///     member and it's length in the HeaderSize member.
        /// </summary>
        public bool Done = true;

        /// <summary>The result of the decode operation</summary>
        public HeaderField HeaderField;

        /// <summary>
        ///     Returns the size of the header field according to the HPACK
        ///     decoding rules, which means size of name, value + 32
        /// </summary>
        public int HeaderSize;

        /// <summary>
        ///     Creates a new HPACK decoder with default options
        /// </summary>
        public Decoder() : this(null)
        {
        }

        /// <summary>
        ///     Creates a new HPACK decoder
        /// </summary>
        /// <param name="options">Decoder options</param>
        public Decoder(Options? options)
        {
            var dynamicTableSize = Defaults.DynamicTableSize;
            DynamicTableSizeLimit = Defaults.DynamicTableSizeLimit;
            var maxStringLength = Defaults.MaxStringLength;
            ArrayPool<byte> bufferPool = null;

            if (options.HasValue)
            {
                var opts = options.Value;
                if (opts.DynamicTableSize.HasValue) dynamicTableSize = opts.DynamicTableSize.Value;
                if (opts.DynamicTableSizeLimit.HasValue) DynamicTableSizeLimit = opts.DynamicTableSizeLimit.Value;
                if (opts.MaxStringLength.HasValue) maxStringLength = opts.MaxStringLength.Value;
                if (opts.BufferPool != null) bufferPool = opts.BufferPool;
            }

            if (bufferPool == null) bufferPool = ArrayPool<byte>.Shared;

            _stringDecoder = new StringDecoder(maxStringLength, bufferPool);

            if (dynamicTableSize > DynamicTableSizeLimit)
                throw new ArgumentException("Dynamic table size must not exceeded limit");

            _headerTable = new HeaderTable(dynamicTableSize);
        }

        /// <summary>
        ///     Returns whether the HPACK decoder is on the initial state, where it
        ///     expects the beginning of a header block fragment.
        ///     If this is not set it means that HPACK decoder is waiting on more
        ///     data in order to process header data.
        /// </summary>
        public bool HasInitialState => _curTask == -1;

        /// <summary>Returns the current maximum size of the dynamic table</summary>
        public int DynamicTableSize => _headerTable.MaxDynamicTableSize;

        /// <summary>Gets the actual used size for the dynamic table</summary>
        public int DynamicTableUsedSize => _headerTable.UsedDynamicTableSize;

        /// <summary>Gets the number of elements in the dynamic table</summary>
        public int DynamicTableLength => _headerTable.DynamicTableLength;

        /// <summary>
        ///     Returns the limit for the dynamic table size. This is the maximum
        ///     amount of bytes the HeaderTable can grow to through table size updates.
        ///     If table updates are received that are requesting a higher size then
        ///     the decoding the table size update will fail.
        /// </summary>
        public int DynamicTableSizeLimit { get; }

        public void Dispose()
        {
            _stringDecoder.Dispose();
            _stringDecoder = null;
        }

        /// <summary>
        ///     Resets the internal state for processing of the next header field
        /// </summary>
        private void Reset()
        {
            _curTask = -1;
            _addToTable = false;
            _sensitive = false;
        }

        /// <summary>
        ///     Handles the decoding of a fully indexed headerfield
        /// </summary>
        private void HandleDecodeIndexed()
        {
            // The index is stored as result of the first task
            var idx = _tasks[0].IntData;
            Reset();
            var tableHeader = _headerTable.GetAt(idx);
            // No need to check for validity here
            // The getAt function will already throw if the index is not valid

            AllowTableSizeUpdates = false;
            Done = true;
            HeaderField = new HeaderField
            {
                Name = tableHeader.Name,
                Value = tableHeader.Value,
                Sensitive = false
            };
            HeaderSize = 32 + tableHeader.NameLen + tableHeader.ValueLen;
        }

        /// <summary>
        ///     Handles the decoding of a headerfield where the name is indexed
        /// </summary>
        private void HandleDecodeNameIndexed()
        {
            var idx = _tasks[0].IntData;
            var tableHeader = _headerTable.GetAt(idx); // Can throw
            var val = _tasks[1].StringData;
            var valLen = _tasks[1].IntData;
            var sensitive = _sensitive;

            if (_addToTable) _headerTable.Insert(tableHeader.Name, tableHeader.NameLen, val, valLen);

            Reset(); // Reset decoder state

            AllowTableSizeUpdates = false;
            Done = true;
            HeaderField = new HeaderField
            {
                Name = tableHeader.Name,
                Value = val,
                Sensitive = sensitive
            };
            HeaderSize = 32 + tableHeader.NameLen + valLen;
        }

        /// <summary>
        ///     Handles the decoding of a not indexed headerfield
        /// </summary>
        private void HandleDecodeNoneIndexed()
        {
            var key = _tasks[0].StringData;
            var keyLen = _tasks[0].IntData;
            var val = _tasks[1].StringData;
            var valLen = _tasks[1].IntData;
            var sensitive = _sensitive;

            if (_addToTable) _headerTable.Insert(key, keyLen, val, valLen);

            Reset(); // Reset decoder state

            AllowTableSizeUpdates = false;
            Done = true;
            HeaderField = new HeaderField
            {
                Name = key,
                Value = val,
                Sensitive = sensitive
            };
            HeaderSize = 32 + keyLen + valLen;
        }

        private void HandleTableUpdate()
        {
            // The length is stored in the result of the first task
            var newLen = _tasks[0].IntData;
            Reset();
            // Check if new size exceeds the set limit
            if (newLen > DynamicTableSizeLimit) throw new Exception("table size limit exceeded");
            _headerTable.MaxDynamicTableSize = newLen;
        }

        /// <summary>
        ///     Processes a chunk of HPACK bytes.
        ///     This method can throw exceptions on decoding errors.
        /// </summary>
        /// <returns>The number of processed bytes</returns>
        public int Decode(ArraySegment<byte> input)
        {
            if (input.Array == null) throw new ArgumentException(nameof(input));
            var offset = input.Offset;
            var count = input.Count;

            // Loop as long as we have data available
            for (;;)
            {
                var segment = new ArraySegment<byte>(input.Array, offset, count);
                if (_curTask == -1)
                {
                    // Start of a packet
                    Done = false; // Reset the done flag
                    if (count < 1) break;
                    // Read the first byte to determine what to do
                    // This will setup the task structures that control decoding
                    // of the data
                    var consumed = HandleStartOfPacket(segment);
                    offset += consumed;
                    count -= consumed;
                }
                else
                {
                    // There are tasks pending
                    bool executeMoreTasks;
                    var consumed = ExecutePendingTask(segment, out executeMoreTasks);
                    offset += consumed;
                    count -= consumed;
                    if (!executeMoreTasks) break;
                }
            }

            return offset - input.Offset;
        }

        /// <summary>
        ///     Reads the start of an HPACK packet and sets up further instructions
        ///     for decoding the remaining bytes
        /// </summary>
        /// <returns>The number of processed bytes</returns>
        private int HandleStartOfPacket(ArraySegment<byte> buf)
        {
            var startByte = buf.Array[buf.Offset];
            // Go the the next task in the next iteration
            _curTask = 0;
            if ((startByte & 0x80) == 0x80)
            {
                // Indexed header field representation
                // Next step is to read the index
                _tasks[0].Type = TaskType.StartReadInt;
                _tasks[0].IntData = 7;
                _tasks[1].Type = TaskType.HandleFullyIndexed;
                return 0;
            }

            if ((startByte & 0xC0) == 0x40)
            {
                // Incremental indexing
                // The decoded header field should be added into the dynamic table
                _addToTable = true;
                if (startByte == 0x40)
                {
                    // New name
                    _tasks[0].Type = TaskType.StartReadString;
                    _tasks[1].Type = TaskType.StartReadString;
                    _tasks[2].Type = TaskType.HandleNoneIndexed;
                    return 1;
                }

                // Indexed name
                _tasks[0].Type = TaskType.StartReadInt;
                _tasks[0].IntData = 6; // 6bit prefix
                _tasks[1].Type = TaskType.StartReadString;
                _tasks[2].Type = TaskType.HandleNameIndexed;
                return 0;
            }

            if ((startByte & 0xF0) == 0x00)
            {
                // Without indexing
                // The decoded header should not be added into dynamic table,
                // but it is not sensitive
                if (startByte == 0x00)
                {
                    // New name
                    _tasks[0].Type = TaskType.StartReadString;
                    _tasks[1].Type = TaskType.StartReadString;
                    _tasks[2].Type = TaskType.HandleNoneIndexed;
                    return 1;
                }

                // Indexed name
                _tasks[0].Type = TaskType.StartReadInt;
                _tasks[0].IntData = 4; // 4bit prefix
                _tasks[1].Type = TaskType.StartReadString;
                _tasks[2].Type = TaskType.HandleNameIndexed;
                return 0;
            }

            if ((startByte & 0xF0) == 0x10)
            {
                // Never indexed
                // The decoded header should not be added into dynamic table,
                // and is sensitive
                _sensitive = true;
                if (startByte == 0x10)
                {
                    // New name
                    _tasks[0].Type = TaskType.StartReadString;
                    _tasks[1].Type = TaskType.StartReadString;
                    _tasks[2].Type = TaskType.HandleNoneIndexed;
                    return 1;
                }

                // Indexed name
                _tasks[0].Type = TaskType.StartReadInt;
                _tasks[0].IntData = 4; // 4bit prefix
                _tasks[1].Type = TaskType.StartReadString;
                _tasks[2].Type = TaskType.HandleNameIndexed;
                return 0;
            }

            if ((startByte & 0xE0) == 0x20)
            {
                // Table size update
                // This is only valid at the beginning of a header block
                if (!AllowTableSizeUpdates) throw new Exception("Table update is not allowed");
                _tasks[0].Type = TaskType.StartReadInt;
                _tasks[0].IntData = 5; // 5bit prefix
                _tasks[1].Type = TaskType.HandleTableUpdate;
                return 0;
            }

            // Can't actually happen
            throw new Exception("Invalid frame");
        }

        /// <summary>
        ///     Executes the current pending task
        /// </summary>
        /// <returns>The number of processed bytes</returns>
        private int ExecutePendingTask(ArraySegment<byte> buf, out bool executeMoreTasks)
        {
            executeMoreTasks = false;

            var offset = buf.Offset;
            var count = buf.Count;

            var currentTask = _tasks[_curTask];
            if (currentTask.Type == TaskType.StartReadInt)
            {
                // Check if we have at least 1 byte available
                if (count < 1) return 0;
                var consumed = _intDecoder.Decode(currentTask.IntData, buf);
                offset += consumed;
                count -= consumed;
                if (_intDecoder.Done)
                {
                    // Store the decoding result
                    _tasks[_curTask].IntData = _intDecoder.Result;
                    // And advance
                    _curTask++;
                    executeMoreTasks = true;
                }
                else
                {
                    // Need more bytes for reading the int
                    // Replace currentTask with a continue read task
                    _tasks[_curTask].Type = TaskType.ContReadInt;
                }
            }
            else if (currentTask.Type == TaskType.ContReadInt)
            {
                var consumed = _intDecoder.DecodeCont(buf);
                offset += consumed;
                count -= consumed;
                if (_intDecoder.Done)
                {
                    // Store the decoding result
                    _tasks[_curTask].IntData = _intDecoder.Result;
                    // And advance
                    _curTask++;
                    executeMoreTasks = true;
                }
            }
            else if (currentTask.Type == TaskType.StartReadString)
            {
                // Check if we have at least 1 byte available
                if (count < 1) return 0;
                var consumed = _stringDecoder.Decode(buf);
                offset += consumed;
                count -= consumed;
                if (_stringDecoder.Done)
                {
                    // Store the decoding result
                    _tasks[_curTask].IntData = _stringDecoder.StringLength;
                    _tasks[_curTask].StringData = _stringDecoder.Result;
                    // And advance
                    _curTask++;
                    executeMoreTasks = true;
                }
                else
                {
                    // Need more bytes for reading the string
                    // Replace currentTask with a continue read task
                    _tasks[_curTask].Type = TaskType.ContReadString;
                }
            }
            else if (currentTask.Type == TaskType.ContReadString)
            {
                var consumed = _stringDecoder.DecodeCont(buf);
                offset += consumed;
                count -= consumed;
                if (_stringDecoder.Done)
                {
                    // Store the decoding result
                    _tasks[_curTask].IntData = _stringDecoder.StringLength;
                    _tasks[_curTask].StringData = _stringDecoder.Result;
                    // And advance
                    _curTask++;
                    executeMoreTasks = true;
                }
            }
            else if (currentTask.Type == TaskType.HandleNoneIndexed)
            {
                HandleDecodeNoneIndexed();
            }
            else if (currentTask.Type == TaskType.HandleNameIndexed)
            {
                HandleDecodeNameIndexed();
            }
            else if (currentTask.Type == TaskType.HandleFullyIndexed)
            {
                HandleDecodeIndexed();
            }
            else if (currentTask.Type == TaskType.HandleTableUpdate)
            {
                HandleTableUpdate();
                executeMoreTasks = true;
            }
            else
            {
                throw new Exception("invalid task");
            }

            return offset - buf.Offset;
        }

        /// <summary>
        ///     Options for creating an HPACK decoder
        /// </summary>
        public struct Options
        {
            /// <summary>
            ///     The limit for the size of the dynamic table.
            ///     This limit may not be exceeded by table size update frames.
            ///     Default to 4096 if not set.
            /// </summary>
            public int? DynamicTableSizeLimit;

            /// <summary>
            ///     The start size for the dynamic Table
            /// </summary>
            public int? DynamicTableSize;

            /// <summary>
            ///     The maximum length for received strings
            /// </summary>
            public int? MaxStringLength;

            /// <summary>
            ///     The buffer pool from which buffers should be rented for string
            ///     decoding.
            /// </summary>
            public ArrayPool<byte> BufferPool;
        }

        private enum TaskType
        {
            None,
            StartReadInt,
            ContReadInt,
            StartReadString,
            ContReadString,
            HandleFullyIndexed,
            HandleNameIndexed,
            HandleNoneIndexed,
            HandleTableUpdate
        }

        private struct Task
        {
            public TaskType Type;
            public int IntData;
            public string StringData;
        }
    }

    /// <summary>
    ///     Extension methods for the HPACK decoder
    /// </summary>
    public static class DecoderExtensions
    {
        /// <summary>
        ///     The status of a DecodeHeaderBlockFragment operation
        /// </summary>
        public enum DecodeStatus
        {
            Success = 0,
            MaxHeaderListSizeExceeded = 1,
            IncompleteHeaderBlockFragment = 2,
            InvalidHeaderBlockFragment = 3
        }

        /// <summary>
        ///     Decodes a whole header block fragment using the given decoder.
        /// </summary>
        /// <param name="decoder">The HPACK decoder which is used</param>
        /// <param name="buffer">The buffer which contains the header block fragment</param>
        /// <param name="maxHeaderFieldsSize">
        ///     The maximum amount of header bytes that should be decoded from this
        ///     header block fragment. If the fragment contains more bytes decoding
        ///     will be stopped and an MaxHeaderListSizeExceeded error will be
        ///     returned.
        /// </param>
        /// <param name="headers">
        ///     The list of header blocks to which the decoded headers should be added
        /// </param>
        public static DecodeFragmentResult DecodeHeaderBlockFragment(
            this Decoder decoder,
            ArraySegment<byte> buffer,
            uint maxHeaderFieldsSize,
            List<HeaderField> headers)
        {
            var offset = buffer.Offset;
            var length = buffer.Count;
            uint headersSize = 0;

            try
            {
                while (length > 0)
                {
                    var segment = new ArraySegment<byte>(buffer.Array, offset, length);
                    var consumed = decoder.Decode(segment);
                    offset += consumed;
                    length -= consumed;
                    if (decoder.Done)
                    {
                        headersSize += (uint) decoder.HeaderSize;
                        if (headersSize > maxHeaderFieldsSize)
                        {
                            // Revert the size update. We haven't added the field
                            // to the list
                            headersSize -= (uint) decoder.HeaderSize;
                            return new DecodeFragmentResult
                            {
                                Status = DecodeStatus.MaxHeaderListSizeExceeded,
                                HeaderFieldsSize = headersSize
                            };
                        }

                        headers.Add(decoder.HeaderField);
                    }
                }
            }
            catch (Exception)
            {
                // The HPACK decoder will throw various exceptions if size checks
                // fail. We convert these to an error status here.
                return new DecodeFragmentResult
                {
                    Status = DecodeStatus.InvalidHeaderBlockFragment,
                    HeaderFieldsSize = headersSize
                };
            }

            return new DecodeFragmentResult
            {
                Status = DecodeStatus.Success,
                HeaderFieldsSize = headersSize
            };
        }

        /// <summary>
        ///     The result of a DecodeHeaderBlockFragment operation
        /// </summary>
        public struct DecodeFragmentResult
        {
            /// <summary>The status of the decode operation</summary>
            public DecodeStatus Status;

            /// <summary>
            ///     The total amount of header bytes that where decoded from this
            ///     single header block fragment
            /// </summary>
            public uint HeaderFieldsSize;
        }
    }
}