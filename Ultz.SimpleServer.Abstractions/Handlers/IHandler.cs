#region

using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Handlers
{
    public interface IHandler
    {
        bool CanHandle(IRequest request);
        void Handle(IContext context);
    }
}