using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Plugins.Gui
{
    public class Point
    {
        int _x;
        int _y;
        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
    }
}
