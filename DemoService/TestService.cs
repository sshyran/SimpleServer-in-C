// TestService.cs - DemoService
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
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace DemoService
{
    public class TestService : Service
    {
        public override IProtocol Protocol { get; } = Http.Create(HttpMode.Legacy);

        protected override void BeforeStart()
        {
            RegisterHandlers(new TestServiceHandlers());
            RegisterHandlers(new CookieTestHandlers());
            Logger?.LogInformation("Go for launch.");
        }

        protected override void OnStart()
        {
            Logger?.LogInformation("Starting");
        }

        protected override void AfterStart()
        {
            Logger?.LogInformation("Started");
        }

        protected override void OnStop()
        {
            Logger?.LogInformation("Stopping");
        }

        protected override void BeforeStop()
        {
        }

        protected override void AfterStop()
        {
            Logger?.LogInformation("Stopped.");
        }

        protected override void OnError(ErrorType type, IContext context)
        {
            Logger?.LogInformation(type + ": " + CurrentError);
        }
    }
}