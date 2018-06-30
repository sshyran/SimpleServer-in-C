using System.IO;
using System.Net;

namespace Ultz.SimpleServer.Internals
{
    public interface IConnection
    {
        Stream Stream { get; }

        bool Connected { get; }

        void Close();

        EndPoint RemoteEndPoint { get; }
    }
}