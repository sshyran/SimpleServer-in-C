#region

using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public interface IResponse
    {
        Stream OutputStream { get; }
        void Close(bool force = false);
    }
}