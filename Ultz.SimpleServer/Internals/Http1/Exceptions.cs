using System;

namespace Ultz.SimpleServer.Internals.Http1
{
    public class RequestException : Exception
    {
        public RequestException() : base("Failed to parse the request, it may be invalid")
        {
        }

        public RequestException(string msg,Exception innerException = null) : base(msg,innerException)
        {
        }
    }
}