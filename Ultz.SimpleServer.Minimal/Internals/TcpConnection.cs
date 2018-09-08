// TcpConnection.cs - Ultz.SimpleServer.Minimal
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
using System.Net.Sockets;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Wrapper class for a <see cref="TcpClient" />
    /// </summary>
    public class TcpConnection : IConnection
    {
        /// <summary>
        ///     Creates an instance with the underlying client, connection id, and optional NoDelay.
        /// </summary>
        /// <param name="client">the client to wrap</param>
        /// <param name="id">the connection id</param>
        /// <param name="noDelay">sets nodelay on the base</param>
        public TcpConnection(TcpClient client, int id, bool noDelay = true)
        {
            Base = client;
            Id = id;
            Base.NoDelay = noDelay;
        }

        private TcpClient Base { get; }

        /// <inheritdoc />
        public Stream Stream => Base.GetStream();

        /// <inheritdoc />
        public bool Connected => Base.Connected;

        /// <inheritdoc />
        public void Close()
        {
            Base.Dispose();
        }

        /// <inheritdoc />
        public EndPoint LocalEndPoint => Base.Client.LocalEndPoint;

        /// <inheritdoc />
        public EndPoint RemoteEndPoint => Base.Client.RemoteEndPoint;

        /// <inheritdoc />
        public int Id { get; }
    }
}