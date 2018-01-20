using System.IO;
using System.Net;

namespace SimpleServer.Net.Clients
{
    public interface IClient
    {

        Stream Stream { get; }

        bool Connected { get; }

        void Close();

        EndPoint RemoteEndPoint { get; }



    }
}
