// HttpMode.cs - Ultz.SimpleServer.Minimal
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

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a HTTP server mode
    /// </summary>
    public enum HttpMode
    {
        /// <summary>
        ///     Processes HTTP/1 requests only
        /// </summary>
        Legacy,

        /// <summary>
        ///     Processes HTTP/1 requests and HTTP/2 requests. This mode also upgrades HTTP/1 requests to HTTP/2 requests if
        ///     requested by the client.
        /// </summary>
        Dual
    }
}