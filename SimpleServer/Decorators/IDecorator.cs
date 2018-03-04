using System.IO;
using SimpleServer.Internals;

namespace SimpleServer.Decorators
{
    public interface IDecorator
    {
        void BeforeHandle();
        void AfterHandle();
        bool CanDecorate(SimpleServerRequest request);
        Stream GetStream(Stream originalStream);
    }
}