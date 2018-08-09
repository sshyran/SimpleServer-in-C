using System;
using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Handlers
{
    public class LamdaHandler : IHandler
    {
        private Func<IRequest, bool> _canHandle;
        private Action<IContext> _handle;
        public LamdaHandler(Func<IRequest, bool> canHandleCallback, Action<IContext> handler)
        {
            _canHandle = canHandleCallback;
            _handle = handler;
        }
        public bool CanHandle(IRequest request)
        {
            return _canHandle(request);
        }

        public void Handle(IContext context)
        {
            _handle(context);
        }
    }
}