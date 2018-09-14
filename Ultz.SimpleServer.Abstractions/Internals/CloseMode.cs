// CloseMode.cs - Ultz.SimpleServer.Abstractions
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

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents what should be done with the stream after the response has been sent, if it's going to be
    /// </summary>
    public enum CloseMode
    {
        /// <summary>
        /// Keeps the stream open after the response has been sent
        /// </summary>
        KeepAlive = 0x73746179,
        /// <summary>
        /// Closes the stream without sending the response
        /// </summary>
        Force = 0x66726365,
        /// <summary>
        /// Sends the response, and closes the stream.
        /// </summary>
        Graceful = 0x6e726d6c,
    }
}