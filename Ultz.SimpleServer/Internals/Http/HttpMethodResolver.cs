using System.Collections.Generic;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpMethodResolver : MethodResolver<HttpMethod>
    {
        public HttpMethodResolver(Dictionary<byte[], HttpMethod> dictionary) : base(dictionary)
        {
        }
    }
}