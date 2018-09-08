// ValveCache.cs - Ultz.SimpleServer
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
                if (_types != null) return _types;
                var dictionary = new Dictionary<string, IValve>();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in asm.GetTypes())
                {
                    if (!type.GetInterfaces().Contains(typeof(IValve)) &&
                        !type.FullName.StartsWith("Ultz.SimpleServer.Common.IValve`1"))
                        continue;
                    var instance = (IValve) Activator.CreateInstance(type);
                    dictionary.Add(instance.Id, instance);
                }

                _types = dictionary;

                return _types;
            }
        }
    }
}