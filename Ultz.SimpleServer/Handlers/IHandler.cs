using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Handlers
{
    public interface IHandler
    {
        bool CanHandle(IContext context);
        void Handle(IContext context);
    }
    public interface IHandler<in TContext> where TContext:IContext
    {
        bool CanHandle(TContext context);
        void Handle(TContext context);
    }
}