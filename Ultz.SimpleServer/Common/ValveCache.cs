#region

using System;
using System.Collections.Generic;
using System.Linq;

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
                if (_types == null)
                {
                    _types = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                        from type in asm.GetTypes()
                        from @interface in type.GetInterfaces()
                        let instance = (IValve) Activator.CreateInstance(type)
                        where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IValve<>)
                        select instance).ToDictionary(instance => instance.Id);
                    ;
                    return _types;
                }

                return _types;
            }
        }
    }
}