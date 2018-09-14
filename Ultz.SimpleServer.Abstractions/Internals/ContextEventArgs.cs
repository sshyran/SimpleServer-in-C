// ContextEventArgs.cs - Ultz.SimpleServer.Abstractions
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

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Provides an <see cref="IContext" /> to a context <see cref="EventHandler" />
    /// </summary>
    public class ContextEventArgs
    {
        /// <summary>
        ///     Creates an instance of <see cref="ContextEventArgs" /> with the given context.
        /// </summary>
        /// <param name="context"></param>
        public ContextEventArgs(IContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     The context attached to this instance
        /// </summary>
        public IContext Context { get; }
    }
}