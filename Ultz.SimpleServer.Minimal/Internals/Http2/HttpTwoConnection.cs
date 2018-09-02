#region

using System.IO;
using System.Net;
using Http2;

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

        /// <inheritdoc />
        public HttpTwoConnection(IStream stream, IConnection @base)
        {
            _stream = stream;
            UnderlyingConnection = @base;
        }

        /// <summary>
        /// The underlying connection.
        /// </summary>
        public IConnection UnderlyingConnection { get; }

        /// <inheritdoc />
        public Stream Stream => new HttpTwoStream(_stream);

        /// <inheritdoc />
        public bool Connected => _stream.State != StreamState.Closed;

        /// <inheritdoc />
        public EndPoint LocalEndPoint => UnderlyingConnection.LocalEndPoint;

        /// <inheritdoc />
        public EndPoint RemoteEndPoint => UnderlyingConnection.RemoteEndPoint;

        /// <inheritdoc />
        public int Id => UnderlyingConnection.Id;

        /// <inheritdoc />
        public void Close()
        {
            _stream.CloseAsync().GetAwaiter().GetResult();
        }
    }
}