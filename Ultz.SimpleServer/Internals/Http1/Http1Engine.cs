using System.Threading.Tasks;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class Http1Engine : IHttpEngine
    {
        public Task<HttpContext> ProvideAsync()
        {
            throw new System.NotImplementedException();
        }

        public HttpContext Provide()
        {
            throw new System.NotImplementedException();
        }
    }
}