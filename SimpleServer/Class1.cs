using System;
using System.Net;

namespace SimpleServer
{
    public class SimpleServer
    {
        public static IPlatform Platform { get; private set; }
        public static void Initialize(IPlatform platform)
        {
            Platform = platform;
        }
        public SimpleServer(IPEndPoint defaultEndPoint)
        {

        }
    }
}
