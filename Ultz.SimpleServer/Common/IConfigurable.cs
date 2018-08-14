#region

using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Common
{
    public interface IConfigurable
    {
        List<Valve> Valves { get; set; }
    }
}