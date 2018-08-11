using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class HttpOne : Http.Http
    {
        public override Task HandleConnectionAsync(IConnection connection,ILogger logger)
        {
            throw new System.NotImplementedException();
        }
    }
}