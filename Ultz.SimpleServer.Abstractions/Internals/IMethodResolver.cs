#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Ultz.SimpleServer.Internals
{
    public abstract class MethodResolver<T> : IMethodResolver where T : IMethod
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
            return _methods.First(x => x.Key.SequenceEqual(id)).Value;
        }
    }

    public interface IMethodResolver
    {
        IMethod GetMethod(byte[] id);
    }
}