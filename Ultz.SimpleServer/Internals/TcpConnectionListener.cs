using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnectionListener : IListener
    {
        private int _id = 0;
        private TcpListener _listener;
        
        public TcpConnectionListener(TcpListener listener)
        {
            _listener = listener;
        }
        
        public IConnection Accept()
        {
            return AcceptAsync().GetAwaiter().GetResult();
        }

        public async Task<IConnection> AcceptAsync()
        {
            var client = await _listener.AcceptTcpClientAsync();
            return new TcpConnection(client, _id);
        }
    }
}