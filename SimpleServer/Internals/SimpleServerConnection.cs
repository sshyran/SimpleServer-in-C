using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SimpleServer.Internals
{
    public class SimpleServerConnection : IDisposable
    {
        private readonly TcpClient _client;
        internal SimpleServerListener _listener;
        internal SimpleServer _server;

        internal SimpleServerConnection(TcpClient client, SimpleServer server, SimpleServerListener listener)
        {
            _client = client;
            _listener = listener;
            _server = server;
        }

        public IPEndPoint LocalEndPoint => (IPEndPoint) _client.Client.LocalEndPoint;
        public IPEndPoint RemoteEndPoint => (IPEndPoint) _client.Client.RemoteEndPoint;
        public Stream Stream => _client.GetStream();

        public void Dispose()
        {
            _client.Dispose();
        }

        public TcpClient AsTcpClient()
        {
            return _client;
        }

        public Socket AsSocket()
        {
            return _client.Client;
        }

        public SimpleServer GetServer()
        {
            return _server;
        }

        public SimpleServerListener GetListener()
        {
            return _listener;
        }
    }
}