using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Gui
{
    public abstract class Button : Control
    {
        public static Button New()
        {
            return (Button)(SimpleServer.Platform as IGuiPlatform).CreateControl("SimpleServer.Btn");
        }
        public override string TypeString { get { return "SimpleServer.Btn"; } }
    }
}
