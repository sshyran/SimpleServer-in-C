using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ultz.SimpleServer.Internals.Http;
using Ultz.SimpleServer.Internals.Http2;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class Http1Request : IHttpRequest
    {
        private StreamReader _reader;
        private TcpConnection _connection;
        private List<IHttpMethodResolver> _methodResolvers;

        private Http1Request(){}
        
        public Http1Request(TcpConnection connection)
        {
            _connection = connection;
            _reader = new StreamReader(_connection.Stream, Encoding.UTF8);
            _methodResolvers = new List<IHttpMethodResolver>(){new DefaultMethodResolver()};
        }

        public string Protocol { get; set; }
        public string Url { get; set; }
        public IMethod Method { get; set; }
        public string MethodName { get; set; }
        public byte[] Data { get; }
        public Dictionary<string,string> Headers { get; set; }

        public Http1Request With(params IHttpMethodResolver[] resolvers)
        {
            _methodResolvers.AddRange(resolvers);
            return this;
        }
        
        public Http1Request ParseRequestLine()
        {
            string[] parts = _reader.ReadLine()?.Split(' ');
            if (parts == null)
                throw new RequestException("Failed to read a line from the connection");
            Url = parts.Where(part => part != parts.First() && part != parts.Last()).Aggregate("", (current, part) => current + (part + " ")).Trim();
            Method = _methodResolvers.First(x => x.GetMethod(parts.First().ToUpper()) != null).GetMethod(parts.First());
            Protocol = parts.Last();
            return this;
        }

        public static Http1Request DualParse(string requestHeader)
        {
            var lines = requestHeader.Split(new string[]{"\r\n"}, StringSplitOptions.None);
            if (lines.Length < 1) throw new RequestException("Invalid HTTP header");

            // Parse request line in form GET /page HTTP/1.1
            // This is a super simple and bad parser
        
            var regex = new Regex(@"([^\s]+) ([^\s]+) ([^\s]+)");
            var match = regex.Match(lines[0]);
            if (!match.Success) throw new RequestException("Invalid HTTP header");

            var method = match.Groups[1].Value;
            var path = match.Groups[2].Value;
            var proto = match.Groups[3].Value;
            if (string.IsNullOrEmpty(method) ||
                string.IsNullOrEmpty(path) ||
                string.IsNullOrEmpty(proto))
                throw new RequestException("Invalid HTTP header");

            var headers = new Dictionary<string, string>();
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var colonIdx = line.IndexOf(':');
                if (colonIdx == -1) throw new RequestException("Invalid HTTP header");
                var name = line.Substring(0, colonIdx).Trim().ToLowerInvariant();
                var value = line.Substring(colonIdx+1).Trim();
                headers[name] = value;
            }

            return new Http1Request()
            {
                Method = new DefaultMethodResolver().GetMethod(method) ?? new PriMethodResolver().GetMethod(method),
                MethodName = method,
                Url = path,
                Protocol = proto,
                Headers = headers,
            };
        }
    }
}