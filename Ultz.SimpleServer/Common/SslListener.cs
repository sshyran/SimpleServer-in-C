#if NETCOREAPP2_1
#region

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    public class SslListener : IListener
    {
        private readonly IListener _child;
        private readonly SslServerAuthenticationOptions _opts;

        public SslListener(IListener listener, SslServerAuthenticationOptions sslServerAuthenticationOptions)
        {
            _child = listener;
            _opts = sslServerAuthenticationOptions;
        }

        public void Dispose()
        {
            _child.Dispose();
        }

        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        public async Task<IConnection> AcceptAsync()
        {
            return new SecureConnection(await _child.AcceptAsync(), _opts);
        }

        public void Start()
        {
            _child.Start();
        }

        public void Stop()
        {
            _child.Stop();
        }

        public void RunChecks()
        {
            if (_opts == null)
                throw new NullReferenceException("No options passed to the SslListener.");
            if (_opts.ServerCertificate == null)
                throw new NullReferenceException("ServerCertificate is null. Cannot continue.");
        }

        public bool Active => _child.Active;

        internal class SecureConnection : IConnection
        {
            private readonly IConnection _child;

            public SecureConnection(IConnection child, SslServerAuthenticationOptions opts)
            {
                _child = child;
                Stream = new SslStream(child.Stream);
                ((SslStream) Stream).AuthenticateAsServerAsync(opts, CancellationToken.None).GetAwaiter().GetResult();
            }

            public Stream Stream { get; }
            public bool Connected => _child.Connected;

            public void Close()
            {
                Stream.Dispose();
            }

            public EndPoint LocalEndPoint => _child.LocalEndPoint;
            public EndPoint RemoteEndPoint => _child.RemoteEndPoint;
            public int Id => _child.Id;
        }
    }
}
#endif