using SimpleServer.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerHost
    {
        public SimpleServerHost()
        {
            Decorators = new List<SimpleServerDecorator>();
            Handlers = new List<IHandler>();
            FQDN = "*";
            AliasFQDNs = new List<string>();
            Endpoint = new SimpleServerEndpoint();
        }
        public List<SimpleServerDecorator> Decorators { get; set; }
        public List<IHandler> Handlers { get; set; }
        public string FQDN { get; set; }
        public List<string> AliasFQDNs { get; set; }
        public SimpleServerEndpoint Endpoint { get; set; }
    }
}
