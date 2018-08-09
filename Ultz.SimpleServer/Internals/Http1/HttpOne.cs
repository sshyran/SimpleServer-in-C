using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class HttpOne : Http.Http
    {
        public override Task<IContext> GetContextAsync(IConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}