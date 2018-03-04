using System.IO;

namespace SimpleServer.Decorators
{
    public interface IDecorator
    {
        void BeforeHandle();
        void AfterHandle();
        Stream GetStream();
    }
}