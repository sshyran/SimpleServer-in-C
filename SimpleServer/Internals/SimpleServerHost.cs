using System.Collections.Generic;
using SimpleServer.Decorators;
using SimpleServer.Handlers;

namespace SimpleServer.Internals
{
    public class SimpleServerHost
    {
        public SimpleServerHost()
        {
            Decorators = new List<IDecorator>();
            Handlers = new List<IHandler>();
            FQDN = "*";
            AliasFQDNs = new List<string>();
            Endpoint = new SimpleServerEndpoint();
        }

        public List<IDecorator> Decorators { get; set; }
        public List<IHandler> Handlers { get; set; }
        public string FQDN { get; set; }
        public List<string> AliasFQDNs { get; set; }
        public SimpleServerEndpoint Endpoint { get; set; }
    }
}