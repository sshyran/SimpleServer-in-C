// IHandler.cs - Ultz.SimpleServer.Abstractions
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

using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Handlers
{
    /// <summary>
    ///     Represents a handler that reads <see cref="IRequest" />s and responds to them, if the handler
    ///     <see cref="CanHandle" /> it.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        ///     Causes the handler to check this request to see if it can handle it.
        /// </summary>
        /// <param name="request">the request in question</param>
        /// <returns>true if this handler is able to handle the request, false otherwise.</returns>
        bool CanHandle(IRequest request);

        /// <summary>
        ///     Handles an <see cref="IContext" />
        /// </summary>
        /// <param name="context">the context to handle</param>
        void Handle(IContext context);
    }
}