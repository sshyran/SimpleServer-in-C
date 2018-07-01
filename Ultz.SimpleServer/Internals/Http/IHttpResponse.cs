using System.Collections.Generic;
using System.IO;

namespace Ultz.SimpleServer.Internals.Http
{
    public interface IHttpResponse : IResponse
    {
        /// <summary>
        ///     Gets the headers of the HTTP response.
        /// </summary>
        Dictionary<string, string> Headers { get; }

        /// <summary>
        ///     Gets the stream containing the content of this response.
        /// </summary>
        Stream OutputStream { get; }

        /// <summary>
        ///     Gets or sets the HTTP version.
        /// </summary>
        string Version { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP reason phrase.
        /// </summary>
        string ReasonPhrase { get; set; }
    }
}