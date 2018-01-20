using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerEndpoint
    {
        public IPAddress Scope { get; set; }
        public int Port { get; set; }
    }
}
