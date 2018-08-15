#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals.Http2.Hpack;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpRequest : IRequest
    {
        public HttpRequest(HttpHeaderCollection headers, byte[] payload,
            MethodResolver<HttpMethod> methodResolver)
        {
            if (payload != null)
                InputStream = new MemoryStream(payload);
            Method = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"].ToUpper()));
            Url = new Uri(headers[":scheme"] + "://" + headers[":authority"] + headers[":path"]);
            RawUrl = headers[":path"];
            Headers = new HttpHeaderCollection(headers.Where<HeaderField>(x => !x.Name.StartsWith(":")));
            InputStream = payload == null ? new MemoryStream() : new MemoryStream(payload);
#pragma warning disable 618
            Protocol = headers.ContainsKey(":version") ? headers[":version"] : null;
#pragma warning restore 618
        }

        public HttpMethod Method { get; }
        public HttpHeaderCollection Headers { get; }
        public Uri Url { get; }
        public string RawUrl { get; }

        [Obsolete("As HTTP/2 omits a version header, it is deprecated and will be removed in a future release")]
        [CanBeNull]
        public string Protocol { get; }

        IMethod IRequest.Method => Method;
        public Stream InputStream { get; }

        /// <summary>
        ///     Parses a HTTP header from a string
        /// </summary>
        /// <param name="requestHeader">the header to parse</param>
        /// <param name="methodResolver">the method resolver</param>
        /// <param name="connection">the connection that the requestHeader was sourced from. Used to determine the scheme (http/https)</param>
        /// <returns>an optional that will determine the next steps of context construction</returns>
        /// <exception cref="BadRequestException">the request was of an invalid structure</exception>
        public static HttpParserOptional ParseFrom(string requestHeader, MethodResolver<HttpMethod> methodResolver,
            IConnection connection)
        {
            var lines = requestHeader.Split(new[] {"\r\n"}, StringSplitOptions.None);
            if (lines.Length < 1)
                throw new BadRequestException("Bad Request (HTTP/1 parser failure)",
                    new ArgumentException("Request header contains less than one line", nameof(requestHeader)));

            var rparts = lines[0].Split(' ');
            var method = rparts.First();
            var path = rparts.Where(part => part != rparts[0]).Where(part => part != rparts[rparts.Length - 1])
                .Aggregate("", (current, part) => current + part + " ").TrimEnd();
            var proto = rparts.Last();

            if (string.IsNullOrEmpty(method) ||
                string.IsNullOrEmpty(path) ||
                string.IsNullOrEmpty(proto))
                throw new BadRequestException("Bad Request (HTTP/1 parser failure)",
                    new NullReferenceException(
                        "The method, path, and/or protocol parameters aren't present in the request line"));

            var headers = new HttpHeaderCollection(new Dictionary<string, string>
            {
                {":method", method},
                {":path", path},
                {":version", proto},
                {":scheme", connection is SslListener.SecureConnection ? "https" : "http"}
            }.Select(x => new HeaderField(){Name = x.Key, Value = x.Value}));
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var colonIdx = line.IndexOf(':');
                if (colonIdx == -1) throw new BadRequestException("Bad Request (HTTP/1 parser failure)");
                var name = line.Substring(0, colonIdx).Trim().ToLowerInvariant();
                var value = line.Substring(colonIdx + 1).Trim();
                if (name == "host")
                    name = ":authority";
                headers[name] = value;
            }

            return new HttpParserOptional
            {
                ExpectPayload = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"])).ExpectPayload ||
                                headers.ContainsKey("content-length"),
                Headers = headers,
                MethodResolver = methodResolver,
                Payload = null
            };
        }

        public static HttpParserOptional ParseFrom(HttpHeaderCollection headers,
            MethodResolver<HttpMethod> methodResolver)
        {
            return new HttpParserOptional
            {
                ExpectPayload = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"])).ExpectPayload ||
                                headers.ContainsKey("content-length"),
                Headers = headers,
                MethodResolver = methodResolver,
                Payload = null
            };
        }

        public HttpHeaderCollection ToDictionary()
        {
            return new HttpHeaderCollection(new List<HeaderField>()
            {
                new HeaderField(){Name =":method", Value=Method.Id},
                new HeaderField(){Name =":authority", Value =Url.Authority},
                new HeaderField(){Name=":path", Value=RawUrl},
#pragma warning disable 618
                new HeaderField(){Name=":version", Value=Protocol}
#pragma warning restore 618
            }.Concat(Headers));
        }
    }

    public class HttpParserOptional
    {
        public HttpRequest Request => new HttpRequest(Headers, Payload, MethodResolver);
        public HttpHeaderCollection Headers { get; set; }
        public bool ExpectPayload { get; set; }
        public byte[] Payload { get; set; }
        public MethodResolver<HttpMethod> MethodResolver { get; set; }
    }
}