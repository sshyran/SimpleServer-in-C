#region

using System.Collections.Generic;
using System.Net;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    public class Connector : IConfigurable
    {
        public Connector(Service service)
        {
            Service = service;
        }

        public Service Service { get; }

        public IListener Listener { get; set; }
        public int Port { get; set; }
        public string Ip { get; set; }

        public List<Valve> Valves { get; set; }

        public IListener GetListener()
        {
            Listener = Service.Protocol.CreateDefaultListener(new IPEndPoint(IPAddress.Parse(Ip), Port));
            Valves.ApplyValves(this);
            return Listener;
        }
    }
}