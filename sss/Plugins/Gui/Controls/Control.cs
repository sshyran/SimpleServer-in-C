using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins.Gui.Controls
{
    public abstract class Control
    {
        public abstract string Name { get; set; }
        public abstract Point Location { get; set; }
        public abstract List<Control> Controls { get; set; }
        public abstract void Update();
    }
}
