using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Functions
{
    public abstract class Function
    {
        public abstract string Name { get; set; }
        public abstract string Path { get; set; }
        public abstract string Type { get; }
    }
}
