// HttpTwo.cs - Ultz.SimpleServer.Minimal
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Http2;
using Http2.Hpack;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals.Http;
using Ultz.SimpleServer.Internals.Http1;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    /// <inheritdoc />
    public class HttpTwo : Http.Http
    {
        private static readonly byte[] Http2Start = Encoding.ASCII.GetBytes(
            "PRI * HTTP/2.0\r\n\r\n");


        private static readonly byte[] UpgradeSuccessResponse = Encoding.ASCII.GetBytes(
            "HTTP/1.1 101 Switching Protocols\r\n" +
            "Connection: Upgrade\r\n" +
            "Upgrade: h2c\r\n\r\n");

        /// <inheritdoc />
        public override async Task HandleConnectionAsync(IConnection connection, ILogger logger)
        {
            var wrappedStreams = new ConnectionByteStream(connection);
            await HandleConnection(
                logger,
                wrappedStreams);
        }

        private static bool MaybeHttpStart(ArraySegment<byte> bytes)
        {
            if (bytes.Count != Http2Start.Length) return false;
            return !Http2Start.Where((t, i) => bytes.Array[bytes.Offset + i] != t).Any();
        }

        private async Task HandleConnection(
            ILogger logger,
            ConnectionByteStream stream)
        {
            var config = new ConnectionConfigurationBuilder(true)
                .UseStreamListener(arg =>
                {
                    Task.Run(() =>
                    {
                        var headers = arg.ReadHeadersAsync().GetAwaiter().GetResult();
                        var optional = HttpRequest.ParseFrom(new HttpHeaderCollection(headers),
                            new HttpMethodResolver(Methods));
                        if (optional.ExpectPayload)
                        {
                            var seg = new ArraySegment<byte>(new byte[int.Parse(optional.Headers["content-length"])]);
                            // TODO: Use the StreamReadResult, as sometimes the packets won't all be sent at once
                            arg.ReadAsync(seg).GetAwaiter().GetResult();
                            optional.Payload = seg.ToArray();
                        }

                        var req = optional.Request;
                        PassContext(new HttpContext(req, new HttpTwoResponse(arg), stream.Connection, logger));
                    });
                    return true;
                })
                .UseSettings(Settings.Default)
                .UseHuffmanStrategy(HuffmanStrategy.IfSmaller)
                .Build();
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
                    upgradeReadStream.UnreadHttpHeader();
                }
                else
                {
                    // This seems to be a HTTP/1 request
                    // Parse the header and check whether it's an upgrade
                    var req = HttpRequest.ParseFrom(
                        Encoding.ASCII.GetString(
                            headerBytes.Array, headerBytes.Offset, headerBytes.Count - 4),
                        (MethodResolver<HttpMethod>) MethodResolver, stream.Connection);

                    if (req.ExpectPayload && req.Headers.TryGetValue("content-length", out string contentLength))
                    {
                        if (int.TryParse(contentLength, out var length))
                        {
                            await upgradeReadStream.WaitForPayload(length);
                            req.Payload = upgradeReadStream.Payload.ToArray();
                        }
                        else
                        {
                            await stream.WriteAsync(
                                new ArraySegment<byte>(Encoding.ASCII.GetBytes(
                                    "HTTP/1.1 400 Bad Request\r\nServer: SimpleServer/1.0 sshttp2\r\nContent-Type: text/html\r\nContent-Length: 26\r\n\r\nThe payload was too large.")));
                            await stream.CloseAsync();
                            return;
                        }
                    }
                    else if (req.ExpectPayload)
                    {
                        await stream.WriteAsync(
                            new ArraySegment<byte>(Encoding.ASCII.GetBytes(
                                "HTTP/1.1 400 Bad Request\r\nServer: SimpleServer/1.0 sshttp2\r\nContent-Type: text/html\r\nContent-Length: 25\r\n\r\nNo content-length header.")));
                        await stream.CloseAsync();
                        return;
                    }

                    // Assure that the HTTP/2 library does not get passed the HTTP/1 request
                    upgradeReadStream.Consume();

                    var request = req.Request;

#pragma warning disable 618
                    if (request.Protocol != "HTTP/1.1")
#pragma warning restore 618
                    {
                        PassContext(new HttpContext(request, new HttpOneResponse(request, stream.Connection),
                            stream.Connection, logger));
                        return;
                    }

                    if (!request.Headers.TryGetValue("connection", out string connectionHeader) ||
                        !request.Headers.TryGetValue("host", out string _) ||
                        !request.Headers.TryGetValue("upgrade", out string upgradeHeader) ||
                        !request.Headers.TryGetValue("http2-settings", out string http2SettingsHeader) ||
                        upgradeHeader != "h2c" ||
                        http2SettingsHeader.Length == 0)
                    {
                        // STAY H1
                        PassContext(new HttpContext(request, new HttpOneResponse(request, stream.Connection),
                            stream.Connection, logger));
                        return;
                    }

                    var connParts =
                        connectionHeader
                            .Split(',')
                            .Select(p => p.Trim())
                            .ToArray();
                    if (connParts.Length != 2 ||
                        !connParts.Contains("Upgrade") ||
                        !connParts.Contains("HTTP2-Settings"))
                        PassContext(new HttpContext(request, new HttpOneResponse(request, stream.Connection),
                            stream.Connection, logger));

                    var headers = new List<HeaderField>
                    {
                        new HeaderField {Name = ":method", Value = request.Method.Id},
                        new HeaderField {Name = ":path", Value = request.RawUrl},
                        new HeaderField
                        {
                            Name = ":scheme",
#if NETCOREAPP2_1 || NETSTANDARD2_1
                            Value = stream.Connection is SslListener.SecureConnection ? "https" : "http"
                            #else
                            Value = "http"
#endif
                        }
                    };
                    foreach (var kvp in request.Headers)
                    {
                        // Skip Connection upgrade related headers
                        if (kvp.Name == "connection" ||
                            kvp.Name == "upgrade" ||
                            kvp.Name == "http2-settings")
                            continue;
                        headers.Add(new HeaderField
                        {
                            Name = kvp.Name,
                            Value = kvp.Value
                        });
                    }

                    var upgradeBuilder = new ServerUpgradeRequestBuilder();
                    upgradeBuilder.SetHeaders(headers);
                    upgradeBuilder.SetPayload(new ArraySegment<byte>(req.Payload));
                    upgradeBuilder.SetHttp2Settings(http2SettingsHeader);
                    upgrade = upgradeBuilder.Build();

                    if (!upgrade.IsValid)
                    {
                        // STAY H1
                        PassContext(new HttpContext(request, new HttpOneResponse(request, stream.Connection),
                            stream.Connection, logger));
                        return;
                    }

                    // Respond to upgrade
                    await writeAndCloseableByteStream.WriteAsync(
                        new ArraySegment<byte>(UpgradeSuccessResponse));
                }
            }
            catch (Exception e)
            {
                logger?.LogError(new EventId(stream.Connection.Id, "StartFail"), e,
                    "An error occured while trying to upgrade a HTTP/1.X request to a H2 request.");
                await writeAndCloseableByteStream.CloseAsync();
                return;
            }

            // Build a H2 connection
            var http2Con = new Connection(
                config, upgradeReadStream, writeAndCloseableByteStream,
                new Connection.Options
                {
                    Logger = logger,
                    ServerUpgradeRequest = upgrade
                });

            // Close the connection if we get a GoAway from the client
            var remoteGoAwayTask = http2Con.RemoteGoAwayReason;
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                await remoteGoAwayTask;
                await http2Con.GoAwayAsync(ErrorCode.NoError, true);
            });
        }
    }
}