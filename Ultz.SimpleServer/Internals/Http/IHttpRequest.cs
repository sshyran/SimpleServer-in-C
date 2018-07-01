using System.Collections.Generic;
using System.IO;

namespace Ultz.SimpleServer.Internals.Http
{
    public interface IHttpRequest : IRequest
    {
        string Protocol { get; set; }
        string Url { get; set; }
        IMethod Method { get; set; }
        string MethodName { get; set; }
        Stream Data { get; }
        Dictionary<string,string> Headers { get; set; }
    }
}