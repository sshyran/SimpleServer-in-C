using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals.Http
{
    public interface IHttpEngine
    {
        void ConfigureFor(HttpServer server);
        Task GotConnectionAsync(TcpConnection connection);
    }
}