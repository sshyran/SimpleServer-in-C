using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Hosts;

namespace Ultz.SimpleServer.Internals
{
    public interface IProtocol
    {
        event EventHandler<ContextEventArgs> ContextCreated;
        Task HandleConnectionAsync(IConnection connection, ILogger logger);
        IListener CreateDefaultListener(IPEndPoint endpoint);
        [CanBeNull] IAttributeHandlerResolver AttributeHandlerResolver { get; }
        IMethodResolver MethodResolver { get; }
    }
}