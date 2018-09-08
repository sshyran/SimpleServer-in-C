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
    public class Connector : IConfigurable
    {
        public Connector()
        {
        }

        public Connector(IPEndPoint endPoint)
        {
            Port = endPoint.Port;
            Ip = endPoint.Address.ToString();
        }

        public Connector(IPAddress address, int port) : this(new IPEndPoint(address, port))
        {
        }

        public Connector(string address, int port) : this(IPAddress.Parse(address), port)
        {
        }

        public Service Service { get; set; }

        public IListener Listener { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }

        public List<Valve> Valves { get; set; } = new List<Valve>();

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