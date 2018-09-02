#region

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals.Http;
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http1
{
    /// <inheritdoc />
    public class HttpOne : Http.Http
    {
        /// <inheritdoc />
        public override async Task HandleConnectionAsync(IConnection connection, ILogger logger)
        {
            var streams = connection.Stream.CreateStreams();
            var outStream = streams.WriteableStream;
            var stream = new HttpStream(streams.ReadableStream);
            await stream.WaitForHttpHeader();
            var req = HttpRequest.ParseFrom(
                Encoding.ASCII.GetString(
                    stream.HeaderBytes.Array, stream.HeaderBytes.Offset, stream.HeaderBytes.Count - 4),
                (MethodResolver<HttpMethod>) MethodResolver, connection);
            if (req.ExpectPayload && req.Headers.TryGetValue("content-length", out var contentLength))
            {
                if (int.TryParse(contentLength, out var length))
                {
                    await stream.WaitForPayload(length);
                    req.Payload = stream.Payload.ToArray();
                }
                else
                {
                    await outStream.WriteAsync(
                        new ArraySegment<byte>(Encoding.ASCII.GetBytes(
                            "HTTP/1.1 400 Bad Request\r\nServer: SimpleServer\r\nContent-Type: text/html\r\nContent-Length: 26\r\n\r\nThe payload was too large.")));
                    await outStream.CloseAsync();
                    return;
                }
            }
            else if (req.ExpectPayload)
            {
                await outStream.WriteAsync(
                    new ArraySegment<byte>(Encoding.ASCII.GetBytes(
                        "HTTP/1.1 400 Bad Request\r\nServer: SimpleServer\r\nContent-Type: text/html\r\nContent-Length: 25\r\n\r\nNo content-length header.")));
                await outStream.CloseAsync();
                return;
            }

            var request = req.Request;
#pragma warning disable 618
            logger.LogInformation(request.Method.Id +" "+ request.RawUrl + " "+ request.Protocol);
#pragma warning restore 618
            PassContext(new HttpContext(request, new HttpOneResponse(request, connection), connection, logger));
        }
    }
}