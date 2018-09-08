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
    public abstract class Service : IConfigurable, IDisposable
    {
        private ILogger _logger;

        private List<Server> _servers;

        public abstract IProtocol Protocol { get; }
        public IEnumerable<IHandler> Handlers { get; } = new List<IHandler>();
        public IEnumerable<Connector> Connectors { get; } = new List<Connector>();
        public ILogger Logger => _logger ?? (_logger = LoggerProvider.CreateLogger(GetType().Name));
        public ILoggerProvider LoggerProvider { get; set; }

        protected Exception CurrentError { get; private set; }

        public bool Active => _servers != null;
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

        protected abstract void BeforeStart();
        protected abstract void OnStart();
        protected abstract void AfterStart();
        protected abstract void OnStop();
        protected abstract void BeforeStop();
        protected abstract void AfterStop();
        protected abstract void OnError(ErrorType type, IContext context);

        public void Add(Connector connector)
        {
            connector.Service = this;
            ((List<Connector>) Connectors).Add(connector);
        }

        public void Add(params Connector[] connectors)
        {
            foreach (var connector in connectors)
                Add(connector);
        }

        public void Start()
        {
            BeforeStart();
            Logger.LogInformation("Executed prerequisites (BeforeStart)");
            OnStart();
            Logger.LogInformation("Executed runtime prerequisites (OnStart)");
            Logger.LogInformation("Starting all servers...");
            _servers = new List<Server>();
            foreach (var server in GetServers())
            {
                server.Start();
                _servers.Add(server);
            }

            Logger.LogInformation("All servers started");
            AfterStart();
            Logger.LogInformation("Executed post-start operations (AfterStart)");
            Logger.LogInformation(GetType().Name + " is now running.");
        }

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
                        e.Context.Response.Close(true);
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

        public void RegisterHandlers(object handlerObject)
        {
            ((List<IHandler>) Handlers).AddRange(Protocol?.AttributeHandlerResolver?.GetHandlers(handlerObject));
        }

        public void RegsiterHandlers(params IHandler[] handlers)
        {
            ((List<IHandler>) Handlers).AddRange(handlers);
        }

        public void RegisterHandler(IHandler handler)
        {
            ((List<IHandler>) Handlers).Add(handler);
        }

        public void RegisterHandler(Func<IRequest, bool> canHandleCallback, Action<IContext> handler)
        {
            ((List<IHandler>) Handlers).Add(new LamdaHandler(canHandleCallback, handler));
        }

        internal IEnumerable<Server> GetServers()
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