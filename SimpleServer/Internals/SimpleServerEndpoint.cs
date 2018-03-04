using System.Net;

namespace SimpleServer.Internals
{
    public class SimpleServerEndpoint
    {
        public IPAddress Scope { get; set; }
        public int Port { get; set; }
    }
}