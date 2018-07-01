using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.SimpleServer.Internals
{
    public class TcpConnectionListener : IListener
    {
        public TcpConnectionListener(TcpListener listener)
        {
            
        }
        
        public IConnection Accept()
        {
            throw new System.NotImplementedException();
        }

        public Task<IConnection> AcceptAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}