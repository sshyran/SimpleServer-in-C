using SimpleServer.Logging;
using System;
using SimpleServer;
using SS = SimpleServer.SimpleServer;
using SimpleServer.Internals;
using System.Net;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting test app...");
            SS.Initialize();
            Log.AddWriter(Console.Out);
            SS server = new SS();
            SimpleServerHost host = new SimpleServerHost();
            host.Handlers.Add(new TestHandler());
            host.Endpoint = new SimpleServerEndpoint() { Port = 11111, Scope = IPAddress.Loopback };
            server.Hosts.Add(host);
            server.Start();
            Console.ReadLine();
        }
    }
}
