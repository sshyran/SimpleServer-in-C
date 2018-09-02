#region

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public abstract class HttpResponse : IResponse
    {
        /// <summary>
        /// Gets or sets the HTTP status code of this response
        /// </summary>
        public int StatusCode { get; set; } = 200;
        /// <summary>
        /// Gets or sets the reason phrase. Empty by default as its deprecated.
        /// </summary>
        [Obsolete] public string ReasonPhrase { get; set; } = "";
        /// <summary>
        /// Gets a dictionary containing additional HTTP headers to be sent
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        /// <summary>
        /// Gets a writable stream for the response payload.
        /// </summary>
        public Stream OutputStream { get; } = new MemoryStream();

        /// <inheritdoc />
        public abstract void Close(bool force = false);
    }
}