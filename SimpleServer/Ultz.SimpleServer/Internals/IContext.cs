namespace Ultz.SimpleServer.Internals
{
    public interface IContext
    {
        IRequest Request { get; }
        IResponse Response { get; }
        IConnection Connection { get; }
    }
}