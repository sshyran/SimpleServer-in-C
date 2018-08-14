#region

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http2.Http2
{
    /// <summary>
    ///     Contains constant values for HTTP/2
    /// </summary>
    internal static class Constants
    {
        /// <summary>The initial flow control window for connections</summary>
        public const int InitialConnectionWindowSize = 65535;

        /// <summary>An empty array segment</summery>
        public static readonly ArraySegment<byte> EmptyByteArray =
            new ArraySegment<byte>(new byte[0]);
    }
}