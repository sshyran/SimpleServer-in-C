// IContext.cs - Ultz.SimpleServer.Abstractions
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

using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    ///     Represents a <see cref="IRequest" />/<see cref="IResponse" /> protocol communication context.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        ///     The <see cref="IRequest" /> parsed by the underlying protocol
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        ///     A response class that will be formatted and pushed to the <see cref="Connection" /> upon closure.
        /// </summary>
        IResponse Response { get; }

        /// <summary>
        ///     The underlying connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        ///     The logger associated with this context.
        /// </summary>
        ILogger Logger { get; }
    }
}