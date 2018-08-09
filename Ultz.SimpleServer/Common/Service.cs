using System;
using System.Collections.Generic;
using System.Linq;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Common
{
    public abstract class Service : IConfigurable
    {
        public abstract IProtocol Protocol { get; }
        public List<Valve> Valves { get; set; }
        public List<IHandler> Handlers { get; set; }
        public List<Connector> Connectors { get; set; }

        public abstract void BeforeStart();
        public abstract void OnStart();
        public abstract void AfterStart();
        public abstract void OnStop();
        public abstract void BeforeStop();
        public abstract void AfterStop();

        public void RegisterHandlers(object handlerObject)
        {
            Handlers.AddRange(Protocol?.AttributeHandlerResolver?.GetHandlers(handlerObject.GetType()));
        }

        public void RegsiterHandlers(params IHandler[] handlers)
        {
            Handlers.AddRange(handlers);
        }

        public void RegisterHandler(IHandler handler)
        {
            Handlers.Add(handler);
        }

        public void RegisterHandler(Func<IRequest,bool> canHandleCallback, Action<IContext> handler)
        {
            Handlers.Add(new LamdaHandler(canHandleCallback,handler));
        }

        internal IEnumerable<Server> GetServers()
        {
            return Connectors.Select(x => new Server(Protocol,x.GetListener()));
        }
    }
}