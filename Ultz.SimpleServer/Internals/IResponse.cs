using System.IO;

namespace Ultz.SimpleServer.Internals
{
    public interface IResponse
    {
        Stream OutputStream { get; }
        void Close();
    }
}