#region

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// A <see cref="System.Net.Sockets.TcpListener"/> that also listens for <see cref="IListener"/>
    /// </summary>
    public class TcpConnectionListener : TcpListener, IListener
    {
        private int _id = 0;
        private readonly bool _noDelay;

        /// <inheritdoc />
        public TcpConnectionListener(IPAddress localaddr, int port, bool noDelay = true) : base(localaddr, port)
        {
            _noDelay = noDelay;
        }

        /// <inheritdoc />
        public TcpConnectionListener(IPEndPoint localEp, bool noDelay = true) : base(localEp)
        {
            _noDelay = noDelay;
        }

        /// <inheritdoc />
        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<IConnection> AcceptAsync()
        {
            var client = await AcceptTcpClientAsync();
            return new TcpConnection(client, _id++, _noDelay);
        }

        /// <inheritdoc />
        public new void Start()
        {
            base.Start();
        }

        /// <inheritdoc />
        public new void Stop()
        {
            base.Stop();
        }

        /// <inheritdoc />
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