namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpContext : IContext
    {
        public HttpRequest Request { get; }
        public HttpResponse Response { get; }
        IRequest IContext.Request => Request;
        IResponse IContext.Response => Response;
        public IConnection Connection { get; }
    }
}