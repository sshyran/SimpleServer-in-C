using System.Collections.Generic;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpMethod : IMethod
    {
        public static readonly ICollection<HttpMethod> DefaultMethods =
            new List<HttpMethod> {Get, Head, Post, Put, Delete, Options, Trace, Patch, Connect}.AsReadOnly();

        public static HttpMethod Get => new HttpMethod {Name = "GET", HasInputStream = false};

        public static HttpMethod Post => new HttpMethod {Name = "POST", HasInputStream = true};

        public static HttpMethod Put => new HttpMethod {Name = "PUT", HasInputStream = true};

        public static HttpMethod Patch => new HttpMethod {Name = "PATCH", HasInputStream = true};

        public static HttpMethod Delete => new HttpMethod {Name = "DELETE", HasInputStream = false};

        public static HttpMethod Head => new HttpMethod {Name = "HEAD", HasInputStream = false};

        public static HttpMethod Options => new HttpMethod {Name = "OPTIONS", HasInputStream = false};

        public static HttpMethod Trace => new HttpMethod {Name = "TRACE", HasInputStream = false};

        public static HttpMethod Connect => new HttpMethod {Name = "CONNECT", HasInputStream = false};

        public string Name { get; set; }
        public bool HasInputStream { get; set; }
    }
}