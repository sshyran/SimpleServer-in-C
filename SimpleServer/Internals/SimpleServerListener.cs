using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerListener
    {
        private TcpListener _tcpListener;
        private SimpleServer _server;

        public SimpleServerListener(IPEndPoint localEndpoint,SimpleServer server)
        {
            LocalEndpoint = localEndpoint;

            Initialize();
        }

        public IPEndPoint LocalEndpoint { get; private set; }

        public Task<SimpleServerConnection> Accept()
        {
            return Accept_Internal();
        }

        private void Initialize()
        {
            _tcpListener = new TcpListener(LocalEndpoint);
        }

        private async Task<SimpleServerConnection> Accept_Internal()
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync();
            return new SimpleServerConnection(tcpClient,_server);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }

        public Socket Socket => _tcpListener.Server;
    }
}
