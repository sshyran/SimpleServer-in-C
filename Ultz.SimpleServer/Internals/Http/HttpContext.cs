#region

using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpContext : IContext
    {
        public HttpContext(HttpRequest req, HttpResponse res, IConnection connection, ILogger logger)
        {
            Request = req;
            Response = res;
            Connection = connection;
            Logger = logger;
        }

        public HttpRequest Request { get; }
        public HttpResponse Response { get; }
        IRequest IContext.Request => Request;
        IResponse IContext.Response => Response;
        public IConnection Connection { get; }
        public ILogger Logger { get; }
    }
}