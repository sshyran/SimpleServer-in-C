using SimpleServer.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Windows.Client.Gui
{
    public class WinGuiForm : Form
    {
        internal string id;

        public override string Id => id;

        public override string TypeString => "Form";
    }
}
