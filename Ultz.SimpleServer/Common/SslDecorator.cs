using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ultz.SimpleServer.Hosts;
using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Common
{
    public class SslDecorator : IDecorator
    {
        public IListener Decorate(IListener child)
        {
        }
    }
    public class SslConnection : DecoratedConnection
    {
        private SslStream _stream;
        public SslConnection(IConnection connection,X509Certificate certificate) : base(connection)
        {
            _stream = new SslStream(connection.Stream);
            _stream.AuthenticateAsServer(certificate, false, SslProtocols.Tls, true);
        }

        public override Stream Stream => _stream;
        public override void Close()
        {
            throw new System.NotImplementedException();
        }
    }
}