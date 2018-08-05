using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnection : IConnection
    {
        public TcpConnection(TcpClient client,int id)
        {
            Base = client;
            Id = id;
        }

        public Stream Stream => Base.GetStream();
        public bool Connected => Base.Connected;
        public void Close()
        {
            Base.Close();
        }

        public EndPoint LocalEndPoint => Base.Client.LocalEndPoint;

        public EndPoint RemoteEndPoint => Base.Client.RemoteEndPoint;
        public int Id { get; }

        public TcpClient Base { get; }
    }
}