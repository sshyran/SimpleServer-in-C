// Http.cs - Ultz.SimpleServer.Minimal
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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Internals.Http1;
using Ultz.SimpleServer.Internals.Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP protocol implementation
    /// </summary>
    public abstract class Http : IProtocol
    {
        /// <summary>
        ///     Gets the dictionary used by the <see cref="HttpMethodResolver" />
        /// </summary>
        public static Dictionary<byte[], HttpMethod> Methods => new Dictionary<string, HttpMethod>
        {
            {"GET", HttpMethod.Get},
            {"POST", HttpMethod.Post},
            {"PUT", HttpMethod.Put},
            {"PATCH", HttpMethod.Patch},
            {"DELETE", HttpMethod.Delete},
            {"OPTIONS", HttpMethod.Options},
            {"CONNECT", HttpMethod.Connect},
            {"HEAD", HttpMethod.Head},
            {"TRACE", HttpMethod.Trace}
        }.ToDictionary(x => Encoding.UTF8.GetBytes(x.Key), x => x.Value);

        /// <inheritdoc />
        public event EventHandler<ContextEventArgs> ContextCreated;

        /// <inheritdoc />
        public abstract Task HandleConnectionAsync(IConnection connection, ILogger logger);

        /// <summary>
        ///     Creates a <see cref="TcpConnectionListener" /> at the given endpoint.
        /// </summary>
        /// <param name="endpoint">the endpoint to listen at</param>
        /// <returns>a <see cref="TcpConnectionListener" /></returns>
        public IListener CreateDefaultListener(IPEndPoint endpoint)
        {
            return new TcpConnectionListener(endpoint);
        }

        /// <inheritdoc />
        public IAttributeHandlerResolver AttributeHandlerResolver { get; } = new HttpAttributeResolver();

        /// <summary>
        ///     Gets a new <see cref="HttpMethodResolver" /> instance
        /// </summary>
        public IMethodResolver MethodResolver => new HttpMethodResolver(Methods);

        /// <summary>
        ///     Creates an instance of the HTTP implementation represented by the given <see cref="HttpMode" />
        /// </summary>
        /// <param name="mode">the HTTP implementation to get</param>
        /// <returns>a HTTP implementation</returns>
        public static Http Create(HttpMode mode = HttpMode.Dual)
        {
            if (mode == HttpMode.Legacy)
                return new HttpOne();
            return new HttpTwo();
        }

        /// <summary>
        ///     Passes a <see cref="HttpContext" /> to the <see cref="ContextCreated" /> event
        /// </summary>
        /// <param name="ctx">the context</param>
        protected void PassContext(HttpContext ctx)
        {
            ContextCreated?.Invoke(this, new ContextEventArgs(ctx));
        }
    }
}