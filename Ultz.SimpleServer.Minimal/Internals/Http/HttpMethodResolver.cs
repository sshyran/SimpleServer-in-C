#region

using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <inheritdoc />
    public class HttpMethodResolver : MethodResolver<HttpMethod>
    {
        /// <inheritdoc />
        public HttpMethodResolver(Dictionary<byte[], HttpMethod> dictionary) : base(dictionary)
        {
        }
    }
}