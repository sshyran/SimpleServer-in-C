// ConnectionWrapper.cs - Ultz.SimpleServer.Minimal
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
using System.Threading.Tasks;
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    internal class ConnectionByteStream : IReadableByteStream, IWriteAndCloseableByteStream
    {
        public ConnectionByteStream(IConnection connection)
        {
            Memory = new MemoryStream();
            IsUsingMemory = false;
            Connection = connection;
        }

        public IConnection Connection { get; }
        public MemoryStream Memory { get; }
        public bool IsUsingMemory { get; set; }

        public async ValueTask<StreamReadResult> ReadAsync(ArraySegment<byte> buffer)
        {
            var stream = IsUsingMemory ? Memory : Connection.Stream;
            var res = await stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
            await Memory.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
            return new StreamReadResult
            {
                BytesRead = res,
                EndOfStream = res == 0
            };
        }

        public async Task WriteAsync(ArraySegment<byte> buffer)
        {
            await Connection.Stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
        }

        public Task CloseAsync()
        {
            Connection.Close();
            Memory.Dispose();
            return Task.CompletedTask;
        }
    }
}