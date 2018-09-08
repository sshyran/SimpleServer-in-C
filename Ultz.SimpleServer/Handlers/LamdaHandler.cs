// LamdaHandler.cs - Ultz.SimpleServer
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

namespace Ultz.SimpleServer.Handlers
{
    public class LamdaHandler : IHandler
    {
        private readonly Func<IRequest, bool> _canHandle;
        private readonly Action<IContext> _handle;

        public LamdaHandler(Func<IRequest, bool> canHandleCallback, Action<IContext> handler)
        {
            _canHandle = canHandleCallback;
            _handle = handler;
        }

        public bool CanHandle(IRequest request)
        {
            return _canHandle(request);
        }

        public void Handle(IContext context)
        {
            _handle(context);
        }
    }
}