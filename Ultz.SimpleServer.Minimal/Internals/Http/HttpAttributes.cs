using System;

namespace Ultz.SimpleServer.Internals.Http
{
    public abstract class HttpAttribute : Attribute
    {
        protected HttpAttribute()
        {
        }

        protected HttpAttribute(string route)
        {
        }

        public abstract string Route { get; }
        public abstract HttpMethod Method { get; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute() : this(""){}
        public HttpGetAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Get;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute() : this(""){}
        public HttpPostAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Post;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : HttpAttribute
    {
        public HttpPutAttribute() : this(""){}
        public HttpPutAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Put;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPatchAttribute : HttpAttribute
    {
        public HttpPatchAttribute() : this(""){}
        public HttpPatchAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Patch;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : HttpAttribute
    {
        public HttpDeleteAttribute() : this(""){}
        public HttpDeleteAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Delete;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpOptionsAttribute : HttpAttribute
    {
        public HttpOptionsAttribute() : this(""){}
        public HttpOptionsAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Options;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpHeadAttribute : HttpAttribute
    {
        public HttpHeadAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Head;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpTraceAttribute : HttpAttribute
    {
        public HttpTraceAttribute() : this(""){}
        public HttpTraceAttribute(string route) : base(route)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;
            if (route.EndsWith("/"))
                route = route.Remove(route.Length - 1);
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Trace;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpConnectAttribute : HttpAttribute
    {
        public HttpConnectAttribute(string route) : base(route)
        {
            // we shouldn"t do anything to the URL, CONNECT requests are pretty specific
            Route = route;
        }

        public override string Route { get; }
        public override HttpMethod Method => HttpMethod.Connect;
    }
}