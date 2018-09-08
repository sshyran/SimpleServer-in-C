// IAttributeHandlerResolver.cs - Ultz.SimpleServer.Abstractions
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
using Ultz.SimpleServer.Handlers;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Provides methods to search an object for handlers
    /// </summary>
    public interface IAttributeHandlerResolver
    {
        /// <summary>
        ///     Searches the given object to resolve methods able to handle requests from the <see cref="IProtocol" /> this
        ///     <see cref="IAttributeHandlerResolver" /> is a member of.
        /// </summary>
        /// <param name="obj">the object to search</param>
        /// <returns>an enumerable containing handlers found in the given object</returns>
        IEnumerable<IHandler> GetHandlers(object obj);
    }
}