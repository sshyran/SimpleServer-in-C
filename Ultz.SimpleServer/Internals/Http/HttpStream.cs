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
        private const int MaxHeaderLength = 1024;
        private readonly IReadableByteStream _stream;
        private byte[] _httpBuffer = new byte[MaxHeaderLength];
        private int _httpBufferOffset;

        private ArraySegment<byte> _remains;

        public HttpStream(IReadableByteStream stream)
        {
            _stream = stream;
        }

        public int HttpHeaderLength { get; private set; }

        public ArraySegment<byte> HeaderBytes =>
            new ArraySegment<byte>(_httpBuffer, 0, HttpHeaderLength);

        public ValueTask<StreamReadResult> ReadAsync(ArraySegment<byte> buffer)
        {
            if (_remains.Count != 0)
            {
                // Return leftover bytes from upgrade request
                var toCopy = Math.Min(_remains.Count, buffer.Count);
                Array.Copy(
                    _remains.Array, _remains.Offset,
                    buffer.Array, buffer.Offset,
                    toCopy);
                var newOffset = _remains.Offset + toCopy;
                var newCount = _remains.Count - toCopy;
                if (newCount != 0)
                {
                    _remains = new ArraySegment<byte>(_remains.Array, newOffset, newCount);
                }
                else
                {
                    _remains = new ArraySegment<byte>();
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

        /// <summary>
        ///     Marks the HTTP reader as unread, which means following
        ///     ReadAsync calls will reread the header.
        /// </summary>
        public void UnreadHttpHeader()
        {
            _remains = new ArraySegment<byte>(
                _httpBuffer, 0, _httpBufferOffset);
        }

        /// <summary>Removes the received HTTP header from the input buffer</summary>
        public void ConsumeHttpHeader()
        {
            if (HttpHeaderLength != _httpBufferOffset)
            {
                // Not everything was consumed
                _remains = new ArraySegment<byte>(
                    _httpBuffer, HttpHeaderLength, _httpBufferOffset - HttpHeaderLength);
            }
            else
            {
                _remains = new ArraySegment<byte>();
                _httpBuffer = null;
            }
        }

        public void Consume(int length)
        {
            if (length != _httpBufferOffset)
            {
                // Not everything was consumed
                _remains = new ArraySegment<byte>(
                    _httpBuffer, length, _httpBufferOffset - length);
            }
            else
            {
                _remains = new ArraySegment<byte>();
                _httpBuffer = null;
            }
        }
    }
}