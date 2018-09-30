// HttpRequest.cs - Ultz.SimpleServer.Minimal
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Http2.Hpack;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP request
    /// </summary>
    public class HttpRequest : IRequest
    {
        /// <summary>
        ///     Instantiates with the given <see cref="HttpHeaderCollection" />, payload, and method resolver.
        /// </summary>
        /// <param name="headers">
        ///     a collection of HTTP/2 headers with pseudo headers included (:method, :scheme, :authority, :path,
        ///     and (optionally), :version
        /// </param>
        /// <param name="payload">the request payload (can be null)</param>
        /// <param name="methodResolver">
        ///     a method resolver to use to map the method name to a server-implemented
        ///     <see cref="HttpMethod" />
        /// </param>
        public HttpRequest(HttpHeaderCollection headers, byte[] payload,
            MethodResolver<HttpMethod> methodResolver)
        {
            if (payload != null)
                InputStream = new MemoryStream(payload);
            Method = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"].ToUpper()));
            RawMethod = headers[":method"];
            Url = new Uri(headers[":scheme"] + "://" + headers[":authority"] + headers[":path"]);
            RawUrl = headers[":path"];
            Headers = new HttpHeaderCollection(headers.Where<HeaderField>(x => !x.Name.StartsWith(":")));
            InputStream = payload == null ? new MemoryStream() : new MemoryStream(payload);
#pragma warning disable 618
            Protocol = headers.ContainsKey(":version") ? headers[":version"] : null;
#pragma warning restore 618
        }

        /// <summary>
        ///     The HTTP method attached to this request.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        ///     The raw method name as it was when passed to the constructor
        /// </summary>
        public string RawMethod { get; }

        /// <summary>
        ///     A collection of additional headers sent with this request.
        /// </summary>
        public HttpHeaderCollection Headers { get; }

        /// <summary>
        ///     The URL attached to this request.
        /// </summary>
        public Uri Url { get; }

        /// <summary>
        ///     The raw URL as it was when passed to the constructor.
        /// </summary>
        public string RawUrl { get; }

        /// <summary>
        ///     The HTTP protcol version. Can be null.
        /// </summary>
        [Obsolete("As HTTP/2 omits a version header, it is deprecated and will be removed in a future release")]
        public string Protocol { get; }

        IMethod IRequest.Method => Method;

        /// <inheritdoc />
        public Stream InputStream { get; }

        /// <summary>
        ///     Parses a HTTP header from a string
        /// </summary>
        /// <param name="requestHeader">the header to parse</param>
        /// <param name="methodResolver">the method resolver</param>
        /// <param name="connection">
        ///     the connection that the requestHeader was sourced from. Used to determine the scheme
        ///     (http/https)
        /// </param>
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
                {
                    ":scheme",
                    connection.GetType().FullName == "Ultz.SimpleServer.Common.SslListener.SecureConnection"
                        ? "https"
                        : "http"
                }
            }.Select(x => new HeaderField {Name = x.Key, Value = x.Value}));
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
                                headers.ContainsKey("content-length") && headers["content-length"] != "0",
                Headers = headers,
                MethodResolver = methodResolver,
                Payload = null
            };
        }

        /// <summary>
        ///     Parses a HTTP header from a HTTP/2 header collection
        /// </summary>
        /// <param name="headers">the header collection including pseudo headers</param>
        /// <param name="methodResolver">the method resolver</param>
        /// <returns>an optional used to determine if the invoker should listen for a payload</returns>
        public static HttpParserOptional ParseFrom(HttpHeaderCollection headers,
            MethodResolver<HttpMethod> methodResolver)
        {
            return new HttpParserOptional
            {
                ExpectPayload = methodResolver.GetMethod(Encoding.UTF8.GetBytes(headers[":method"])).ExpectPayload ||
                                headers.ContainsKey("content-length") && headers["content-length"] != "0",
                Headers = headers,
                MethodResolver = methodResolver,
                Payload = null
            };
        }

        /// <summary>
        ///     Converts this HTTP request to a HTTP/2 header collection.
        /// </summary>
        /// <returns>the HTTP/2 header representation of this request</returns>
        public HttpHeaderCollection ToDictionary()
        {
            return new HttpHeaderCollection(new List<HeaderField>
            {
                new HeaderField {Name = ":method", Value = RawMethod},
                new HeaderField {Name = ":authority", Value = Url.Authority},
                new HeaderField {Name = ":path", Value = RawUrl},
#pragma warning disable 618
                new HeaderField {Name = ":version", Value = Protocol}
#pragma warning restore 618
            }.Concat(Headers));
        }
    }

    /// <summary>
    ///     Represents a parser result, used to determine if a request payload is expected.
    /// </summary>
    public class HttpParserOptional
    {
        /// <summary>
        ///     Instantiates a <see cref="HttpRequest" /> from the contents of this optional.
        /// </summary>
        public HttpRequest Request => new HttpRequest(Headers, Payload, MethodResolver);

        /// <summary>
        ///     A HTTP request header represented as a HTTP/2 header collection
        /// </summary>
        public HttpHeaderCollection Headers { get; set; }

        /// <summary>
        ///     True if the parser should expect a payload, false otherwise. This is set by the parser and is not automatically
        ///     determined.
        /// </summary>
        public bool ExpectPayload { get; set; }

        /// <summary>
        ///     A payload associated with the request. Can be null.
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        ///     The method resolver to be used.
        /// </summary>
        public MethodResolver<HttpMethod> MethodResolver { get; set; }
    }
}