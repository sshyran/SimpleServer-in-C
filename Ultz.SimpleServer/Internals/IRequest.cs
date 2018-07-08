using System.IO;

namespace Ultz.SimpleServer.Internals
{
    public interface IRequest
    {
        IMethod Method { get; }
        Stream Data { get; }
    }
}