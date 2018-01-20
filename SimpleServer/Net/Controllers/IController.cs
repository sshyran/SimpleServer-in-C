namespace SimpleServer.Net.Controllers
{
    public interface IController
    {
        IPipeline Pipeline { get; }
    }
}