using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpRequest : IRequest
    {
        public HttpRequest(Dictionary<string, string> headers,byte[] payload,MethodResolver<HttpMethod> methodResolver)
        {
            if (payload != null)
                InputStream = new MemoryStream(payload);
            Method = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"].ToUpper()));
            Url = new Uri(headers[":authority"]+headers[":path"]);
            RawUrl = headers[":path"];
            Headers = new ReadOnlyDictionary<string,string>(headers.Where(x => !x.Key.StartsWith(":"))
                .ToDictionary(x => x.Key, x => x.Value));
            InputStream = payload == null ? new MemoryStream() : new MemoryStream(payload);
        }
        public HttpMethod Method { get; }
        IMethod IRequest.Method => Method;
        public Stream InputStream { get; }
        public ReadOnlyDictionary<string,string> Headers { get; }
        public Uri Url { get; set; }
        public string RawUrl { get; set; }
        public static HttpParserOptional ParseFrom(string requestHeader,MethodResolver<HttpMethod> methodResolver)
        {
            var lines = requestHeader.Split(new string[]{"\r\n"}, StringSplitOptions.None);
            if (lines.Length < 1) throw new BadRequestException("Bad Request (HTTP/1 parser failure)",new ArgumentException("Request header contains less than one line",nameof(requestHeader)));

            var rparts = lines[0].Split(' ');
            var method = rparts.First();
            var path = rparts.Where(part => part != rparts[0]).Where(part => part != rparts[rparts.Length - 1]).Aggregate("", (current, part) => current + (part + " ")).TrimEnd();
            var proto = rparts.Last();
            
            if (string.IsNullOrEmpty(method) ||
                string.IsNullOrEmpty(path) ||
                string.IsNullOrEmpty(proto))
                throw new BadRequestException("Bad Request (HTTP/1 parser failure)",new NullReferenceException("The method, path, and/or protocol parameters aren't present in the request line"));

            var headers = new Dictionary<string, string>(){{":method",method},{":path",path},{":authority",""}};
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var colonIdx = line.IndexOf(':');
                if (colonIdx == -1) throw new BadRequestException("Bad Request (HTTP/1 parser failure)");
                var name = line.Substring(0, colonIdx).Trim().ToLowerInvariant();
                var value = line.Substring(colonIdx+1).Trim();
                if (name == "host")
                    name = ":authority";
                headers[name] = value;
            }

            return new HttpParserOptional(){ExpectPayload = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"])).ExpectPayload, Headers = headers, MethodResolver = methodResolver, Payload = null};
        }
    }

    public class HttpParserOptional
    {
        public HttpRequest Request => new HttpRequest(Headers,Payload,MethodResolver);
        public Dictionary<string,string> Headers { get; set; }
        public bool ExpectPayload { get; set; }
        public byte[] Payload { get; set; }
        public MethodResolver<HttpMethod> MethodResolver { get; set; }
    }
}