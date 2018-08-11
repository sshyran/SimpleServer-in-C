using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Ultz.SimpleServer.Internals
{
    public interface IConnection
    {
        Stream Stream { get; }

        bool Connected { get; }

        void Close();

        EndPoint LocalEndPoint { get; }

        EndPoint RemoteEndPoint { get; }
        
        int Id { get; }
    }
}