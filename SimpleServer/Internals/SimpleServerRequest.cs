using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerRequest
    {
        private SimpleServerConnection client;

        internal SimpleServerRequest()
        {
        }

        /// <summary>
        ///     Gets the endpoint of the listener that received the request.
        /// </summary>
        public IPEndPoint LocalEndpoint { get; internal set; }

        /// <summary>
        ///     Gets the endpoint that sent the request.
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; internal set; }

        /// <summary>
        ///     Gets the URI send with the request.
        /// </summary>
        public Uri RequestUri { get; internal set; }

        /// <summary>
        ///     Gets the HTTP method.
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        ///     Gets the headers of the HTTP request.
        /// </summary>
        public Dictionary<string, string> Headers { get; internal set; }

        /// <summary>
        ///     Gets the stream containing the content sent with the request.
        /// </summary>
        public Stream InputStream { get; internal set; }

        /// <summary>
        ///     Gets the HTTP version.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the request was sent locally or not.
        /// </summary>
        public bool IsLocal => RemoteEndpoint.Address.Equals(LocalEndpoint.Address);

        public SimpleServerConnection Connection { get; internal set; }
        public string RawUrl { get; internal set; }

        public string FormattedUrl => RawUrl.UrlFormat();

        /// <summary>
        ///     Reads the content of the request as a string.
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadContentAsStringAsync()
        {
            var length = InputStream.Length;
            var buffer = new byte[length];
            await InputStream.ReadAsync(buffer, 0, (int) length);
            return Encoding.UTF8.GetString(buffer);
        }


        public SimpleServerHost Host { get; set; }
    }
}