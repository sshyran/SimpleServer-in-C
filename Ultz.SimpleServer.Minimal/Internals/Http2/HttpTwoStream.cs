// HttpTwoStream.cs - Ultz.SimpleServer.Minimal
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.IO;
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    /// <summary>
    ///     Provides a <see cref="Stream" /> implementation for <see cref="IStream" />
    /// </summary>
    public class HttpTwoStream : Stream
    {
        private readonly IStream _parent;

        /// <summary>
        ///     Creates a <see cref="Stream" /> from a HTTP2 <see cref="IStream" />
        /// </summary>
        /// <param name="parent">the <see cref="IStream" /> to wrap</param>
        public HttpTwoStream(IStream parent)
        {
            _parent = parent;
        }

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Flush()
        {
            // do nothing
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_parent.State == StreamState.HalfClosedRemote || _parent.State == StreamState.Closed)
                throw new ObjectDisposedException(nameof(_parent));
            return _parent.ReadAsync(new ArraySegment<byte>(buffer, offset, count)).GetAwaiter().GetResult().BytesRead;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_parent.State == StreamState.HalfClosedLocal || _parent.State == StreamState.Closed)
                throw new ObjectDisposedException(nameof(_parent));
            _parent.ReadAsync(new ArraySegment<byte>(buffer, offset, count));
        }
    }
}