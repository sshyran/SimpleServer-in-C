// HttpAttributes.cs - Ultz.SimpleServer.Minimal
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

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     An abstract <see cref="System.Attribute" /> that, when impemented, is representative of a method and route. Used to
    ///     resolve handlers.
    /// </summary>
    public abstract class HttpAttribute : Attribute
    {
        /// <summary>
        ///     Creates an instance of this <see cref="HttpAttribute" /> at the index route
        /// </summary>
        protected HttpAttribute()
        {
        }

        /// <summary>
        ///     Creates an instance of this <see cref="HttpAttribute" /> at the specified route
        /// </summary>
        /// <param name="route"></param>
        // ReSharper disable once UnusedParameter.Local
        protected HttpAttribute(string route)
        {
        }

        /// <summary>
        ///     Represents the route that the handler should serve.
        /// </summary>
        public abstract string Route { get; set; }

        /// <summary>
        ///     Represents the methods the handler can handle.
        /// </summary>
        public abstract HttpMethod Method { get; }

        /// <summary>
        ///     Formats the specified route so that it is a web-friendly URI.
        /// </summary>
        /// <param name="uri">the route to format</param>
        /// <returns>a formatted route</returns>
        protected string ProcessUri(string uri)
        {
            var route = uri;
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            return route;
        }
    }
    /// <summary>
    ///     Redirects requests from the given URL to the target handler if an attribute able to handle the redirected request exists.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpRedirectFromAttribute : Attribute
    {
        public HttpRedirectFromAttribute(string route)
        {
            Route = route;
        }
        
        public string Route { get; set; }
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the GET method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpGetAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpGetAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpGetAttribute(string route) : base(route)
        {
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Get;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the POST method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpPostAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpPostAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpPostAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Post;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the PUT method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpPutAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpPutAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpPutAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Put;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the PATCH method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpPatchAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpPatchAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpPatchAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Patch;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the DELETE method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpDeleteAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpDeleteAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpDeleteAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Delete;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the OPTIONS method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpOptionsAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpOptionsAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpOptionsAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Options;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the HEAD method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpHeadAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpHeadAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpHeadAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Head;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the TRACE method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpTraceAttribute : HttpAttribute
    {
        private string _route = "";

        /// <inheritdoc />
        public HttpTraceAttribute() : this("")
        {
        }

        /// <inheritdoc />
        public HttpTraceAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route
        {
            get => _route;
            set => _route = ProcessUri(value);
        }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Trace;
    }

    /// <summary>
    ///     An attribute used on handlers that can handle the CONNECT method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HttpConnectAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpConnectAttribute(string route) : base(route)
        {
            // we shouldn"t do anything to the URL, CONNECT requests are pretty specific
            Route = route;
        }

        /// <inheritdoc />
        public override string Route { get; set; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Connect;
    }
}