#region

using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a response
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Data associated with this response.
        /// </summary>
        Stream OutputStream { get; }
        /// <summary>
        /// Formats this response, then sends it to the underlying connection and closes it. This method can also forcibly terminate the underlying connection without sending the response.
        /// </summary>
        /// <param name="force">true if the underlying connection should be terminated without sending the response, false otherwise</param>
        void Close(bool force = false);
    }
}