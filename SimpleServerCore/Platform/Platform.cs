using SimpleServer.Core.Platform.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Core.Platform
{
    public interface Platform
    {
        WebRequest CreateWebRequest(string url);

    }
}
