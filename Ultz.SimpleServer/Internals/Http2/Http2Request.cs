using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals.Http;
using Ultz.SimpleServer.Internals.Http2.Http2;

namespace Ultz.SimpleServer.Internals.Http2
{
    public class Http2Request : IHttpRequest
    {
        private IStream _stream;
        private HttpServer _server;
        public Http2Request(IStream stream,HttpServer server)
        {
            _stream = stream;
        }

        internal async Task<Http2Request> LoadAsync()
        {
            var headers = (await _stream.ReadHeadersAsync()).ToDictionary(x => x.Name,x => x.Value);
            Headers = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                switch (header.Key.ToLower())
                {
                    case ":method":
                        MethodName = header.Value;
                        Method = _server.GetMethod(MethodName);
                        continue;
                    case ":path":
                        Url = header.Value;
                        continue;
                    case ":authority" when !Headers.ContainsKey("Host"):
                        Headers.Add("Host",header.Value);
                        continue;
                        // we do this so that handlers can use the host header regardless of the protocol
                    default:
                        Headers.Add(header.Key,header.Value);
                        break;
                }
            }
            var buf = new byte[2048];
            while (true)
            {
                var readResult = await _stream.ReadAsync(new ArraySegment<byte>(buf));
                if (readResult.EndOfStream) break;
                Data = Data.Concat(buf);
            }
        }
        
        public string Protocol { get; set; }
        public string Url { get; set; }
        public IMethod Method { get; set; }
        public string MethodName { get; set; }
        public IEnumerable<byte> Data { get; set; } = new byte[0];
        public Dictionary<string, string> Headers { get; set; }
    }
}