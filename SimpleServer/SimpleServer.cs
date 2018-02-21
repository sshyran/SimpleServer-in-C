using SimpleServer.Handlers;
using SimpleServer.Internals;
using SimpleServer.Logging;
using SimpleServer.Managers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class SimpleServer
    {
        public SimpleServer()
        {
            if (!_initialized)
            {
                Initialize();
            }
            Hosts = new List<SimpleServerHost>();
        }
        private static bool _initialized = false;
        public static void Initialize()
        {
            if (_initialized)
                return;
            SimpleServerConfig.Http11Only = false;
            SimpleServerConfig.Http2Subsystem = false;
            SimpleServerConfig.IgnoreSendExceptions = true;
            Log.Writers = new List<System.IO.TextWriter>();
            _initialized = true;
        }
        List<SimpleServerEngine> _engines;
        HandlerManager handler;
        public List<SimpleServerHost> Hosts { get; set; }
        public bool HasWildcardHost()
        {
            bool result = false;
            Hosts.ForEach(x => { if (x.FQDN == "*" || x.AliasFQDNs.Contains("*")) { result = true; } });
            return result;
        }
        internal async Task HandleRequestAsync(SimpleServerRequest request,SimpleServerResponse response)
        {
            await handler.HandleAsync(new SimpleServerContext() { Request = request, Response = response });
        }
        public void Start()
        {
            handler = HandlerManager.For(this);
            try
            {
                List<int> ports = new List<int>();
                if (Hosts.Count == 0)
                {
                    throw new InvalidOperationException("Hosts were empty, please add at least 1 SimpleServerHost before you attempt to start SimpleServer. Error Code: 0x48737430");
                }
                Log.WriteLine("Checking port availability and bind permission...");
                _engines = new List<SimpleServerEngine>();
                Hosts.ForEach(x => { _engines.Add(new SimpleServerEngine(x, this)); ports.Add(x.Endpoint.Port); });
                foreach (int port in ports)
                {
                    IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
                    try
                    {
                        TcpListener tcpListener = new TcpListener(ipAddress, port);
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
