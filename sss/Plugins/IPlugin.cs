using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins
{
    public interface IPlugin
    {
        void Enable();
        void Disable();
    }
}
