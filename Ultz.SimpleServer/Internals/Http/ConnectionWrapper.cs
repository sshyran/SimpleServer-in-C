using System;
using System.IO;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals.Http2.Http2;

namespace Ultz.SimpleServer.Internals.Http
{
    public class ConnectionByteStream : IReadableByteStream, IWriteAndCloseableByteStream
    {
        private IConnection _connection;
        public MemoryStream Memory { get; }
        public bool IsUsingMemory { get; set; }
        public ConnectionByteStream(IConnection connection)
        {
            Memory = new MemoryStream();
            IsUsingMemory = false;
            _connection = connection;
        }
        
        public async ValueTask<StreamReadResult> ReadAsync(ArraySegment<byte> buffer)
        {
            var stream = IsUsingMemory ? Memory : _connection.Stream;
            var res = await stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
            await Memory.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
            return new StreamReadResult
            {
                BytesRead = res,
                EndOfStream = res == 0,
            };
        }

        public async Task WriteAsync(ArraySegment<byte> buffer)
        {
            await _connection.Stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count);
        }

        public Task CloseAsync()
        {
            _connection.Close();
            Memory.Close();
            return Task.CompletedTask;
        }
        
        
    }
}