using System.Collections.Generic;

namespace Ultz.SimpleServer.Internals
{
    public abstract class MethodResolver<T> where T:IMethod
    {
        protected Dictionary<byte[], T> _methods;
        protected MethodResolver(Dictionary<byte[], T> dictionary)
        {
            _methods = dictionary;
        }
        public T GetMethod(byte[] id)
        {
            return _methods[id];
        }
    }
}