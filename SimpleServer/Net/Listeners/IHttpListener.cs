using System.Threading.Tasks;
using SimpleServer.Net.Clients;

namespace SimpleServer.Net.Listeners
{
    public interface IHttpListener
    {

        Task<IClient> GetClient();

    }
}
