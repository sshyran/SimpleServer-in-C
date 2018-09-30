// Connector.cs - Ultz.SimpleServer
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

using System;
using System.Collections.Generic;
using System.Net;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    ///     A class representing an <see cref="IPEndPoint" />
    /// </summary>
    public class Connector : IConfigurable
    {
        /// <summary>
        ///     Creates a <see cref="Connector" /> representing the given <see cref="IPEndPoint" />
        /// </summary>
        /// <param name="endPoint">the endpoint that this connector should represent</param>
        public Connector(IPEndPoint endPoint)
        {
            Port = endPoint.Port;
            Ip = endPoint.Address.ToString();
        }

        /// <summary>
        ///     Creates a <see cref="Connector" /> representing the given <see cref="IPAddress" /> and port.
        /// </summary>
        /// <param name="address">the scope of the <see cref="IPEndPoint" /></param>
        /// <param name="port">the port</param>
        public Connector(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {
        }

        /// <summary>
        ///     Creates a <see cref="Connector" /> representing the given address and port.
        /// </summary>
        /// <param name="address">the raw <see cref="IPAddress" /></param>
        /// <param name="port">the port</param>
        public Connector(string address, int port) : this(IPAddress.Parse(address), port)
        {
        }

        /// <summary>
        ///     The <see cref="Service" /> this <see cref="Connector" /> is attached to
        /// </summary>
        public Service Service { get; set; }

        /// <summary>
        ///     The listener returned by <see cref="GetListener" />. This method shouldn't be used unless used by a
        ///     <see cref="Valve" />.
        /// </summary>
        public IListener Listener { get; set; }

        /// <summary>
        ///     The port this <see cref="Connector" /> should bind to
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The raw <see cref="IPAddress" /> of this <see cref="Connector" />
        /// </summary>
        public string Ip { get; set; }


        /// <inheritdoc />
        public List<Valve> Valves { get; set; } = new List<Valve>();

        /// <summary>
        ///     Creates a default listener and applies <see cref="Valve" />s to it
        /// </summary>
        /// <returns>the listener</returns>
        /// <exception cref="NullReferenceException"><see cref="Service" /> has not been set</exception>
        public IListener GetListener()
        {
            if (Service == null)
                throw new NullReferenceException(
                    "This connector has not been setup to work with a service yet. Please set the Service property on this object.");
            Listener = Service.Protocol.CreateDefaultListener(new IPEndPoint(IPAddress.Parse(Ip), Port));
            Valves.ApplyValves(this);
            return Listener;
        }
    }
}