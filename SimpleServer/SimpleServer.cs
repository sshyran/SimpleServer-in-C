using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SimpleServer.Internals;
using SimpleServer.Logging;
using SimpleServer.Managers;

namespace SimpleServer
{
    public class SimpleServer
    {
        private static bool _initialized;

        internal List<SimpleServerEngine> _engines;
        private HandlerManager handler;

        public SimpleServer()
        {
            if (!_initialized) Initialize();

            Hosts = new List<SimpleServerHost>();
        }

        public List<SimpleServerHost> Hosts { get; set; }

        public static void Initialize()
        {
            if (_initialized)
                return;
            SimpleServerConfig.Http11Only = false;
            SimpleServerConfig.Http2Subsystem = false;
            SimpleServerConfig.IgnoreSendExceptions = true;
            Log.Writers = new List<TextWriter>();
            _initialized = true;
        }

        public bool HasWildcardHost()
        {
            var result = false;
            Hosts.ForEach(x =>
            {
                if (x.FQDN == "*" || x.AliasFQDNs.Contains("*")) result = true;
            });
            return result;
        }

        internal async Task HandleRequestAsync(SimpleServerRequest request, SimpleServerResponse response)
        {
            await handler.HandleAsync(new SimpleServerContext {Request = request, Response = response});
        }

        public void Start()
        {
            handler = HandlerManager.For(this);
            try
            {
                var ports = new List<int>();
                if (Hosts.Count == 0)
                    throw new InvalidOperationException(
                        "Hosts were empty, please add at least 1 SimpleServerHost before you attempt to start SimpleServer. Error Code: 0x48737430");

                Log.WriteLine("Checking port availability and bind permission...");
                _engines = new List<SimpleServerEngine>();
                Hosts.ForEach(x =>
                {
                    _engines.Add(new SimpleServerEngine(x, this));
                    ports.Add(x.Endpoint.Port);
                });
                foreach (var port in ports)
                {
                    var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
                    try
                    {
                        var tcpListener = new TcpListener(ipAddress, port);
                        tcpListener.Start();
                        tcpListener.Stop();
                    }
                    catch (SocketException ex)
                    {
                        Log.Error("Port " + port + " is not available. Stopping...");
                        return;
                    }
                }

                Log.WriteLine("Ports are bindable, proceeding with server start...");
                // Everybody START YOUR ENGINES!
                _engines.ForEach(x => x.Start());
                Log.WriteLine("SimpleServer is now active.");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void Stop()
        {
            Log.WriteLine("Stopping server...");
            _engines.ForEach(x => x.Stop());
            Log.WriteLine("SimpleServer is no longer active.");
        }
    }
}