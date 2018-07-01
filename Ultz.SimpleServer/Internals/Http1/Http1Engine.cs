using System.Threading.Tasks;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class Http1Engine : IHttpEngine
    {
        public void ConfigureFor(HttpServer server)
        {
            throw new System.NotImplementedException();
        }

        public Task GotConnectionAsync(TcpConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}