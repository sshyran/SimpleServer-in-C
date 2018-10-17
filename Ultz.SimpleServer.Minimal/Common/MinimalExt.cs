// MinimalExt.cs - Ultz.SimpleServer.Minimal
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

using Ultz.SimpleServer.Internals.Http;

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    ///     Contains commonly used extension methods
    /// </summary>
    public static class MinimalExt
    {
        /// <summary>
        ///     Trims and/or appends slashes on the given string
        /// </summary>
        /// <param name="s">the given string</param>
        /// <returns>the formatted string</returns>
        public static string ToUrlFormat(this string s)
        {
            if (!s.StartsWith("/"))
                s = "/" + s;
            if (s.EndsWith("/"))
                s = s.Remove(s.Length - 1);
            return s;
        }

        /// <summary>
        /// Redirects the current context to a different location
        /// </summary>
        /// <param name="context">the context to redirect</param>
        /// <param name="url">the target location</param>
        public static void Redirect(this HttpContext context, string url)
        {
            context.Response.StatusCode = 302;
            context.Response.Headers["Location"] = url;
            context.Response.Close();
        }
    }
}