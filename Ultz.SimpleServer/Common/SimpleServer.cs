using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Hosts;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Common
{
    public class SimpleServer
    {
        private ILoggerProvider _loggerProvider;
        private ILogger _logger;
        
        public Dictionary<string,Service> Services { get; } = new Dictionary<string, Service>();
        
        public SimpleServer(ILoggerProvider logger = null)
        {
            _loggerProvider = logger;
            _logger = logger?.CreateLogger("ssmain");
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }
}