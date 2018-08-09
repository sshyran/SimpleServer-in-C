using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ultz.SimpleServer.Hosts;
using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Common
{
    public class SslListener : IListener
    {

        public SslListener(IListener listener,SslServerAuthenticationOptions sslServerAuthenticationOptions)
        {
            _child = listener;
        }

        private IListener _child;

        public void Dispose()
        {
        }

        public IConnection Accept()
        {
            throw new System.NotImplementedException();
        }

        public Task<IConnection> AcceptAsync()
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void RunChecks()
        {
            throw new System.NotImplementedException();
        }

        public bool Active { get; }
    }
}