using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Hosts
{
    public abstract class Host : IDisposable
    {
        public List<IDecorator> Decorators { get; set; } = new List<IDecorator>();
        public List<IHandler> Handlers { get; set; } = new List<IHandler>();
        
        public IPEndPoint Endpoint { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Handle(IContext context)
        {
            var handler = Handlers.FirstOrDefault(x => x.CanHandle(context.Request));
            handler?.Handle(context);
        }

        public void Start()
        {
        }

        public void Listen()
        {
            Task.Run(() => {  });
        }
    }
}