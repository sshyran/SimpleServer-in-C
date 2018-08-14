#region

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    public abstract class HttpResponse : IResponse
    {
        public int StatusCode { get; set; } = 200;
        [Obsolete] public string ReasonPhrase { get; set; } = "";
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public Stream OutputStream { get; } = new MemoryStream();
        public abstract void Close(bool force = false);
    }
}