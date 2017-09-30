using System;

namespace SimpleServer
{
    public class SimpleServer
    {
        public static IPlatform Platform { get; private set; }
        public static void Initialize(IPlatform platform)
        {
            Platform = platform;
        }
    }
}
