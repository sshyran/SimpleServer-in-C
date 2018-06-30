using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpServer : IServer
    {
        
        public IContext GetContext(IConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public Task<IContext> GetContextAsync(IConnection connection)
        {
            throw new System.NotImplementedException();
        }
        
        
    }
}