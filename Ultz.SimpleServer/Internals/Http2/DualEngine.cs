using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals.Http2
{
    public class HttpTwo : Internals.Http.Http
    {
        public override Task<IContext> GetContextAsync(IConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}