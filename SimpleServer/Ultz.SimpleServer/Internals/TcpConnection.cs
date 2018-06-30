using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnection : IConnection
    {
        public TcpConnection(TcpClient client)
        {
            Base = client;
        }

        public Stream Stream => Base.GetStream();
        public bool Connected => Base.Connected;
        public void Close()
        {
            Base.Close();
        }

        public EndPoint RemoteEndPoint => Base.Client.RemoteEndPoint;
        public TcpClient Base { get; }
    }
}