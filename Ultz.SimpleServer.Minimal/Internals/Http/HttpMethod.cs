// HttpMethod.cs - Ultz.SimpleServer.Minimal
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System.Text;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP request method
    /// </summary>
    public class HttpMethod : IMethod
    {
        /// <summary>
        ///     Creates an instance representing the given method name, and whether a request can contain a payload when this
        ///     method is used
        /// </summary>
        /// <param name="name">the method name</param>
        /// <param name="payload">if a request using this method can contain a payload</param>
        public HttpMethod(string name, bool payload)
        {
            Id = name;
            ExpectPayload = payload;
        }

        /// <summary>
        ///     Gets an instance representing the GET method
        /// </summary>
        public static HttpMethod Get => new HttpMethod("GET", false);

        /// <summary>
        ///     Gets an instance representing the POST method
        /// </summary>
        public static HttpMethod Post => new HttpMethod("POST", true);

        /// <summary>
        ///     Gets an instance representing the PUT method
        /// </summary>
        public static HttpMethod Put => new HttpMethod("PUT", true);

        /// <summary>
        ///     Gets an instance representing the PATCH method
        /// </summary>
        public static HttpMethod Patch => new HttpMethod("PATCH", true);

        /// <summary>
        ///     Gets an instance representing the DELETE method
        /// </summary>
        public static HttpMethod Delete => new HttpMethod("DELETE", false);

        /// <summary>
        ///     Gets an instance representing the OPTIONS method
        /// </summary>
        public static HttpMethod Options => new HttpMethod("OPTIONS", false);

        /// <summary>
        ///     Gets an instance representing the HEAD method
        /// </summary>
        public static HttpMethod Head => new HttpMethod("HEAD", false);

        /// <summary>
        ///     Gets an instance representing the TRACE method
        /// </summary>
        public static HttpMethod Trace => new HttpMethod("TRACE", false);

        /// <summary>
        ///     Gets an instance representing the CONNECT method
        /// </summary>
        public static HttpMethod Connect => new HttpMethod("CONNECT", false);

        /// <summary>
        ///     The name of this method
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Whether a request using this method can contain a payload
        /// </summary>
        public bool ExpectPayload { get; }

        byte[] IMethod.Id => Encoding.UTF8.GetBytes(Id);

        /// <summary>
        ///     Compares this object to another
        /// </summary>
        /// <param name="other">the other object</param>
        /// <returns></returns>
        protected bool Equals(HttpMethod other)
        {
            return string.Equals(Id, other.Id) && ExpectPayload == other.ExpectPayload;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ ExpectPayload.GetHashCode();
            }
        }

        /// <summary>
        ///     Checks if the two given objects represent the same method
        /// </summary>
        /// <param name="left">object one</param>
        /// <param name="right">object 2</param>
        /// <returns></returns>
        public static bool operator ==(HttpMethod left, HttpMethod right)
        {
            return (object) left != null && left.Equals(right);
        }

        /// <summary>
        ///     Checks if the two given objects don't represent the same method
        /// </summary>
        /// <param name="left">object one</param>
        /// <param name="right">object 2</param>
        /// <returns></returns>
        public static bool operator !=(HttpMethod left, HttpMethod right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is HttpMethod method && method.Id == Id && method.ExpectPayload == ExpectPayload;
        }
    }
}