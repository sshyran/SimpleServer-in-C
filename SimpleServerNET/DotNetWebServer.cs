using SimpleServer.Core.Platform.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleServer.Core.Platform;
using SimpleServer.Core.Platform.Net.Listeners;

namespace SimpleServer.DotNet
{
    public class DotNetWebServer : WebServer
    {
        private HttpListener listener;
        private List<Function> functions;
        public DotNetWebServer(HttpListener listener)
        {

        }
        public List<Function> GetFunctions()
        {
            throw new NotImplementedException();
        }

        public HttpListener GetUnderlyingListener()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
