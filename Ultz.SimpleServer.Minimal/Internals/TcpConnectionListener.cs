#region

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnectionListener : TcpListener, IListener
    {
        private int _id = 0;
        private readonly bool _noDelay;

        public TcpConnectionListener(IPAddress localaddr, int port, bool noDelay = true) : base(localaddr, port)
        {
            _noDelay = noDelay;
        }

        public TcpConnectionListener(IPEndPoint localEp, bool noDelay = true) : base(localEp)
        {
            _noDelay = noDelay;
        }

        public EndPoint Endpoint => LocalEndpoint;

        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        public async Task<IConnection> AcceptAsync()
        {
            var client = await AcceptTcpClientAsync();
            return new TcpConnection(client, _id++, _noDelay);
        }

        public new void Start()
        {
            base.Start();
        }

        public new void Stop()
        {
            base.Stop();
        }

        public void RunChecks()
        {
            // TODO: Check the port
        }

        public new bool Active => base.Active;

        public void Dispose()
        {
            Stop();
        }
    }
}