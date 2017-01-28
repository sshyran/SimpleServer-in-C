using SimpleServer.Core.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleServer.Core.Platform.Net;
using SimpleServer.Core.Platform.Net.Listeners;

namespace SimpleServer.DotNet
{
    public class DotNetPlatform : Platform
    {
        public HttpListener CreateHttpListener(Listener underlyingListener)
        {
            throw new NotImplementedException();
        }

        public Listener CreateListener(int port)
        {
            throw new NotImplementedException();
        }

        public WebRequest CreateWebRequest(string url)
        {
            return (WebRequest)new DotNetWebRequest(url);
        }

        public WebServer CreateWebServer(int port)
        {
            throw new NotImplementedException();
        }
    }
}
