using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public interface IServer
    {
        IContext GetContext(IConnection connection);
        Task<IContext> GetContextAsync(IConnection connection);
    }
}