#region

using System.IO;
using System.Net;
using Ultz.SimpleServer.Internals.Http2.Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    /// <summary>
    ///     A <see cref="IConnection" /> implementation for HTTP/2 that ensures the user doesn't tamper with the underlying
    ///     connection without intending to.
    /// </summary>
    public class HttpTwoConnection : IConnection
    {
        private readonly IStream _stream;

        public HttpTwoConnection(IStream stream, IConnection @base)
        {
            _stream = stream;
            UnderlyingConnection = @base;
        }

        public IConnection UnderlyingConnection { get; }

        public Stream Stream => new HttpTwoStream(_stream);
        public bool Connected => _stream.State != StreamState.Closed;
        public EndPoint LocalEndPoint => UnderlyingConnection.LocalEndPoint;
        public EndPoint RemoteEndPoint => UnderlyingConnection.RemoteEndPoint;
        public int Id => UnderlyingConnection.Id;

        public void Close()
        {
            _stream.CloseAsync().GetAwaiter().GetResult();
        }
    }
}