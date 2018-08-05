using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnectionListener : TcpListener, IListener
    {
        private int _id = -1;
        
        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        public async Task<IConnection> AcceptAsync()
        {
            var client = await AcceptTcpClientAsync();
            return new TcpConnection(client, _id++);
        }

        public void Start()
        {
            base.Start();
        }

        public void Stop()
        {
            base.Stop();
        }

        public void RunChecks()
        {
        }

        public bool Active => base.Active;
        public EndPoint Endpoint => base.LocalEndpoint;

        public void Dispose()
        {
            Stop();
        }

        public TcpConnectionListener(int port) : base(port)
        {
        }

        public TcpConnectionListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public TcpConnectionListener(IPEndPoint localEP) : base(localEP)
        {
        }
    }
}