// ErrorType.cs - Ultz.SimpleServer
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

using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A reason to why an error has been thrown
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// No handlers can handle the <see cref="IRequest"/>
        /// </summary>
        HandlerNotFound = 404,
        /// <summary>
        /// An unhandled exception occured
        /// </summary>
        UnhandledException = 500
    }
}