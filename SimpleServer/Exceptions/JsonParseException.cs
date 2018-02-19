using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Exceptions
{
    public class JsonParseException : Exception
    {
        public JsonParseException(string message) : base(message) { }
    }
}
