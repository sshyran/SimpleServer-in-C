using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public interface IListener
    {
        IConnection Accept();
        Task<IConnection> AcceptAsync();
    }
}