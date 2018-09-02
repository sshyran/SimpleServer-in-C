#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace Ultz.SimpleServer.Internals.Http1
{
    /// <inheritdoc />
    public class HttpOneResponse : HttpResponse
    {
        private readonly IConnection _connection;
        private readonly HttpRequest _request;

        /// <inheritdoc />
        public HttpOneResponse(HttpRequest request, IConnection conection)
        {
            _connection = conection;
            _request = request;
        }

        /// <inheritdoc />
        public override void Close(bool force = false)
        {
            if (force)
            {
                _connection.Close();
            }

            var headers = Headers.Select(x => new KeyValuePair<string, string>(x.Key.ToLower(), x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
            if (headers.TryGetValue("server", out var val))
                headers["server"] = "SimpleServer/1.0 " + val;
            else
                headers["server"] = "SimpleServer/1.0";
            const string crlf = "\r\n";
            var response = "";
#pragma warning disable 618
#pragma warning disable 612
            response += _request.Protocol + " " + StatusCode + " " + ReasonPhrase + crlf;
#pragma warning restore 618
#pragma warning restore 612
            foreach (var header in Headers)
                response += header.Key + ": " + header.Value + crlf;
            response += crlf;
            // write headers
            var bytes = Encoding.UTF8.GetBytes(response);
            _connection.Stream.Write(bytes, 0, bytes.Length);
            // write response
            bytes = ((MemoryStream) OutputStream).ToArray();
            _connection.Stream.Write(bytes, 0, bytes.Length);
            // close the connection
            // TODO: Keep Alive support?
            _connection.Close();
        }
    }
}