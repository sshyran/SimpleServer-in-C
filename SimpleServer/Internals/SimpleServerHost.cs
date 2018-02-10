using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerHost
    {
        public List<SimpleServerDecorator> Decorators { get; set; }
        public List<SimpleServerHandler> Handlers { get; set; }
        public string FQDN { get; set; }
        public List<string> AliasFQDNs { get; set; }
        public SimpleServerEndpoint Endpoint { get; set; }
    }
}
