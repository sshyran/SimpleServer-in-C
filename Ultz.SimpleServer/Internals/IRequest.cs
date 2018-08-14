#region

using System.IO;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public interface IRequest
    {
        IMethod Method { get; }
        Stream InputStream { get; }
    }
}