#region

using System;
using System.Threading.Tasks;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a network listener that listens for <see cref="IConnection"/>s
    /// </summary>
    public interface IListener : IDisposable
    {
        /// <summary>
        /// True if this listener is actively listening, false otherwise
        /// </summary>
        bool Active { get; }
        /// <summary>
        /// Listens for a new connection, will not return until a client connects.
        /// </summary>
        /// <returns>a new connection</returns>
        IConnection Accept();
        /// <summary>
        /// Asynchronously listens for a new connection, will not return until a client connects.
        /// </summary>
        /// <returns>a new connection</returns>
        Task<IConnection> AcceptAsync();
        /// <summary>
        /// Starts this listener, so it begins listening for new connections.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops this listener, refusing any new connections.
        /// </summary>
        void Stop();
        /// <summary>
        /// Checks that this listener is able to start. When implemented, this method will throw exceptions relating to any problems preventing this listener from starting
        /// </summary>
        void RunChecks();
    }
}