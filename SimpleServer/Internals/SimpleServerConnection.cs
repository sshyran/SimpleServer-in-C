using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerConnection : IDisposable
    {
        TcpClient _client;
        internal SimpleServer _server;
        internal SimpleServerListener _listener;
        internal SimpleServerConnection(TcpClient client,SimpleServer server,SimpleServerListener listener)
        {
            _client = client;
            _listener = listener;
            _server = server;
        }
        public IPEndPoint LocalEndPoint => (IPEndPoint)_client.Client.LocalEndPoint;
        public IPEndPoint RemoteEndPoint => (IPEndPoint)_client.Client.RemoteEndPoint;
        public Stream Stream => _client.GetStream();
        public TcpClient AsTcpClient()
        {
            return _client;
        }
        public Socket AsSocket()
        {
            return _client.Client;
        }

        public void Dispose()
        {
            _client.Dispose();
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
