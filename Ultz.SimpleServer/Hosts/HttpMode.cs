using System;

namespace Ultz.SimpleServer.Hosts
{
    public enum HttpMode
    {
        /// <summary>
        ///     Processes HTTP/1 requests only
        /// </summary>
        Legacy,

        /// <summary>
        ///     Processes HTTP/1 requests and HTTP/2 requests. This mode also upgrades HTTP/1 requests to HTTP/2 requests if
        ///     requested by the client.
        /// </summary>
        Dual
    }
}