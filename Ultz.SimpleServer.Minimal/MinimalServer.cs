// MinimalServer.cs - Ultz.SimpleServer.Minimal
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer
{
    /// <summary>
    /// A minimalistic server that uses <see cref="IHandler"/>s and can use multiple <see cref="IPEndPoint"/>
    /// </summary> 
    public class MinimalServer : IDisposable
    {
        /// <summary>
        /// Creates a <see cref="MinimalServer"/> with the given <see cref="IProtocol"/>
        /// </summary>
        /// <param name="protocol"></param>
        public MinimalServer(IProtocol protocol)
        {
            Protocol = protocol;
        }

        private IEnumerable<Server> _servers;

        /// <summary>
        /// A list of <see cref="IPEndPoint"/>s to bind to
        /// </summary>
        public List<IPEndPoint> Endpoints { get; set; } = new List<IPEndPoint>();

        /// <summary>
        /// A list of <see cref="IHandler"/>s to handle <see cref="IContext"/>s
        /// </summary>
        public List<IHandler> Handlers { get; set; } = new List<IHandler>();

        /// <summary>
        /// The <see cref="IProtocol"/> that this server uses
        /// </summary>
        public IProtocol Protocol { get; }

        /// <summary>
        /// An <see cref="ILoggerProvider"/> for this server and its members
        /// </summary>
        public ILoggerProvider LoggerProvider { get; set; }

        /// <summary>
        /// An <see cref="ILogger"/> for this server.
        /// </summary>
        public ILogger Logger => LoggerProvider?.CreateLogger("MinimalServer");

        /// <summary>
        /// Raised when an error occurs when handling.
        /// </summary>
        public event EventHandler<ErrorEventArgs> OnError;

        /// <summary>
        /// True if this server has been started.
        /// </summary>
        public bool Active => _servers != null;

        /// <summary>
        /// Starts this server on all <see cref="Endpoints"/>
        /// </summary>
        public void Start()
        {
            Logger?.LogInformation("SimpleServer is starting...");
            _servers = Endpoints.Select(x => new Server(Protocol, Protocol.CreateDefaultListener(x), LoggerProvider))
                .Select(x =>
                {
                    x.RequestReceived += XOnRequestReceived;
                    return x;
                });
            foreach (var server in _servers)
            {
                server.Start();
            }
            Logger?.LogInformation("SimpleServer has started.");
        }

        private void XOnRequestReceived(object sender, ContextEventArgs e)
        {
            var handler = Handlers.FirstOrDefault(x => x.CanHandle(e.Context.Request));
            Exception err;
            if (handler == null)
            {
                err = new Exception("Handler not found");
                OnError?.Invoke(this,
                    new ErrorEventArgs() {Type = ErrorType.HandlerNotFound, Context = e.Context, CurrentError = err});
                return;
            }

            try
            {
                handler.Handle(e.Context);
            }
            catch (Exception ex)
            {
                err = ex;
                try
                {
                    OnError?.Invoke(this,
                        new ErrorEventArgs()
                            {Type = ErrorType.HandlerNotFound, Context = e.Context, CurrentError = err});
                }
                catch
                {
                    try
                    {
                        e.Context.Response.Close(CloseMode.Force);
                    }
                    catch
                    {
                        // Look, we've caught every exception imaginable. If we can't force close, give up.
                        // Exception is ignored
                    }
                }
            }
        }

        /// <summary>
        /// Stops this server
        /// </summary>
        public void Stop()
        {
            Logger?.LogInformation("SimpleServer is stopping...");
            foreach (var server in _servers)
            {
                server.Stop();
                server.Dispose();
            }

            _servers = null;
            Logger?.LogInformation("SimpleServer has stopped.");
        }


        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
            LoggerProvider?.Dispose();
            _servers = null;
            Endpoints.Clear();
            Handlers.Clear();
        }
    }
}