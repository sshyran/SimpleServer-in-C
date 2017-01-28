using SimpleServer.Core.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleServer.Core.Platform.Net;

namespace SimpleServer.DotNet
{
    public class DotNetPlatform : Platform
    {
        public WebRequest CreateWebRequest(string url)
        {
            return (WebRequest)new DotNetWebRequest(url);
        }
    }
}
