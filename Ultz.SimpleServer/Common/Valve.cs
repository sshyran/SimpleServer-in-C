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
    /// <summary>
    ///     Represents a Valve that changes settings on an <see cref="IConfigurable" />
    /// </summary>
    public class Valve
    {
        /// <summary>
        ///     The key/ID of the <see cref="IValve" />
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Settings that should be passed to the resolved<see cref="IValve" />
        /// </summary>
        public List<Setting> Settings { get; set; }
    }

    /// <summary>
    ///     Represents a base <see cref="Valve" /> interface, which adds the ability to be resolved by its <see cref="Id" />
    /// </summary>
    public interface IValve
    {
        /// <summary>
        ///     The Key property of the <see cref="Valve" />
        /// </summary>
        string Id { get; }
    }

    /// <summary>
    ///     Represents a <see cref="Valve" /> that can be assigned to a specific <see cref="IConfigurable" />
    /// </summary>
    /// <typeparam name="T">the target <see cref="IConfigurable" /></typeparam>
    public interface IValve<in T> : IValve where T : IConfigurable
    {
        /// <summary>
        ///     Executes on the target <see cref="IConfigurable" />
        /// </summary>
        /// <param name="obj">the target</param>
        /// <param name="settings">additional settings</param>
        void Execute(T obj, Dictionary<string, string> settings);
    }
}