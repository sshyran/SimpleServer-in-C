using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins.Gui.Controls
{
    public class Tab : Control
    {
        public Tab(string name)
        {
            Name = name;
            Title = "New Tab";
            Controls = new List<Control>();
        }
        public string Title { get; set; }
        public override Point Location { get { return new Point(0,0); } set { } }
        public override string Name { get; set; }
        public override List<Control> Controls { get; set; }

        public override void Update()
        {
        }
    }
}
