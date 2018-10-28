// HttpTwoResponse.cs - Ultz.SimpleServer.Minimal
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
using Http2;
using Http2.Hpack;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    /// <inheritdoc />
    public class HttpTwoResponse : HttpResponse
    {
        /// <inheritdoc />
        public HttpTwoResponse(IStream stream)
        {
            Stream = stream;
        }

        private IStream Stream { get; }

        /// <inheritdoc />
        public override void Close(CloseMode mode = CloseMode.Graceful)
        {
            foreach (var cookie in Cookies)
                Headers.Add("set-cookie",cookie.ToString());
            if (mode == CloseMode.Force)
                Stream.CloseAsync().GetAwaiter().GetResult();
            var body = ((MemoryStream) OutputStream).ToArray();
            Stream.WriteHeadersAsync(
                new List<HeaderField> {new HeaderField {Name = ":status", Value = StatusCode.ToString()}},
                body.Length == 0).GetAwaiter().GetResult();
            if (body.Length != 0)
                Stream.WriteAsync(new ArraySegment<byte>(body), mode != CloseMode.KeepAlive).GetAwaiter().GetResult();
            // the stream is already closed, no need to do anything else.
        }
    }
}