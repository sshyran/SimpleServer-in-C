using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins.Gui
{
    public abstract class EmptyGuiPlatform : IPlatform
    {
        public bool IsEmpty()
        {
            return true;
        }
    }
}
