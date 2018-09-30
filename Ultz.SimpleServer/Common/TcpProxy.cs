// TcpProxy.cs - Ultz.SimpleServer
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
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    ///     Contains helper classes for a TCP Proxy, intended to be used with the CONNECT method
    /// </summary>
    public static class TcpProxy
    {
        /// <summary>
        ///     Returns a running <see cref="Task" /> proxying traffic to/from the <see cref="TcpConnection" /> from/to the
        ///     <see cref="TcpClient" /> created from the <see cref="IPEndPoint" />
        /// </summary>
        /// <param name="localConnection">the connection that will be on one end of the proxy</param>
        /// <param name="remoteEndPoint">the endpoint of the other end of the proxy</param>
        /// <returns></returns>
        public static Task CreateTcpProxy(TcpConnection localConnection, IPEndPoint remoteEndPoint)
        {
            var client = new TcpClient(remoteEndPoint);
            return Task.Run(() =>
            {
                while (client.Connected && localConnection.Connected)
                {
                    if (client.Available == 0 && !((NetworkStream) localConnection.Stream).DataAvailable)
                        continue;
                    if (client.Available != 0)
                    {
                        var buffer = new byte[client.Available];
                        var count = client.GetStream().Read(buffer, 0, buffer.Length);
                        localConnection.Stream.Write(buffer, 0, count);
                    }

                    if (((NetworkStream) localConnection.Stream).DataAvailable)
                    {
                        var buffer = new byte[4096];
                        var count = localConnection.Stream.Read(buffer, 0, buffer.Length);
                        client.GetStream().Write(buffer, 0, count);
                    }
                }

                if (client.Connected)
                    client.Close();
                if (localConnection.Connected)
                    localConnection.Close();
            });
        }
    }
}