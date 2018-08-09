using System.Collections.Generic;

namespace Ultz.SimpleServer.Common
{
    public interface IConfigurable
    {
        List<Valve> Valves { get; set; }
    }
}