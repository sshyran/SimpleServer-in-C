using System.Threading.Tasks;

namespace SimpleServer.Net.Handlers
{
    public interface IResponseProvider
    {

        Task<IHttpResponse> Provide(object value, HttpResponseCode responseCode = HttpResponseCode.Ok);

    }
}