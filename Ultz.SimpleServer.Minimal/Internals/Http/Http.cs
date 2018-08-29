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
    public abstract class Http : IProtocol
    {
        public static Dictionary<byte[], HttpMethod> DefaultMethods => new Dictionary<string, HttpMethod>
        {
            {"GET", new HttpMethod("GET", false)},
            {"POST", new HttpMethod("POST", true)},
            {"PUT", new HttpMethod("PUT", true)},
            {"PATCH", new HttpMethod("PATCH", true)},
            {"DELETE", new HttpMethod("DELETE", false)},
            {"OPTIONS", new HttpMethod("OPTIONS", false)},
            {"CONNECT", new HttpMethod("CONNECT", false)},
            {"HEAD", new HttpMethod("HEAD", false)},
            {"TRACE", new HttpMethod("TRACE", false)}
        }.ToDictionary(x => Encoding.UTF8.GetBytes(x.Key), x => x.Value);

        public event EventHandler<ContextEventArgs> ContextCreated;
        public abstract Task HandleConnectionAsync(IConnection connection, ILogger logger);

        public IListener CreateDefaultListener(IPEndPoint endpoint)
        {
            return new TcpConnectionListener(endpoint);
        }

        public IAttributeHandlerResolver AttributeHandlerResolver { get; }= new HttpAttributeResolver();
        public IMethodResolver MethodResolver => new HttpMethodResolver(DefaultMethods);

        public static Http Create(HttpMode mode = HttpMode.Legacy)
        {
            if (mode == HttpMode.Legacy)
                return new HttpOne();
            return new HttpTwo();
        }

        protected void PassContext(HttpContext ctx)
        {
            ContextCreated?.Invoke(this, new ContextEventArgs(ctx));
        }
    }
}