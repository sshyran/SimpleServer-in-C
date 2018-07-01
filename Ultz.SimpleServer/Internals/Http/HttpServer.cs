using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpServer : IServer, IHttpMethodResolver
    {
        
        public IContext GetContext(IConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public Task<IContext> GetContextAsync(IConnection connection)
        {
            throw new System.NotImplementedException();
        }


        public HttpMethod GetMethod(string method)
        {
            throw new System.NotImplementedException();
        }
    }
}