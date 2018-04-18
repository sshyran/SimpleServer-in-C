using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SimpleServer.Exceptions;
using SimpleServer.Logging;

namespace SimpleServer.Internals
{
    public class SimpleServerEngine
    {
        internal List<SimpleServerMethod> _methods;

        private readonly SimpleServerHost _host;
        private List<SimpleServerHost> _additionalHosts;
        private string[] _hostnames;

        private readonly SimpleServerListener _listener;
        private readonly SimpleServer _server;

        public SimpleServerEngine(SimpleServerHost host, SimpleServer server)
        {
            var fqdn = new List<string> {host.FQDN};
            fqdn.AddRange(host.AliasFQDNs);
            _hostnames = fqdn.ToArray();
            _server = server;
            _host = host;
            _additionalHosts = new List<SimpleServerHost>();
            _listener = new SimpleServerListener(new IPEndPoint(host.Endpoint.Scope, host.Endpoint.Port), _server,
                this);
        }

        public void AddHost(SimpleServerHost host)
        {
            if (host.Endpoint.Port != _host.Endpoint.Port)
            {
                throw new ArgumentException(
                    "Additional hosts must have the same endpoint as the host used to create the engine (port)");
            }

            if (host.Endpoint.Scope.Equals(_host.Endpoint.Scope))
            {
                throw new ArgumentException(
                    "Additional hosts must have the same endpoint as the host used to create the engine (scope)");
            }

            _additionalHosts.Add(host);
        }

        #region Statics

        public static async Task<SimpleServerRequest> ProcessRequestAsync(SimpleServerConnection connection)
        {
            try
            {
                var reader = new StreamReader(connection.Stream);
                string line = null;
                var lines = new List<string>();
                while (!string.IsNullOrWhiteSpace(line = await reader.ReadLineAsync())) lines.Add(line);

                var headers = new Dictionary<string, string>();
                foreach (var headerLine in lines)
                {
                    if (lines.IndexOf(headerLine) == 0)
                        continue;
                    var parts = headerLine.Split(':');
                    var key = parts[0];
                    var value = parts[1].Trim();
                    headers.Add(key, value);
                }

                var rline = lines.ElementAt(0);
                var rparts = rline.Split(' ');
                var smethod = rparts[0];
                var version = rparts.Last();
                var path = rparts.Where(urlPart => urlPart != smethod).TakeWhile(urlPart => urlPart != version)
                    .Aggregate("", (current, urlPart) => current + urlPart);
                var wildcard = false;
                if (!headers.ContainsKey("Host"))
                    if (version == "HTTP/1.1")
                        throw new RfcViolationException("rfc2616-s14.23");
                    else if (!connection._server.HasWildcardHost())
                        throw new Exception("SS-HOST-404");
                    else
                        wildcard = true;

                var host = (wildcard
                               ? connection._server.GetWildcardHost()
                               : connection.GetListener().GetEngine().GetHost(headers["Host"])) ??
                           connection._server.GetWildcardHost();
                var method = host.Methods.Get(smethod);
                var url = new UriBuilder(wildcard ? "" : headers["Host"] + path).Uri;
                Stream stream;
                if (method.HasInputStream)
                {
                    var contentLength = long.Parse(headers["Content-Length"]);
                    var buffer = new char[contentLength];
                    await reader.ReadAsync(buffer, 0, (int) contentLength);
                    stream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));
                }
                else
                {
                    stream = null;
                }

                return new SimpleServerRequest
                {
                    Headers = headers,
                    InputStream = stream,
                    LocalEndpoint = connection.LocalEndPoint,
                    Method = method,
                    RemoteEndpoint = connection.RemoteEndPoint,
                    RequestUri = url,
                    Version = version,
                    Connection = connection,
                    RawUrl = path,
                    Host = host
                };
            }
            catch (Exception ex)
            {
                Log.Error("An error has occured when serving a request.");
                Log.Error(ex);
                connection.Dispose();
                Log.Error("The clients connection will be terminated.");
                return SimpleServerRequest.Empty;
            }
        }
        // TODO: Rewrite engine for better performance

        #endregion

        public void Start()
        {
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public SimpleServerHost GetHost(string fqdn)
        {
            SimpleServerHost wildcard;
            SimpleServerHost result = null;
            foreach (var host in GetHosts())
            {
                if (host.FQDN == "*")
                {
                    wildcard = host;
                }
            }

            return result;
        }

        public IEnumerable<SimpleServerHost> GetHosts()
        {
            var hosts = new List<SimpleServerHost> {_host};
            hosts.AddRange(_additionalHosts);
            return hosts.ToArray();
        }
    }
}