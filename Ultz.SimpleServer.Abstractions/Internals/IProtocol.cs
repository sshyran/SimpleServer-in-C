#region

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Handlers;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a protocol implementation, with classes for resolving handlers and methods, parsing & passing requests to context listeners, and creates default network listeners.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Provides an <see cref="IAttributeHandlerResolver"/> used to find <see cref="IHandler"/>s in a given object.
        /// </summary>
        IAttributeHandlerResolver AttributeHandlerResolver { get; }
        /// <summary>
        /// Provides an <see cref="IMethodResolver"/> used to find methods by their IDs.
        /// </summary>
        IMethodResolver MethodResolver { get; }
        /// <summary>
        /// An event called when a context has been successfully created from <see cref="HandleConnectionAsync"/>
        /// </summary>
        event EventHandler<ContextEventArgs> ContextCreated;
        /// <summary>
        /// Parses data received on an <see cref="IConnection"/>, and passes <see cref="IContext"/>s to <see cref="ContextCreated"/> as they're received. 
        /// </summary>
        /// <param name="connection">the connection to parse data from</param>
        /// <param name="logger">a logger for this request</param>
        /// <returns>an awaitable task</returns>
        Task HandleConnectionAsync(IConnection connection, ILogger logger);
        /// <summary>
        /// Returns a listner class (recommended by the protocol implementation) listening at the given <see cref="IPEndPoint"/>
        /// </summary>
        /// <param name="endpoint">the <see cref="IPEndPoint"/> that the listener should bind to.</param>
        /// <returns>a listener</returns>
        IListener CreateDefaultListener(IPEndPoint endpoint);
    }
}