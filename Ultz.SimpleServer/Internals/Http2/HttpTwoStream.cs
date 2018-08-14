#region

using System;
using System.IO;
using Ultz.SimpleServer.Internals.Http2.Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    public class HttpTwoStream : Stream
    {
        private readonly IStream _parent;

        public HttpTwoStream(IStream parent)
        {
            _parent = parent;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            // do nothing
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_parent.State == StreamState.HalfClosedRemote || _parent.State == StreamState.Closed)
                throw new ObjectDisposedException(nameof(_parent));
            return _parent.ReadAsync(new ArraySegment<byte>(buffer, offset, count)).GetAwaiter().GetResult().BytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_parent.State == StreamState.HalfClosedLocal || _parent.State == StreamState.Closed)
                throw new ObjectDisposedException(nameof(_parent));
            _parent.ReadAsync(new ArraySegment<byte>(buffer, offset, count));
        }
    }
}