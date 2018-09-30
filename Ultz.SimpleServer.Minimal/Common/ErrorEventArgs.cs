// ErrorEventArgs.cs - Ultz.SimpleServer.Minimal
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
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    ///     <see cref="EventArgs" /> that are passed to an error <see cref="EventHandler" />
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        ///     An <see cref="Exception" /> representing the error thrown by the server.
        /// </summary>
        public Exception CurrentError { get; set; }

        /// <summary>
        ///     The <see cref="ErrorType" /> of this error
        /// </summary>
        public ErrorType Type { get; set; }

        /// <summary>
        ///     The context in which this error was thrown
        /// </summary>
        public IContext Context { get; set; }
    }
}