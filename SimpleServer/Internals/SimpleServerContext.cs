using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Internals
{
    public class SimpleServerContext
    {
        public SimpleServerRequest Request { get; set; }
        public SimpleServerResponse Response { get; set; }
    }
}
