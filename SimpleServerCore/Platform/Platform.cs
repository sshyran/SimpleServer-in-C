using SimpleServer.Core.Platform.Net;
using SimpleServer.Core.Platform.Net.Listeners;

namespace SimpleServer.Core.Platform
{
    public interface Platform
    {
        /// <summary>
        /// Creates a http request to the specified url
        /// </summary>
        /// <param name="url">the target url</param>
        /// <returns>a WebRequest ready to send</returns>
        WebRequest CreateWebRequest(string url);
        /// <summary>
        /// Creates a standard TCP listener tied to the specified port
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <returns>a stopped Listener ready to start</returns>
        Listener CreateListener(int port);
        /// <summary>
        /// Creates a HttpListener from a TCP listener
        /// </summary>
        /// <param name="underlyingListener">the underlying TCP listener</param>
        /// <returns>a stopped HttpListener ready to start</returns>
        HttpListener CreateHttpListener(Listener underlyingListener);
        /// <summary>
        /// Creates a WebServer that serves functions on the specified port
        /// </summary>
        /// <param name="port">the target port</param>
        /// <returns>a stopped WebServer ready to start</returns>
        WebServer CreateWebServer(int port);
    }
}
