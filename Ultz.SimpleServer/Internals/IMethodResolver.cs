using System.Collections.Generic;

namespace Ultz.SimpleServer.Internals
{
    public abstract class MethodResolver<T> : IMethodResolver where T:IMethod
    {
        protected Dictionary<byte[], T> _methods;
        protected MethodResolver(Dictionary<byte[], T> dictionary)
        {
            _methods = dictionary;
        }

        IMethod IMethodResolver.GetMethod(byte[] id)
        {
            return GetMethod(id);
        }
        public T GetMethod(byte[] id)
        {
            return _methods[id];
        }
    }

    public interface IMethodResolver
    {
        IMethod GetMethod(byte[] id);
    }
}