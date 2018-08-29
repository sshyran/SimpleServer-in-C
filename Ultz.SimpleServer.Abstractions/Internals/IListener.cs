#region

using System;
using System.Threading.Tasks;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public interface IListener : IDisposable
    {
        bool Active { get; }
        IConnection Accept();
        Task<IConnection> AcceptAsync();
        void Start();
        void Stop();
        void RunChecks();
    }
}