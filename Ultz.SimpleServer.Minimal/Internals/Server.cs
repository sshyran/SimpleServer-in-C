#region

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals
{
    /// <summary>
    /// Represents a server that listens for connections, processes them as contexts, and passes them to an <see cref="EventHandler"/>.
    /// </summary>
    public class Server : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerProvider _loggerProvider;
        private readonly IProtocol _protocol;
        private CancellationTokenSource _cancellationToken;
        private Task _listenerTask;

        /// <summary>
        /// Creates a server for the given protocol, listening for connections at the given listener, and optionally a logger provider.
        /// </summary>
        /// <param name="protocol">the protocol that connections are passed to</param>
        /// <param name="listener">the listener that listens for connections</param>
        /// <param name="logger">an optional logger provider</param>
        public Server(IProtocol protocol, IListener listener, ILoggerProvider logger = null)
        {
            _protocol = protocol;
            Listener = listener;
            _loggerProvider = logger;
            _logger = _loggerProvider?.CreateLogger("isrv");
        }

        /// <summary>
        /// The underlying connection listener.
        /// </summary>
        public IListener Listener { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationToken?.Dispose();
            Listener?.Dispose();
        }

        /// <summary>
        /// An event that's called when a <see cref="IContext"/> is received.
        /// </summary>
        public event EventHandler<ContextEventArgs> RequestReceived;

        /// <summary>
        /// Starts the server
        /// </summary>
        public void Start()
        {
            _logger.LogDebug("Startup has begun with protocol " + _protocol.GetType().FullName);
            _logger.LogDebug("Running listener checks...");
            Listener.RunChecks();
            _logger.LogDebug("Success.");
            _logger.LogDebug("Starting listener...");
            _cancellationToken = new CancellationTokenSource();
            _protocol.ContextCreated += GotContext;
            Listener.Start();
            _listenerTask = Task.Factory.StartNew(ListenAysnc, _cancellationToken.Token);
            _logger.LogInformation(_protocol.GetType().Name + "Listener has started!");
        }

        private void GotContext(object sender, ContextEventArgs e)
        {
            RequestReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            _logger.LogDebug("Stopping isrv...");
            _cancellationToken.Cancel();
            Listener.Stop();
            _logger.LogInformation("Listener has stopped!");
        }

        private async Task ListenAysnc()
        {
            _cancellationToken.Token.ThrowIfCancellationRequested();
            _logger.LogDebug("Active: " + Listener.Active);
            while (Listener.Active)
            {
                _cancellationToken.Token.ThrowIfCancellationRequested();
                _logger.LogDebug("Awaiting connection...");
                var connection = await Listener.AcceptAsync();
                if (connection == null) // listener shutdown
                    continue;
                _logger.LogDebug("Got connection " + connection.Id + "!");
#pragma warning disable 4014
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await _protocol.HandleConnectionAsync(connection,
                            _loggerProvider?.CreateLogger("c" + connection.Id));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Couldn't handle request" + ex);
                    }
                });
#pragma warning restore 4014
            }
        }

        public void Restart()
        {
            _logger.LogDebug("Restart triggered!");
            Stop();
            Start();
        }
    }
}