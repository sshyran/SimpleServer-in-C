using SimpleServer.Internals;
using SimpleServer.Logging;
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
            Hosts = new List<SimpleServerHost>();
        }
        List<SimpleServerEngine> _engines;
        public List<SimpleServerHost> Hosts { get; set; }
        public bool HasWildcardHost()
        {
            bool result = false;
            Hosts.ForEach(x => { if (x.FQDN == "*" || x.AliasFQDNs.Contains("*")) { result = true; } });
            return result;
        }
        internal async Task HandleRequestAsync(SimpleServerRequest request,SimpleServerResponse response)
        {

        }
        public void Start()
        {
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
    }
}
