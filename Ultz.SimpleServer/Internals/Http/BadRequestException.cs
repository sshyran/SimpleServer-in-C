using System;

namespace Ultz.SimpleServer.Internals.Http
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Bad Request"){}
        public BadRequestException(Exception inner) : base("Bad Request",inner){}
        public BadRequestException(string message) : base(message){}
        public BadRequestException(string message,Exception inner) : base(message,inner){}
    }
}