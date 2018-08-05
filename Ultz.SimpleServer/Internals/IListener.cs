using System;
using System.Net;
using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public interface IListener : IDisposable
    {
        IConnection Accept();
        Task<IConnection> AcceptAsync();
        void Start();
        void Stop();
        void RunChecks();
        bool Active { get; }
    }
}