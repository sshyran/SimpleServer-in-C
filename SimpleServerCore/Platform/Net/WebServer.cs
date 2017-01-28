using SimpleServer.Core.Platform.Net.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleServer.Core.Platform.Net.Http;

namespace SimpleServer.Core.Platform.Net
{
    public interface WebServer
    {
        HttpListener GetUnderlyingListener();
        List<Function> GetFunctions();
        void Start();
        void Stop();
        void Pause();
    }
}
