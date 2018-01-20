using System.Threading.Tasks;

namespace SimpleServer.Net.Handlers.Compression
{
    public class GZipCompressor : ICompressor
    {
        public static readonly ICompressor Default = new GZipCompressor();

        public string Name
        {
            get { return "gzip"; }
        }
        public Task<IHttpResponse> Compress(IHttpResponse response)
        {
            return CompressedResponse.CreateGZip(response);
        }
    }
}