using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerEngine
    {
        TcpListener _listener;
        string[] _hostnames;
        public SimpleServerEngine(SimpleServerHost host)
        {
            _listener = new TcpListener(host.Endpoint.Scope,host.Endpoint.Port);
            var fqdn = new List<string>() { host.FQDN };
            fqdn.AddRange(host.AliasFQDNs);
            _hostnames = fqdn.ToArray();
        }
        public void Start()
        {

        }
    }
}
