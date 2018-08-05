using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpRequest : IRequest
    {
        public HttpRequest(Dictionary<string, string> headers,byte[] payload,MethodResolver<HttpMethod> methodResolver)
        {
            if (payload != null)
                InputStream = new MemoryStream(payload);
            methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"].ToUpper()));
        }
        public HttpMethod Method { get; }
        IMethod IRequest.Method => Method;
        public Stream InputStream { get; }
        public ReadOnlyDictionary<string,string> Headers { get; }
        public string Url { get; set; }
    }
}