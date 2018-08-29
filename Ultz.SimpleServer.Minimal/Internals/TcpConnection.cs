#region

using System.IO;
using System.Net;
using System.Net.Sockets;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnection : IConnection
    {
        public TcpConnection(TcpClient client, int id, bool noDelay = true)
        {
            Base = client;
            Id = id;
            Base.NoDelay = noDelay;
        }

        public TcpClient Base { get; }

        public Stream Stream => Base.GetStream();
        public bool Connected => Base.Connected;

        public void Close()
        {
            Base.Dispose();
        }

        public EndPoint LocalEndPoint => Base.Client.LocalEndPoint;

        public EndPoint RemoteEndPoint => Base.Client.RemoteEndPoint;
        public int Id { get; }
    }
}