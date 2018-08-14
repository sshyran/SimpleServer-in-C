#region

using System;
using System.IO;
using System.Net;
using System.Threading;
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
                new ConsoleLoggerProvider((s, level) => true, true));
            server.RequestReceived += ServerOnRequestReceived;
            server.Start();
            Console.ReadLine();
            server.Stop();
        }

        private static void ServerOnRequestReceived(object sender, ContextEventArgs e)
        {
            var ctx = (HttpContext) e.Context;
            foreach (var header in ctx.Request.ToDictionary())
                Console.WriteLine(header.Key + ": "+header.Value);
            ctx.Response.Headers["content-type"] = "text/html";
            var sw = new StreamWriter(ctx.Response.OutputStream);
            sw.WriteLine("<h1>Hello, world!</h1>");
            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }
    }
}