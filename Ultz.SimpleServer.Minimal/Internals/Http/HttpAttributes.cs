using System;

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    /// An abstract <see cref="System.Attribute"/> that, when impemented, is representative of a method and route. Used to resolve handlers.
    /// </summary>
    public abstract class HttpAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of this <see cref="HttpAttribute"/> at the index route 
        /// </summary>
        protected HttpAttribute()
        {
        }

        /// <summary>
        /// Creates an instance of this <see cref="HttpAttribute"/> at the specified route 
        /// </summary>
        /// <param name="route"></param>
        protected HttpAttribute(string route)
        {
        }

        /// <summary>
        /// Represents the route that the handler should serve.
        /// </summary>
        public abstract string Route { get; }
        /// <summary>
        /// Represents the methods the handler can handle.
        /// </summary>
        public abstract HttpMethod Method { get; }
    }
    /// <summary>
    /// An attribute used on handlers that can handle the GET method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpGetAttribute() : this(""){}

        /// <inheritdoc />
        public HttpGetAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        /// <inheritdoc />
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Get;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the POST method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpPostAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Post;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the PUT method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpPutAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Put;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the PATCH method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPatchAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpPatchAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Patch;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the DELETE method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpDeleteAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Delete;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the OPTIONS method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOptionsAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpOptionsAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Options;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the HEAD method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpHeadAttribute : HttpAttribute
    {
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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Head;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the TRACE method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpTraceAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpTraceAttribute() : this(""){}

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
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Trace;
    }

    /// <summary>
    /// An attribute used on handlers that can handle the CONNECT method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpConnectAttribute : HttpAttribute
    {
        /// <inheritdoc />
        public HttpConnectAttribute(string route) : base(route)
        {
            // we shouldn"t do anything to the URL, CONNECT requests are pretty specific
            Route = route;
        }

        /// <inheritdoc />
        public override string Route { get; }

        /// <inheritdoc />
        public override HttpMethod Method => HttpMethod.Connect;
    }
}