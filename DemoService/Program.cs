using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ultz.SimpleServer.Common;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            // Logging code
            var loggerFactory = new LoggerFactory().AddConsole();
            
            var server = new SimpleServer(new ConsoleLoggerProvider((s, level) => true, true));
            var service = new TestService();
            service.Add(new Connector(IPAddress.Loopback, 8081));
            server.Add(service);
            server.Start();
            Console.ReadLine();
        }
    }
}