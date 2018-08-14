#region

using System.IO;
using System.Net;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public interface IConnection
    {
        Stream Stream { get; }

        bool Connected { get; }

        EndPoint LocalEndPoint { get; }

        EndPoint RemoteEndPoint { get; }

        int Id { get; }

        void Close();
    }
}