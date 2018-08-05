using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Handlers
{
    public abstract class HttpHandler : IHandler
    {
        public bool CanHandle(IContext request)
        {
            return CanHandle((HttpContext) request);
        }

        public abstract bool CanHandle(HttpContext request);
        public abstract bool Handle(HttpContext context);

        public void Handle(IContext context)
        {
            Handle((HttpContext) context);
        }
    }
}