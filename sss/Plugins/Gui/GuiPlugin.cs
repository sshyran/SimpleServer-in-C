using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins
{
    public abstract class GuiCapablePlugin : IPlugin
    {
        public abstract void Disable();
        public abstract void Enable();

    }
}
