#region

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Common
{
    public class SimpleServer
    {
        private ILogger _logger;
        private ILoggerProvider _loggerProvider;

        public SimpleServer(ILoggerProvider logger = null)
        {
            _loggerProvider = logger;
            _logger = logger?.CreateLogger("ssmain");
        }

        public Dictionary<string, Service> Services { get; } = new Dictionary<string, Service>();

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}