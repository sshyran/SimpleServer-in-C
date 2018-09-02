#region

using System.IO;
using System.Net;
using System.Net.Sockets;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Wrapper class for a <see cref="TcpClient"/>
    /// </summary>
    public class TcpConnection : IConnection
    {
        /// <summary>
        /// Creates an instance with the underlying client, connection id, and optional NoDelay.
        /// </summary>
        /// <param name="client">the client to wrap</param>
        /// <param name="id">the connection id</param>
        /// <param name="noDelay">sets nodelay on the base</param>
        public TcpConnection(TcpClient client, int id, bool noDelay = true)
        {
            Base = client;
            Id = id;
            Base.NoDelay = noDelay;
        }

        private TcpClient Base { get; }

        /// <inheritdoc />
        public Stream Stream => Base.GetStream();

        /// <inheritdoc />
        public bool Connected => Base.Connected;

        /// <inheritdoc />
        public void Close()
        {
            Base.Dispose();
        }

        /// <inheritdoc />
        public EndPoint LocalEndPoint => Base.Client.LocalEndPoint;

        /// <inheritdoc />
        public EndPoint RemoteEndPoint => Base.Client.RemoteEndPoint;

        /// <inheritdoc />
        public int Id { get; }
    }
}