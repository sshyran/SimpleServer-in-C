#region

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public class Server : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ILoggerProvider _loggerProvider;
        private readonly IProtocol _protocol;
        private CancellationTokenSource _cancellationToken;
        private Task _listenerTask;

        public Server(IProtocol protocol, IListener listener, ILoggerProvider logger = null)
        {
            _protocol = protocol;
            Listener = listener;
            _loggerProvider = logger;
            _logger = _loggerProvider?.CreateLogger("isrv");
        }

        public IListener Listener { get; }

        public void Dispose()
        {
            _listenerTask?.Dispose();
            _cancellationToken?.Dispose();
            Listener?.Dispose();
        }

        public event EventHandler<ContextEventArgs> RequestReceived;

        public void Start()
        {
            _logger.LogDebug("Startup has begun with protocol " + _protocol.GetType().FullName);
            _logger.LogDebug("Running listener checks...");
            Listener.RunChecks();
            _logger.LogDebug("Success.");
            _logger.LogDebug("Starting listener...");
            _cancellationToken = new CancellationTokenSource();
            _protocol.ContextCreated += GotContext;
            _listenerTask = Task.Factory.StartNew(ListenAysnc, _cancellationToken.Token);
            _logger.LogInformation(_protocol.GetType().Name + "Listener has started!");
        }

        private void GotContext(object sender, ContextEventArgs e)
        {
            RequestReceived?.Invoke(this, e);
        }

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
            while (Listener.Active)
            {
                _cancellationToken.Token.ThrowIfCancellationRequested();
                var connection = await Listener.AcceptAsync();
                if (connection == null) // listener shutdown
                    continue;
#pragma warning disable 4014
                Task.Factory.StartNew(async () =>
                {
                    await _protocol.HandleConnectionAsync(connection,
                        _loggerProvider?.CreateLogger("c" + connection.Id));
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