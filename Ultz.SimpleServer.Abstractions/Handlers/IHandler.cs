#region

using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Handlers
{
    /// <summary>
    /// Represents a handler that reads <see cref="IRequest"/>s and responds to them, if the handler <see cref="CanHandle"/> it.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Causes the handler to check this request to see if it can handle it.
        /// </summary>
        /// <param name="request">the request in question</param>
        /// <returns>true if this handler is able to handle the request, false otherwise.</returns>
        bool CanHandle(IRequest request);
        /// <summary>
        /// Handles an <see cref="IContext"/>
        /// </summary>
        /// <param name="context">the context to handle</param>
        void Handle(IContext context);
    }
}