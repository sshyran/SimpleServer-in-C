using SimpleServer.Handlers;
using SimpleServer.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Managers
{
    public class HandlerManager
    {
        private HandlerManager() { handlers = new List<IHandler>(); }
        private List<IHandler> handlers = new List<IHandler>();
        private static Dictionary<SimpleServer, HandlerManager> submanagers = new Dictionary<SimpleServer, HandlerManager>();
        public static HandlerManager For(SimpleServer server)
        {
            if (!submanagers.ContainsKey(server))
            {
                submanagers.Add(server, new HandlerManager());
            }
            return submanagers[server];
        }
        public HandlerManager With(SimpleServerHost host, params IHandler[] handlers)
        {
            foreach (IHandler handler in handlers)
            {
                Add(host, handler);
            }
            return this;
        }
        public void Add(SimpleServerHost host, IHandler handler)
        {
            handlers.Add(handler);
        }
        public async Task HandleAsync(SimpleServerContext ctx)
        {
            await ctx.Request.Connection.GetListener().GetEngine().GetHost().Handlers.ForEachAsync(async x => { if (x.CanHandle(ctx.Request)) { await Task.Factory.StartNew(() => { x.Handle(ctx); }); } });
        }
    }
}
