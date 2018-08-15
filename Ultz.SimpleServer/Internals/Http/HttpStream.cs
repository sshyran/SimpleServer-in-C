#region

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals.Http2.Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpStream : IReadableByteStream
    {

        private const int MaxHeaderLength = 1024 * 1024; // 1MB
        private readonly IReadableByteStream _stream;
        private byte[] _httpBuffer = new byte[MaxHeaderLength];
        private ArraySegment<byte> _remains;
        private int _httpBufferOffset;

        private ArraySegment<byte> _completeRemains;

        public HttpStream(IReadableByteStream stream)
        {
            _stream = stream;
        }

        public int HttpHeaderLength { get; private set; }

        public ArraySegment<byte> HeaderBytes =>
            new ArraySegment<byte>(_httpBuffer, 0, HttpHeaderLength);
        
        public ArraySegment<byte> Payload => new ArraySegment<byte>(_httpBuffer, HttpHeaderLength, _httpBuffer.Length - HttpHeaderLength);

        public ValueTask<StreamReadResult> ReadAsync(ArraySegment<byte> buffer)
        {
            if (_completeRemains.Count != 0)
            {
                // Return leftover bytes from upgrade request
                var toCopy = Math.Min(_completeRemains.Count, buffer.Count);
                Array.Copy(
                    _completeRemains.Array, _completeRemains.Offset,
                    buffer.Array, buffer.Offset,
                    toCopy);
                var newOffset = _completeRemains.Offset + toCopy;
                var newCount = _completeRemains.Count - toCopy;
                if (newCount != 0)
                {
                    _completeRemains = new ArraySegment<byte>(_completeRemains.Array, newOffset, newCount);
                }
                else
                {
                    _completeRemains = new ArraySegment<byte>();
                    _httpBuffer = null;
                }

                return new ValueTask<StreamReadResult>(
                    new StreamReadResult
                    {
                        BytesRead = toCopy,
                        EndOfStream = false
                    });
            }

            return _stream.ReadAsync(buffer);
        }

        /// <summary>
        ///     Waits until a whole HTTP/1 header, terminated by \r\n\r\n was received.
        ///     This may only be called once at the beginning of the stream.
        ///     If the header was found it can be accessed with HeaderBytes.
        ///     Then it must be either consumed or marked as unread.
        /// </summary>
        public async Task WaitForHttpHeader()
        {
            while (true)
            {
                var res = await _stream.ReadAsync(
                    new ArraySegment<byte>(_httpBuffer, _httpBufferOffset, _httpBuffer.Length - _httpBufferOffset));

                if (res.EndOfStream)
                    throw new EndOfStreamException();
                _httpBufferOffset += res.BytesRead;

                // Check for end of headers in the received data
                var str = Encoding.ASCII.GetString(_httpBuffer, 0, _httpBufferOffset);
                var endOfHeaderIndex = str.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                if (endOfHeaderIndex == -1)
                {
                    // Header end not yet found
                    if (_httpBufferOffset == _httpBuffer.Length)
                    {
                        _httpBuffer = null;
                        throw new Exception("No HTTP header received");
                    }

                    // else read more bytes by looping around
                }
                else
                {
                    HttpHeaderLength = endOfHeaderIndex + 4;
                    return;
                }
            }
        }
        
        public async Task WaitForPayload(int length)
        {
            var initLength = _httpBuffer.Length;
            while (_httpBuffer.Length - initLength == length)
            {
                Console.WriteLine(_httpBuffer.Length+"-"+initLength+"=="+length +": "+(_httpBuffer.Length - initLength == length));
                var res = await _stream.ReadAsync(
                    new ArraySegment<byte>(_httpBuffer, _httpBufferOffset, _httpBuffer.Length - _httpBufferOffset));

                if (res.EndOfStream)
                    throw new EndOfStreamException();
                _httpBufferOffset += res.BytesRead;
            }
        }

        /// <summary>
        ///     Marks the HTTP reader as unread, which means following
        ///     ReadAsync calls will reread the header.
        /// </summary>
        public void UnreadHttpHeader()
        {
            _completeRemains = new ArraySegment<byte>(
                _httpBuffer, 0, _httpBufferOffset);
        }

        /// <summary>Removes the received HTTP header from the input buffer</summary>
        public void Consume()
        {
            if (HttpHeaderLength != _httpBufferOffset)
            {
                // Not everything was consumed
                _completeRemains = new ArraySegment<byte>(
                    _httpBuffer, HttpHeaderLength, _httpBufferOffset - HttpHeaderLength);
            }
            else
            {
                _completeRemains = new ArraySegment<byte>();
                _httpBuffer = null;
            }
        }
    }
}