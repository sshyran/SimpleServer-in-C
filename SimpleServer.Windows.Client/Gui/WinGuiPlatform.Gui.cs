using SimpleServer.Gui;
using SimpleServer.Windows.Client.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.Windows.Client.Platform
{
    partial class WinGuiPlatform
    {
        internal Dictionary<string, Form> forms = new Dictionary<string, Form>();
        public Control CreateControl(string typeString)
        {
            if (typeString == "Form")
            {
                return new WinGuiForm();
            }
            return null;
        }
        

        public bool IsGuiCapable()
        {
            return true;
        }

        public string ShowForm(Form form)
        {
            forms.
        }
    }
}
