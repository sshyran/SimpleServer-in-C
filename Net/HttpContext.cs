using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Net
{
    public class HttpContext
    {
        TcpClient _client;
        private HttpRequest _request;
    }
}
