using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleServer.Internals
{
    public class SimpleServerRequest
    {
        private SimpleServerConnection client;

        // Request regex developed by Ultz
        internal static string requestRegex = @"^(?<method>GET|HEAD|POST|PUT|DELETE|OPTIONS|TRACE|PATCH).(?<url>.*).(?<version>(HTTP\/1\.1|HTTP\/1\.0))$";

        internal SimpleServerRequest()
        {
        }

        internal async Task ProcessAsync()
        {
            var reader = new StreamReader(client.Stream);

            StringBuilder request = await ReadRequest(reader);

            var localEndpoint = client.LocalEndPoint;
            var remoteEnpoint = client.RemoteEndPoint;

            LocalEndpoint = (IPEndPoint)localEndpoint;
            RemoteEndpoint = (IPEndPoint)remoteEnpoint;

            var lines = request.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            ParseHeaders(lines);
            ParseRequestLine(lines);

            await PrepareInputStream(reader);

            // make sure we don't run into any RAM problems
            request.Clear();
            request = null;
        }

        private void ParseRequestLine(string[] lines)
        {
            var line = lines.ElementAt(0);
            Regex regex = new Regex(requestRegex);
            Match m = regex.Match(line);

            if (!m.Success)
            {
                throw new Exception("Invalid request -- couldn't match request line to regex");
            }
            //if (SimpleServerConfig.Http11Only && version)
            //if (!Headers.ContainsKey("Host"))
            //{
            //    if ()
            //}

            //var url = new UriBuilder(Headers.Host + m.Groups["url"]).Uri;
            //var httpMethod = m.Groups["method"];

            //Version = m.Groups["version"].Value;
            //Method = httpMethod.Value;
            //RequestUri = url;
        }

        private async Task PrepareInputStream(StreamReader reader)
        {
            if (Method == "POST" || Method == "PUT" || Method == "PATCH")
            {

                InputStream = new MemoryStream();

                await reader.BaseStream.CopyToAsync(InputStream);
            }
        }

        private void ParseHeaders(IEnumerable<string> lines)
        {
            lines = lines.Skip(1);
            //Headers.ParseHeaderLines(lines);
        }

        private static async Task<StringBuilder> ReadRequest(StreamReader reader)
        {
            var request = new StringBuilder();

            string line = null;
            while (!string.IsNullOrWhiteSpace(line = await reader.ReadLineAsync()))
            {
                request.AppendLine(line);
            }

            var requestStr = request.ToString();
            return request;
        }

        /// <summary>
        /// Gets the endpoint of the listener that received the request.
        /// </summary>
        public IPEndPoint LocalEndpoint { get; internal set; }

        /// <summary>
        /// Gets the endpoint that sent the request.
        /// </summary>
        public IPEndPoint RemoteEndpoint { get; internal set; }

        /// <summary>
        /// Gets the URI send with the request.
        /// </summary>
        public Uri RequestUri { get; internal set; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public string Method { get; internal set; }

        /// <summary>
        /// Gets the headers of the HTTP request.
        /// </summary>
        public Dictionary<string,string> Headers { get; internal set; }

        /// <summary>
        /// Gets the stream containing the content sent with the request.
        /// </summary>
        public Stream InputStream { get; internal set; }

        /// <summary>
        /// Gets the HTTP version.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the request was sent locally or not.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return RemoteEndpoint.Address.Equals(LocalEndpoint.Address);
            }
        }

        /// <summary>
        /// Reads the content of the request as a string.
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadContentAsStringAsync()
        {
            var length = InputStream.Length;
            byte[] buffer = new byte[length];
            await InputStream.ReadAsync(buffer, 0, (int)length);
            return Encoding.UTF8.GetString(buffer);
        }

        public SimpleServerConnection Connection { get; internal set; }
    }
}
