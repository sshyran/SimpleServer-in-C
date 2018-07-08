using System.Collections.Generic;
using System.Reflection;
using Ultz.SimpleServer.Hosts;

namespace Ultz.SimpleServer.Common
{
    public sealed class SimpleServer
    {
        public static readonly string Version = Assembly.GetAssembly(typeof(SimpleServer)).GetName().Version.ToString();
        public List<Host> Hosts { get; set; }
    }
}