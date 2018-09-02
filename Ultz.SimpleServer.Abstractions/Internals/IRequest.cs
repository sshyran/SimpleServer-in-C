#region

using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a network request
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// The method attached to this request
        /// </summary>
        IMethod Method { get; }
        /// <summary>
        /// A <see cref="Stream"/> containing any additional data that was sent
        /// </summary>
        Stream InputStream { get; }
    }
}