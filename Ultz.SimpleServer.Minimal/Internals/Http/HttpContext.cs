// HttpContext.cs - Ultz.SimpleServer.Minimal
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

using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP request/response context
    /// </summary>
    public class HttpContext : IContext
    {
        /// <summary>
        ///     Creates an instance with the given properties
        /// </summary>
        /// <param name="req">the request</param>
        /// <param name="res">the response</param>
        /// <param name="connection">the underlying connection</param>
        /// <param name="logger">the logger assigned to this context</param>
        public HttpContext(HttpRequest req, HttpResponse res, IConnection connection, ILogger logger)
        {
            Request = req;
            Response = res;
            Connection = connection;
            Logger = logger;
        }

        /// <summary>
        ///     The HTTP request
        /// </summary>
        public HttpRequest Request { get; }

        /// <summary>
        ///     The HTTP response
        /// </summary>
        public HttpResponse Response { get; }

        IRequest IContext.Request => Request;
        IResponse IContext.Response => Response;

        /// <inheritdoc />
        public IConnection Connection { get; }

        /// <inheritdoc />
        public ILogger Logger { get; }
    }
}