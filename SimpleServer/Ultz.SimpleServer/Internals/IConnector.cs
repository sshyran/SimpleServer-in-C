using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public interface IConnector
    {
        IConnection Accept();
        Task<IConnection> AcceptAsync();
    }
}