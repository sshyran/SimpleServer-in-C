using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins.Gui
{
    public interface IPlatform
    {
        /// <summary>
        /// Checks if there is an instance behind this platform
        /// </summary>
        /// <returns>true if this is platform is a dummy (null) instance, false otherwise</returns>
        bool IsEmpty();
    }
}
