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
        protected Service()
        {
            
        }
        
        private IEnumerable<Server> _servers;
        private ILogger _logger;
        public abstract IProtocol Protocol { get; }
        public IEnumerable<IHandler> Handlers { get; } = new List<IHandler>();
        public IEnumerable<Connector> Connectors { get; } = new List<Connector>();
        public List<Valve> Valves { get; set; }
        public ILogger Logger => _logger ?? (_logger = LoggerProvider.CreateLogger(GetType().Name));
        public ILoggerProvider LoggerProvider { get; set; }

        protected abstract void BeforeStart();
        protected abstract void OnStart();
        protected abstract void AfterStart();
        protected abstract void OnStop();
        protected abstract void BeforeStop();
        protected abstract void AfterStop();

        protected Exception CurrentError { get; private set; }
        protected abstract void OnError(ErrorType type, IContext context);

        public void Add(Connector connector)
        {
            connector.Service = this;
            ((List<Connector>)Connectors).Add(connector);
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
            _servers = GetServers().Select(RegisterServer).Select(x =>
            {
                x.Start();
                return x;
            });
            Logger.LogInformation("All servers started");
            AfterStart();
            Logger.LogInformation("Executed post-start operations (AfterStart)");
            Logger.LogInformation(GetType().Name+" is now running.");
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

        public Server RegisterServer(Server server)
        {
            server.RequestReceived += ServerOnRequestReceived;
            return server;
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

        public bool Active => _servers != null;

        public void RegisterHandlers(object handlerObject)
        {
            ((List<IHandler>)Handlers).AddRange(Protocol?.AttributeHandlerResolver?.GetHandlers(handlerObject.GetType()));
        }

        public void RegsiterHandlers(params IHandler[] handlers)
        {
            ((List<IHandler>)Handlers).AddRange(handlers);
        }

        public void RegisterHandler(IHandler handler)
        {
            ((List<IHandler>)Handlers).Add(handler);
        }

        public void RegisterHandler(Func<IRequest, bool> canHandleCallback, Action<IContext> handler)
        {
            ((List<IHandler>)Handlers).Add(new LamdaHandler(canHandleCallback, handler));
        }

        internal IEnumerable<Server> GetServers()
        {
            return Connectors.Select(x => new Server(Protocol, x.GetListener(), LoggerProvider));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Active)
                Stop();
            ((List<IHandler>)Handlers).Clear();
            ((List<Connector>)Connectors).Clear();
            Valves.Clear();
        }
    }
}