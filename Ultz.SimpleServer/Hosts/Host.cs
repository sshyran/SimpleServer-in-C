using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Hosts
{
    public class Host<T> : IDisposable where T : IProtocol
    {
        public List<IDecorator> Decorators { get; set; } = new List<IDecorator>();
        public List<IHandler> Handlers { get; set; } = new List<IHandler>();

        public IPEndPoint Endpoint { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private Server<T> _server;
        private T _protocol;
        public ILoggerProvider LoggerProvider { get; set; }

        public void Start()
        {
            _protocol = Activator.CreateInstance<T>();
            _server = new Server<T>(_protocol,
                Decorators.Aggregate(_protocol.CreateDefaultListener(Endpoint),
                    (current, decorator) => decorator.Decorate(current)), LoggerProvider);
            _server.RequestReceived += Handle;
            _server.Start();
        }

        private void Handle(object sender, ContextEventArgs e)
        {
            var handler = Handlers.FirstOrDefault(x => x.CanHandle(e.Context));
            handler?.Handle(e.Context);
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}