using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Handlers
{
    public interface IHandler
    {
        bool CanHandle(IRequest request);
        void Handle(IContext context);
    }
}