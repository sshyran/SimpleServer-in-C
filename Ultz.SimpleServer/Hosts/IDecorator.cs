using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Hosts
{
    public interface IDecorator
    {
        IListener Decorate(IListener @base);
    }
}