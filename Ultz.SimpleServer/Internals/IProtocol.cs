using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Ultz.SimpleServer.Hosts;

namespace Ultz.SimpleServer.Internals
{
    public interface IProtocol
    {
        IContext GetContext(IConnection connection);
        Task<IContext> GetContextAsync(IConnection connection);
        IListener CreateDefaultListener(IPEndPoint endpoint);
        [CanBeNull] IAttributeHandlerResolver AttributeHandlerResolver { get; }
    }
}