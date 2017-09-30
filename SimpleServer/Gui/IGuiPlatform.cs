using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Gui
{
    public interface IGuiPlatform
    {
        string ShowForm(Form form);
        Control CreateControl(string typeString);
    }
}
