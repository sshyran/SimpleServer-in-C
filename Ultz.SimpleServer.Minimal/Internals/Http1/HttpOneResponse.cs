// HttpOneResponse.cs - Ultz.SimpleServer.Minimal
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace Ultz.SimpleServer.Internals.Http1
{
    /// <inheritdoc />
    public class HttpOneResponse : HttpResponse
    {
        private readonly IConnection _connection;
        private readonly HttpRequest _request;

        /// <inheritdoc />
        public HttpOneResponse(HttpRequest request, IConnection conection)
        {
            _connection = conection;
            _request = request;
        }

        /// <inheritdoc />
        public override void Close(bool force = false)
        {
            if (force) _connection.Close();

            var headers = Headers.Select(x => new KeyValuePair<string, string>(x.Key.ToLower(), x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
            if (headers.TryGetValue("server", out var val))
                headers["server"] = "SimpleServer/1.0 " + val;
            else
                headers["server"] = "SimpleServer/1.0";
            const string crlf = "\r\n";
            var response = "";
#pragma warning disable 618
#pragma warning disable 612
            response += _request.Protocol + " " + StatusCode + " " + ReasonPhrase + crlf;
#pragma warning restore 618
#pragma warning restore 612
            foreach (var header in Headers)
                response += header.Key + ": " + header.Value + crlf;
            response += crlf;
            // write headers
            var bytes = Encoding.UTF8.GetBytes(response);
            _connection.Stream.Write(bytes, 0, bytes.Length);
            // write response
            bytes = ((MemoryStream) OutputStream).ToArray();
            _connection.Stream.Write(bytes, 0, bytes.Length);
            // close the connection
            // TODO: Keep Alive support?
            _connection.Close();
        }
    }
}