#region

using System;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public interface IProtocol
    {
        [CanBeNull] IAttributeHandlerResolver AttributeHandlerResolver { get; }
        IMethodResolver MethodResolver { get; }
        event EventHandler<ContextEventArgs> ContextCreated;
        Task HandleConnectionAsync(IConnection connection, ILogger logger);
        IListener CreateDefaultListener(IPEndPoint endpoint);
    }
}