// HttpResponse.cs - Ultz.SimpleServer.Minimal
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.IO;
using Http2.Hpack;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP response
    /// </summary>
    public abstract class HttpResponse : IResponse
    {
        /// <summary>
        ///     Gets or sets the HTTP status code of this response
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        ///     Gets or sets the reason phrase. Empty by default as its deprecated.
        /// </summary>
        [Obsolete]
        public string ReasonPhrase { get; set; } = "";

        /// <summary>
        ///     Gets a dictionary containing additional HTTP headers to be sent
        /// </summary>
        public HttpHeaderCollection Headers { get; } = new HttpHeaderCollection(new List<HeaderField>());

        /// <summary>
        /// Gets a collection containing additional HTTP cookies to be sent
        /// </summary>
        public HttpCookieCollection Cookies { get; } = new HttpCookieCollection();
        
        /// <summary>
        ///     Gets a writable stream for the response payload.
        /// </summary>
        public Stream OutputStream { get; } = new MemoryStream();

        /// <inheritdoc />
        public abstract void Close(CloseMode mode = CloseMode.Graceful);

        /// <summary>
        ///     Formats this response, then sends it to the underlying connection and closes it. This method can also forcibly
        ///     terminate the underlying connection without sending the response.
        /// </summary>
        /// <param name="force">true if the stream should be closed without sending the response, false otherwise</param>
        public void Close(bool force)
        {
            Close(force ? CloseMode.Force : CloseMode.Graceful);
        }
    }
}