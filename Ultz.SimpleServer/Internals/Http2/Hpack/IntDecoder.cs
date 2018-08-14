#region

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http2.Hpack
{
    /// <summary>
    ///     Decodes integer values according to the HPACK specification.
    /// </summary>
    public class IntDecoder
    {
        /// <summary>Accumulator</summary>
        private int _acc;

        /// <summary>
        ///     Whether decoding was completed.
        ///     This is set after a call to decode().
        ///     If a complete integer could be decoded from the input buffer
        ///     the value is true. If a complete integer could not be decoded
        ///     then more bytes are needed and decodeCont must be called until
        ///     done is true before reading the result.
        /// </summary>
        public bool Done = true;

        /// <summary>The result of the decode operation</summary>
        public int Result;

        /// <summary>
        ///     Starts the decoding of an integer number from the input buffer
        ///     with the given prefix. The input Buffer MUST have at least a
        ///     single readable byte available at the given offset. If a complete
        ///     integer could be decoded during this call the result member will
        ///     be set to the result and the done member will be set to true.
        ///     Otherwise more data is needed and decodeCont must be called with
        ///     new Buffer data until done is set to true before reading the result.
        /// </summary>
        public int Decode(int prefixLen, ArraySegment<byte> buf)
        {
            var offset = buf.Offset;
            var length = buf.Count;

            var bt = buf.Array[offset];
            offset++;
            length--;
            var consumed = 1;

            var prefixMask = (1 << prefixLen) - 1;
            Result = bt & prefixMask;
            if (prefixMask == Result)
            {
                // Prefix bits are all set to 1
                _acc = 0;
                Done = false;
                consumed += DecodeCont(new ArraySegment<byte>(buf.Array, offset, length));
            }
            else
            {
                // Variable is in the prefix
                Done = true;
            }

            return consumed;
        }

        /// <summary>
        ///     Continue to decode an integer using the new input buffer data.
        /// </summary>
        public int DecodeCont(ArraySegment<byte> buf)
        {
            var offset = buf.Offset;
            var length = buf.Count;

            // Try to decode as long as we have bytes available
            while (length > 0)
            {
                var bt = buf.Array[offset];
                offset++;
                length--;

                // Calculate new result
                // Thereby check for overflows
                var add = (bt & 127) * (1u << _acc);
                var n = add + Result;
                if (n > int.MaxValue) throw new Exception("invalid integer");

                Result = (int) n;
                _acc += 7;

                if ((bt & 0x80) == 0)
                {
                    // First bit is not set - we're done
                    Done = true;
                    return offset - buf.Offset;
                }
            }

            return offset - buf.Offset;
        }
    }
}