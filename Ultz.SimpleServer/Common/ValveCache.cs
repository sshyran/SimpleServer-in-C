#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace Ultz.SimpleServer.Common
{
    public class ValveCache
    {
        private static Dictionary<string, IValve> _types;

        public static IDictionary<string, IValve> Valves
        {
            get
            {
                if (_types != null) return _types;
                Dictionary<string, IValve> dictionary = new Dictionary<string, IValve>();
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in asm.GetTypes())
                {
                    if (!type.GetInterfaces().Contains(typeof(IValve)) &&
                        !type.FullName.StartsWith("Ultz.SimpleServer.Common.IValve`1"))
                        continue;
                    IValve instance = (IValve) Activator.CreateInstance(type);
                    dictionary.Add(instance.Id, instance); 
                }

                _types = dictionary;

                return _types;
            }
        }
    }
}