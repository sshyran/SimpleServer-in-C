using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleServer.Net
{
    public class HttpParser
    {
        private static Regex RequestRegex = new Regex("^([A-Z]+) ([^ ]+) (HTTP/[^ ]+)$", RegexOptions.Compiled);
        public static HttpRequest ParseRequest()
        {
            string state = "parseReq";
            HttpRequest r = null;
            return null;
            
        }
    }
    public class HttpParserRequest : HttpRequest
    {
        internal HttpParserRequest(Dictionary<string,string> args,byte[] body,int bodySize,Dictionary<string,string> headers,string method,string url,string version)
        {
            _args = args;
            _body = body;
            _body_size = bodySize;
            _headers = headers;
            _method = method;
            _url = url;
            _version = version;
        }
    }
}
