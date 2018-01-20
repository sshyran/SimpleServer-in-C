using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SimpleServer.Net.RequestProviders
{
    public interface IHttpRequestProvider
    {
        /// <summary>
        /// Provides an <see cref="IHttpRequest"/> based on the context of the stream,
        /// May return null / throw exceptions on invalid requests.
        /// </summary>
        /// <param name="streamReader"></param>
        /// <returns></returns>
        Task<IHttpRequest> Provide(IStreamReader streamReader);

    }
}