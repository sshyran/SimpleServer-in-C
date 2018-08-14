#region

using System.IO;
using System.Text;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace Ultz.SimpleServer.Internals.Http1
{
    public class HttpOneResponse : HttpResponse
    {
        private readonly IConnection _connection;
        private readonly HttpRequest _request;

        public HttpOneResponse(HttpRequest request, IConnection conection)
        {
            _connection = conection;
            _request = request;
        }

        public override void Close(bool force = false)
        {
            if (force)
                _connection.Close();
            const string crlf = "\r\n";
            var response = "";
#pragma warning disable 612
            response += _request.Protocol + " " + StatusCode + " " + ReasonPhrase + crlf;
#pragma warning restore 612
            foreach (var header in Headers)
                response += header.Key + ": " + header.Value + crlf;
            response += crlf;
            var bytes = Encoding.UTF8.GetBytes(response);
            _connection.Stream.Write(bytes, 0, bytes.Length);
            bytes = ((MemoryStream) OutputStream).ToArray();
            _connection.Stream.Write(bytes, 0, bytes.Length);
            _connection.Close();
        }
    }
}