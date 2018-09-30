// TcpConnectionListener.cs - Ultz.SimpleServer.Abstractions
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

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     A <see cref="System.Net.Sockets.TcpListener" /> that also listens for <see cref="IListener" />
    /// </summary>
    public class TcpConnectionListener : TcpListener, IListener
    {
        private readonly bool _noDelay;
        private int _id;

        /// <inheritdoc />
        public TcpConnectionListener(IPAddress localaddr, int port, bool noDelay = true) : base(localaddr, port)
        {
            _noDelay = noDelay;
        }

        /// <inheritdoc />
        public TcpConnectionListener(IPEndPoint localEp, bool noDelay = true) : base(localEp)
        {
            _noDelay = noDelay;
        }

        /// <inheritdoc />
        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public async Task<IConnection> AcceptAsync()
        {
            var client = await AcceptTcpClientAsync();
            return new TcpConnection(client, _id++, _noDelay);
        }

        /// <inheritdoc />
        public new void Start()
        {
            base.Start();
        }

        /// <inheritdoc />
        public new void Stop()
        {
            base.Stop();
        }

        /// <inheritdoc />
        public void RunChecks()
        {
            // TODO: Check the port
        }

        /// <inheritdoc />
        public new bool Active => base.Active;

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
        }
    }
}