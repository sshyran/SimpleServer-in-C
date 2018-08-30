#region

using System;
using System.Collections;
using System.Text;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpMethod : IMethod
    {
        public static bool operator ==(HttpMethod left, HttpMethod right)
        {
            return (object)left != null && left.Equals(right);
        }

        public static bool operator !=(HttpMethod left, HttpMethod right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is HttpMethod method && (method.Id == Id && method.ExpectPayload == ExpectPayload);
        }

        public static HttpMethod Get => new HttpMethod("GET", false);
        public static HttpMethod Post => new HttpMethod("POST", true);
        public static HttpMethod Put => new HttpMethod("PUT", true);
        public static HttpMethod Patch => new HttpMethod("PATCH", true);
        public static HttpMethod Delete => new HttpMethod("DELETE", false);
        public static HttpMethod Options => new HttpMethod("OPTIONS", false);
        public static HttpMethod Head => new HttpMethod("HEAD", false);
        public static HttpMethod Trace => new HttpMethod("TRACE", false);
        public static HttpMethod Connect => new HttpMethod("CONNECT", false);

        public HttpMethod(string name, bool payload)
        {
            Id = name;
            ExpectPayload = payload;
        }

        public string Id { get; }
        public bool ExpectPayload { get; }
        byte[] IMethod.Id => Encoding.UTF8.GetBytes(Id);
    }
}