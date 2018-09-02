#region

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <inheritdoc />
    public class BadRequestException : Exception
    {
        /// <inheritdoc />
        public BadRequestException() : base("Bad Request")
        {
        }

        /// <inheritdoc />
        public BadRequestException(Exception inner) : base("Bad Request", inner)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}