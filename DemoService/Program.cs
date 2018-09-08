// Program.cs - DemoService
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
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ultz.SimpleServer.Common;

#endregion

namespace DemoService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Logging code
            var loggerFactory = new LoggerFactory().AddConsole();

            var server = new SimpleServer(new ConsoleLoggerProvider((s, level) => true, true));
            var service = new TestService();
            service.Add(new Connector(IPAddress.Loopback, 8081));
            server.Add(service);
            server.Start();
            Console.ReadLine();
        }
    }
}