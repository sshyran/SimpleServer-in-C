namespace Ultz.SimpleServer.Internals.Http
{
    public sealed class HttpContext : IContext
    {
        public HttpContext(IHttpRequest request, IHttpResponse response, IConnection connection)
        {
            Request = request;
            Response = response;
            Connection = connection;
        }
        public IRequest Request { get; }
        public IResponse Response { get; }
        public IConnection Connection { get; }
    }
}