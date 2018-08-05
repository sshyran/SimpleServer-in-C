using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Hosts;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Common
{
    public class SimpleServer<T> where T:IProtocol
    {
        private ILoggerProvider _loggerProvider;
        private ILogger _logger;
        public SimpleServer(ILoggerProvider logger = null)
        {
            _loggerProvider = logger;
            _logger = logger.CreateLogger("ssmain");
        }
        
        public List<Host<T>> Hosts { get; set; }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }

    public sealed class SimpleServer : SimpleServer<Http>
    {
        public static readonly string Version = Assembly.GetAssembly(typeof(SimpleServer)).GetName().Version.ToString();
    }
}