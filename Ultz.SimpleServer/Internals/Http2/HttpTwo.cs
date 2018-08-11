using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals.Http;
using Ultz.SimpleServer.Internals.Http2.Hpack;
using Ultz.SimpleServer.Internals.Http2.Http2;

namespace Ultz.SimpleServer.Internals.Http2
{
    public class HttpTwo : Http.Http
    {
        ConnectionConfiguration config = new ConnectionConfigurationBuilder(true)
            .UseStreamListener(AcceptIncomingStream)
            .UseSettings(Settings.DefaultSimpleServer)
            .UseHuffmanStrategy(HuffmanStrategy.IfSmaller)
            .Build();

        private static bool AcceptIncomingStream(IStream arg)
        {
            
        }

        public override async Task HandleConnectionAsync(IConnection connection, ILogger logger)
        {
            var wrappedStreams = new ConnectionByteStream(connection);
            await HandleConnection(
                logger,
                wrappedStreams,
                connection.Id);
        }

        private static async Task HandleConnection(
            ILogger logger,
            ConnectionByteStream stream,
            int connectionId)
        {
            var upgradeReadStream = new HttpStream(stream);
            ServerUpgradeRequest upgrade = null;
            var writeAndCloseableByteStream = stream;
            try
            {
                // Wait for either HTTP/1 upgrade header or HTTP/2 magic header
                await upgradeReadStream.WaitForHttpHeader();
                var headerBytes = upgradeReadStream.HeaderBytes;
                if (MaybeHttpStart(headerBytes))
                {
                    // This seems to be a HTTP/2 request
                    // No upgrade necessary
                    // Make the header rereadable by the stream reader consumer,
                    // so that the library can also read the preface
                    upgradeReadStream.UnreadHttpHeader();
                }
                else
                {
                    // This seems to be a HTTP/1 request
                    // Parse the header and check whether it's an upgrade
                    var request = HttpRequest.ParseFrom(
                        Encoding.ASCII.GetString(
                            headerBytes.Array, headerBytes.Offset, headerBytes.Count - 4),);
                    // Assure that the HTTP/2 library does not get passed the HTTP/1 request
                    upgradeReadStream.ConsumeHttpHeader();

                    if (request.Protocol != "HTTP/1.1")
                        throw new Exception("Invalid upgrade request");

                    // If the request has some payload we can't process it in this demo
                    string contentLength;
                    if (request.Headers.TryGetValue("content-length", out contentLength))
                    {
                        await writeAndCloseableByteStream.WriteAsync(
                            new ArraySegment<byte>(UpgradeErrorResponse));
                        await writeAndCloseableByteStream.CloseAsync();
                        return;
                    }

                    string connectionHeader;
                    string hostHeader;
                    string upgradeHeader;
                    string http2SettingsHeader;
                    if (!request.Headers.TryGetValue("connection", out connectionHeader) ||
                        !request.Headers.TryGetValue("host", out hostHeader) ||
                        !request.Headers.TryGetValue("upgrade", out upgradeHeader) ||
                        !request.Headers.TryGetValue("http2-settings", out http2SettingsHeader) ||
                        upgradeHeader != "h2c" ||
                        http2SettingsHeader.Length == 0)
                        throw new Exception("Invalid upgrade request");

                    var connParts =
                        connectionHeader
                            .Split(new[] {','})
                            .Select(p => p.Trim())
                            .ToArray();
                    if (connParts.Length != 2 ||
                        !connParts.Contains("Upgrade") ||
                        !connParts.Contains("HTTP2-Settings"))
                        throw new Exception("Invalid upgrade request");

                    var headers = new List<HeaderField>
                    {
                        new HeaderField {Name = ":method", Value = request.Method},
                        new HeaderField {Name = ":path", Value = request.Path},
                        new HeaderField {Name = ":scheme", Value = "http"}
                    };
                    foreach (var kvp in request.Headers)
                    {
                        // Skip Connection upgrade related headers
                        if (kvp.Key == "connection" ||
                            kvp.Key == "upgrade" ||
                            kvp.Key == "http2-settings")
                            continue;
                        headers.Add(new HeaderField
                        {
                            Name = kvp.Key,
                            Value = kvp.Value
                        });
                    }

                    var upgradeBuilder = new ServerUpgradeRequestBuilder();
                    upgradeBuilder.SetHeaders(headers);
                    upgradeBuilder.SetHttp2Settings(http2SettingsHeader);
                    upgrade = upgradeBuilder.Build();

                    if (!upgrade.IsValid)
                    {
                        await writeAndCloseableByteStream.WriteAsync(
                            new ArraySegment<byte>(UpgradeErrorResponse));
                        await writeAndCloseableByteStream.CloseAsync();
                        return;
                    }

                    // Respond to upgrade
                    await writeAndCloseableByteStream.WriteAsync(
                        new ArraySegment<byte>(UpgradeSuccessResponse));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during connection upgrade: {0}", e.Message);
                await writeAndCloseableByteStream.CloseAsync();
                return;
            }

            // Build a HTTP connection on top of the stream abstraction
            var http2Con = new Connection(
                config, upgradeReadStream, writeAndCloseableByteStream,
                new Connection.Options
                {
                    Logger = logger,
                    ServerUpgradeRequest = upgrade
                });

            // Close the connection if we get a GoAway from the client
            var remoteGoAwayTask = http2Con.RemoteGoAwayReason;
            var closeWhenRemoteGoAway = Task.Run(async () =>
            {
                await remoteGoAwayTask;
                await http2Con.GoAwayAsync(ErrorCode.NoError, true);
            });
        }
    }
}