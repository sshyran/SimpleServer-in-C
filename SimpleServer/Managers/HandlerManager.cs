using SimpleServer.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Managers
{
    public class HandlerManager
    {
        private HandlerManager() { handlers = new List<IHandler>(); }
        private List<IHandler> handlers;
        private static Dictionary<SimpleServer,HandlerManager> submanagers;
        public static HandlerManager For(SimpleServer server)
        {
            return submanagers[server];
        }
        public HandlerManager With(IHandler handler)
        {
            Add(handler);
            return this;
        }
        public void Add(IHandler handler)
        {
            handlers.Add(handler);
        }
    }
}
