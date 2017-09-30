using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Gui
{
    public abstract class Control
    {
        /// <summary>
        /// A client-generated identifier for this control.
        /// </summary>
        public abstract string Id { get; }
        /// <summary>
        /// A string that represents this type of control.
        /// </summary>
        public abstract string TypeString { get; }
        /// <summary>
        /// Controls contained within this control.
        /// </summary>
        public IList<Control> Controls { get; }
    }
}
