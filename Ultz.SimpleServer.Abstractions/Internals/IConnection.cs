// IConnection.cs - Ultz.SimpleServer.Abstractions
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

using System.IO;
using System.Net;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Represents a network connection
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        ///     The data <see cref="Stream" /> for communicating with this client
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        ///     Returns true if this client is still connected to the server, false otherwise.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        ///     The local endpoint of this client
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        ///     The remote endpoint of this client
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     The connection ID (used by the server only) used to identify connections apart from eachother
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Terminates this connection
        /// </summary>
        void Close();
    }
}