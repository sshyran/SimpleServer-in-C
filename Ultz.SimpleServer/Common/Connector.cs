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
        public Connector(){}

        public Connector(IPEndPoint endPoint)
        {
            Port = endPoint.Port;
            Ip = endPoint.Address.ToString();
        }

        public Connector(IPAddress address, int port): this(new IPEndPoint(address,port)){}
        public Connector(string address, int port): this(IPAddress.Parse(address),port){}
        public Service Service { get; set; }

        public IListener Listener { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }

        public List<Valve> Valves { get; set; }

        public IListener GetListener()
        {
            if (Service == null)
                throw new NullReferenceException("This connector has not been setup to work with a service yet. Please set the Service property on this object.");
            Listener = Service.Protocol.CreateDefaultListener(new IPEndPoint(IPAddress.Parse(Ip), Port));
            Valves.ApplyValves(this);
            return Listener;
        }
    }
}