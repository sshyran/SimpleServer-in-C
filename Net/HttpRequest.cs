using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Net
{
    public class HttpRequest
    {
        protected string _method;
        public string Method { get { return _method; } }
        protected string _url;
        public string Url { get { return _url; } }
        protected string _version;
        public string Version { get { return _url; } }
        protected Dictionary<string, string> _args;
        public Dictionary<string, string> QueryString { get { return _args; } }
        protected Dictionary<string,string> _headers;
        public Dictionary<string, string> Headers { get { return _headers; } }
        protected int _body_size;
        protected byte[] _body;
        public HttpBody Body { get { return new HttpBody() { BodyData = _body, BodySize = _body_size }; } }
        internal HttpRequest() { }
    }
    public class HttpBody
    {
        public int BodySize { get; set; }
        public byte[] BodyData { get; set; }
    }
}
