#region

using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ultz.SimpleServer.Hosts;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace DemoProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server(Http.Create(HttpMode.Legacy),
                new TcpConnectionListener(IPAddress.Loopback, 11111),
                new ConsoleLoggerProvider((n, l) => l >= LogLevel.Information, true));
            server.RequestReceived += ServerOnRequestReceived;
        }

        private static void ServerOnRequestReceived(object sender, ContextEventArgs e)
        {
            var ctx = (HttpContext) e.Context;
        }
    }
}