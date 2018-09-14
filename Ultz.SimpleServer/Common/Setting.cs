// Setting.cs - Ultz.SimpleServer
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

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A <see cref="Name"/>/<see cref="Value"/> pair
    /// </summary>
    public struct Setting
    {
        /// <summary>
        /// The name of this <see cref="Setting"/> as recognised by the <see cref="Valve"/>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An acceptable value for this <see cref="Setting"/>.
        /// </summary>
        public string Value { get; set; }
    }
}