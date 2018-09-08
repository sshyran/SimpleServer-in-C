// Valve.cs - Ultz.SimpleServer
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

using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Common
{
    // 1. Read the Type attribute
    // 2. Try to resolve a Type corresponding to the namespace in the type attribute
    // 3. Find out which IValves the Type can be casted to
    // 4. Execute the appropriate method
    public class Valve
    {
        public string Key { get; set; }
        public List<Setting> Settings { get; set; }
    }

    public interface IValve
    {
        string Id { get; }
    }

    public interface IValve<in T> : IValve where T : IConfigurable
    {
        void Execute(T obj, Dictionary<string, string> settings);
    }
}