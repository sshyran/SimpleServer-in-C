#region

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http1
{
    /// <summary>
    ///     Thrown when a HTTP/2 request is caught by the HTTP/1 parser. This should be an indicator to a dual engine that the
    ///     request should be moved to HTTP/2.
    /// </summary>
    public class Http2RequestException : Exception
    {
        public Http2RequestException() : base(
            "A HTTP/2 request was sent to a HTTP/1 endpoint, and isn't configured to use HTTP/2")
        {
        }
    }
}