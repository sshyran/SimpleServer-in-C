// Service.cs - Ultz.SimpleServer
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Handlers;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Common
{
    /// <summary>
    /// A <see cref="Service"/> containing <see cref="IHandler"/>s and one or more <see cref="Connector"/> endpoints.
    /// </summary>
    public abstract class Service : IConfigurable, IDisposable
    {
        private ILogger _logger;

        private List<Server> _servers;

        /// <summary>
        /// The protocol that this <see cref="Service"/> is designed for
        /// </summary>
        public abstract IProtocol Protocol { get; }
        /// <summary>
        /// The handlers registered on this <see cref="Service"/>.
        /// </summary>
        public IEnumerable<IHandler> Handlers { get; } = new List<IHandler>();
        /// <summary>
        /// The endpoints that this <see cref="Service"/> will listen at.
        /// </summary>
        public IEnumerable<Connector> Connectors { get; } = new List<Connector>();
        /// <summary>
        /// The <see cref="ILogger"/> for this <see cref="Service"/>.
        /// </summary>
        public ILogger Logger => _logger ?? (_logger = LoggerProvider?.CreateLogger(GetType().Name));
        /// <summary>
        /// An <see cref="ILoggerProvider"/> for this <see cref="Service"/>, <see cref="IConnection"/>s, and other underlying classes.
        /// </summary>
        public ILoggerProvider LoggerProvider { get; set; }

        /// <summary>
        /// An <see cref="Exception"/> representing the current error at the time of <see cref="OnError"/> execution
        /// </summary>
        protected Exception CurrentError { get; private set; }

        /// <summary>
        /// True if this <see cref="Service"/> has been started
        /// </summary>
        public bool Active => _servers != null;
        /// <summary>
        /// <see cref="Valve"/> to be applied to this <see cref="Service"/>, or its members.
        /// </summary>
        public List<Valve> Valves { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Active)
                Stop();
            ((List<IHandler>) Handlers).Clear();
            ((List<Connector>) Connectors).Clear();
            Valves.Clear();
        }

        /// <summary>
        /// Operations that should be executed before the <see cref="Service"/> begins starting up.
        /// </summary>
        protected abstract void BeforeStart();
        /// <summary>
        /// Operations that should be executed mid-startup
        /// </summary>
        protected abstract void OnStart();
        /// <summary>
        /// Operations that should be executed after the <see cref="Service"/> has started
        /// </summary>
        protected abstract void AfterStart();
        /// <summary>
        /// Operations that should be executed before the <see cref="Service"/> begins stopping up.
        /// </summary>
        protected abstract void OnStop();
        /// <summary>
        /// Operations that should be executed mid-shutdown
        /// </summary>
        protected abstract void BeforeStop();
        /// <summary>
        /// Operations that should be executed after the <see cref="Service"/> has stopped
        /// </summary>
        protected abstract void AfterStop();
        /// <summary>
        /// Executed when an <see cref="Exception"/> or other error is thrown when handling a request
        /// </summary>
        /// <param name="type">the error type</param>
        /// <param name="context">the context in question</param>
        protected abstract void OnError(ErrorType type, IContext context);

        /// <summary>
        /// Adds a <see cref="Connector"/> to this <see cref="Service"/>'s <see cref="Connectors"/>
        /// </summary>
        /// <param name="connector">the connector to add</param>
        public void Add(Connector connector)
        {
            connector.Service = this;
            ((List<Connector>) Connectors).Add(connector);
        }

        /// <summary>
        /// Adds multiple <see cref="Connector"/>s to this <see cref="Service"/>'s <see cref="Connectors"/>
        /// </summary>
        /// <param name="connectors"></param>
        public void Add(params Connector[] connectors)
        {
            foreach (var connector in connectors)
                Add(connector);
        }

        /// <summary>
        /// Starts this <see cref="Service"/> on all <see cref="Connectors"/>
        /// </summary>
        public void Start()
        {
            BeforeStart();
            Logger?.LogInformation("Executed prerequisites (BeforeStart)");
            OnStart();
            Logger?.LogInformation("Executed runtime prerequisites (OnStart)");
            Logger?.LogInformation("Starting all servers...");
            _servers = new List<Server>();
            foreach (var server in GetServers())
            {
                server.Start();
                _servers.Add(server);
            }

            Logger?.LogInformation("All servers started");
            AfterStart();
            Logger?.LogInformation("Executed post-start operations (AfterStart)");
            Logger?.LogInformation(GetType().Name + " is now running.");
        }

        /// <summary>
        /// Stops this <see cref="Service"/> on all <see cref="Connectors"/>
        /// </summary>
        public void Stop()
        {
            BeforeStop();
            OnStop();
            foreach (var server in _servers)
                server.Stop();
            _servers = null;
            AfterStop();
        }

        private void ServerOnRequestReceived(object sender, ContextEventArgs e)
        {
            var handler = Handlers.FirstOrDefault(x => x.CanHandle(e.Context.Request));
            if (handler == null)
            {
                CurrentError = new Exception("Handler not found");
                OnError(ErrorType.HandlerNotFound, e.Context);
                CurrentError = null;
                return;
            }

            try
            {
                handler.Handle(e.Context);
            }
            catch (Exception ex)
            {
                CurrentError = ex;
                try
                {
                    OnError(ErrorType.UnhandledException, e.Context);
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

                CurrentError = null;
            }
        }

        /// <summary>
        /// Uses a <see cref="IAttributeHandlerResolver"/> to register <see cref="Handlers"/>
        /// </summary>
        /// <param name="handlerObject">the type and instance to search and register</param>
        public void RegisterHandlers(object handlerObject)
        {
            ((List<IHandler>) Handlers).AddRange(Protocol?.AttributeHandlerResolver?.GetHandlers(handlerObject) ?? new List<IHandler>());
        }

        /// <summary>
        /// Registers multiple <see cref="IHandler"/>s
        /// </summary>
        /// <param name="handlers">the handlers to register</param>
        public void RegsiterHandlers(params IHandler[] handlers)
        {
            ((List<IHandler>) Handlers).AddRange(handlers);
        }

        /// <summary>
        /// Registers a single <see cref="IHandler"/>
        /// </summary>
        /// <param name="handler">the handler to register</param>
        public void RegisterHandler(IHandler handler)
        {
            ((List<IHandler>) Handlers).Add(handler);
        }

        /// <summary>
        /// Creates a <see cref="LamdaHandler"/> using lamda expressions for CanHandle and Handle methods.
        /// </summary>
        /// <param name="canHandleCallback"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(Func<IRequest, bool> canHandleCallback, Action<IContext> handler)
        {
            ((List<IHandler>) Handlers).Add(new LamdaHandler(canHandleCallback, handler));
        }

        private IEnumerable<Server> GetServers()
        {
            return Connectors.Select(x =>
            {
                var srv = new Server(Protocol, x.GetListener(), LoggerProvider);
                srv.RequestReceived += ServerOnRequestReceived;
                return srv;
            });
        }
    }
}