// HttpTwoConnection.cs - Ultz.SimpleServer.Minimal
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
using Http2;

#endregion

namespace Ultz.SimpleServer.Internals.Http2
{
    /// <summary>
    ///     A <see cref="IConnection" /> implementation for HTTP/2 that ensures the user doesn't tamper with the underlying
    ///     connection without intending to.
    /// </summary>
    public class HttpTwoConnection : IConnection
    {
        private readonly IStream _stream;

        /// <inheritdoc />
        public HttpTwoConnection(IStream stream, IConnection @base)
        {
            _stream = stream;
            UnderlyingConnection = @base;
        }

        /// <summary>
        ///     The underlying connection.
        /// </summary>
        public IConnection UnderlyingConnection { get; }

        /// <inheritdoc />
        public Stream Stream => new HttpTwoStream(_stream);

        /// <inheritdoc />
        public bool Connected => _stream.State != StreamState.Closed;

        /// <inheritdoc />
        public EndPoint LocalEndPoint => UnderlyingConnection.LocalEndPoint;

        /// <inheritdoc />
        public EndPoint RemoteEndPoint => UnderlyingConnection.RemoteEndPoint;

        /// <inheritdoc />
        public int Id => UnderlyingConnection.Id;

        /// <inheritdoc />
        public void Close()
        {
            _stream.CloseAsync().GetAwaiter().GetResult();
        }
    }
}