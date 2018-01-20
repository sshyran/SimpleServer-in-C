namespace SimpleServer.Net
{
    public interface IHttpMethodProvider
    {
        HttpMethods Provide(string name);
    }
}