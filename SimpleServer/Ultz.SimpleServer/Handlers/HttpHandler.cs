using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Handlers
{
    public abstract class HttpHandler : IHandler
    {
        public bool CanHandle(IRequest request)
        {
            return CanHandle((IHttpRequest) request);
        }

        public abstract bool CanHandle(IHttpRequest request);
        public abstract bool Handle(HttpContext context);

        public void Handle(IContext context)
        {
            Handle((HttpContext) context);
        }
    }
}