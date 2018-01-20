using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleServer.Net.Handlers;

namespace SimpleServer.Net.Controllers
{
    public interface IErrorContainer
    {

        void Log(string description);

        IEnumerable<string> Errors { get; }

        bool Any { get; }

        Task<IControllerResponse> GetResponse();

    }
}