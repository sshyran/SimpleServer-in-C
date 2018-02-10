using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Exceptions
{
    public class RfcViolationException : Exception
    {
        public RfcViolationException(string msg) : base(msg)
        {

        }
    }
}
