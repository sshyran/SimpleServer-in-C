using System.Collections.Generic;
using System.IO;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpResponse : IResponse
    {
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public Dictionary<string,string> Headers { get; }
        public Stream OutputStream { get; }
        public void Close()
        {
            
        }
    }
}