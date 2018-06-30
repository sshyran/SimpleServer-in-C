using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Internals.Http2
{
    public class PriMethodResolver : IHttpMethodResolver
    {
        public HttpMethod GetMethod(string method)
        {
            return method.ToUpper() == "PRI" ? new HttpMethod(){Name = "PRI",HasInputStream = true} : null;
        }
    }
}