using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Bad request.") { }
        public BadRequestException(string msg) : base(msg) { }
    }
}
