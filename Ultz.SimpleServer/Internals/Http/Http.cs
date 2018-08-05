using System.Net;
using System.Threading.Tasks;
using Ultz.SimpleServer.Hosts;

namespace Ultz.SimpleServer.Internals.Http
{
    public abstract class Http : IProtocol
    {
        public IContext GetContext(IConnection connection)
        {
            return GetContextAsync(connection).GetAwaiter().GetResult();
        }

        public abstract Task<IContext> GetContextAsync(IConnection connection);

        public IListener CreateDefaultListener(IPEndPoint endpoint)
        {
            return new TcpConnectionListener(endpoint);
        }
    }
}