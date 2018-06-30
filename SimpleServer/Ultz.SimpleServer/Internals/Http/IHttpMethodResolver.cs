namespace Ultz.SimpleServer.Internals.Http
{
    public interface IHttpMethodResolver
    {
        HttpMethod GetMethod(string method);
    }
}