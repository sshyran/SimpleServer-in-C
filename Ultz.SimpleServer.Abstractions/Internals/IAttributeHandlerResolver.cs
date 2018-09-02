#region

using System.Collections.Generic;
using Ultz.SimpleServer.Handlers;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Provides methods to search an object for handlers
    /// </summary>
    public interface IAttributeHandlerResolver
    {
        /// <summary>
        /// Searches the given object to resolve methods able to handle requests from the <see cref="IProtocol"/> this <see cref="IAttributeHandlerResolver"/> is a member of.
        /// </summary>
        /// <param name="obj">the object to search</param>
        /// <returns>an enumerable containing handlers found in the given object</returns>
        IEnumerable<IHandler> GetHandlers(object obj);
    }
}