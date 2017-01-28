using SimpleServer.Core.Platform.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Core.Platform.Net.Listeners
{
    public interface HttpListener : Listener
    {
        void ContextRecieved(HttpContext context);
    }
}
