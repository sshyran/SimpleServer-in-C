#region

using System.IO;
using System.Net;
using System.Threading.Tasks;
using Ultz.SimpleServer.Internals;

#endregion

namespace Ultz.SimpleServer.Hosts
{
    public abstract class DecoratorListener : IListener
    {
        private readonly IListener _listener;

        protected DecoratorListener(IListener listener)
        {
            _listener = listener;
        }

        public void Dispose()
        {
            _listener?.Dispose();
        }

        public IConnection Accept()
        {
            return GetConnection();
        }

        public async Task<IConnection> AcceptAsync()
        {
            return await GetConnectionAsync();
        }

        public void Start()
        {
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void RunChecks()
        {
            _listener.RunChecks();
        }

        public bool Active => _listener.Active;

        public abstract DecoratedConnection GetConnection();

        public abstract Task<DecoratedConnection> GetConnectionAsync();
    }

    public abstract class DecoratedConnection : IConnection
    {
        protected IConnection _connection;

        protected DecoratedConnection(IConnection connection)
        {
            _connection = connection;
        }

        public abstract Stream Stream { get; }
        public bool Connected => _connection.Connected;
        public abstract void Close();
        public EndPoint LocalEndPoint => _connection.LocalEndPoint;
        public EndPoint RemoteEndPoint => _connection.RemoteEndPoint;
        public int Id => _connection.Id;
    }
}