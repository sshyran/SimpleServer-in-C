#region

using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a <see cref="IRequest"/>/<see cref="IResponse"/> protocol communication context.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// The <see cref="IRequest"/> parsed by the underlying protocol
        /// </summary>
        IRequest Request { get; }
        /// <summary>
        /// A response class that will be formatted and pushed to the <see cref="Connection"/> upon closure.
        /// </summary>
        IResponse Response { get; }
        /// <summary>
        /// The underlying connection.
        /// </summary>
        IConnection Connection { get; }
        /// <summary>
        /// The logger associated with this context.
        /// </summary>
        ILogger Logger { get; }
    }
}