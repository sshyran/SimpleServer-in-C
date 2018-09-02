using System;

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Provides an <see cref="IContext"/> to a context <see cref="EventHandler"/>
    /// </summary>
    public class ContextEventArgs
    {
        /// <summary>
        /// Creates an instance of <see cref="ContextEventArgs"/> with the given context.
        /// </summary>
        /// <param name="context"></param>
        public ContextEventArgs(IContext context)
        {
            Context = context;
        }

        /// <summary>
        /// The context attached to this instance
        /// </summary>
        public IContext Context { get; }
    }
}