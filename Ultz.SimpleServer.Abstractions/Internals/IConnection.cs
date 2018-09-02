#region

using System.IO;
using System.Net;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a network connection
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// The data <see cref="Stream"/> for communicating with this client
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// Returns true if this client is still connected to the server, false otherwise.
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// The local endpoint of this client
        /// </summary>
        EndPoint LocalEndPoint { get; }
        /// <summary>
        /// The remote endpoint of this client
        /// </summary>
        EndPoint RemoteEndPoint { get; }
        /// <summary>
        /// The connection ID (used by the server only) used to identify connections apart from eachother
        /// </summary>
        int Id { get; }
        /// <summary>
        /// Terminates this connection
        /// </summary>
        void Close();
    }
}