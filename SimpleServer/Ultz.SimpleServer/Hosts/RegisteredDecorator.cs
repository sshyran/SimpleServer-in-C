using Ultz.SimpleServer.Internals;

namespace Ultz.SimpleServer.Hosts
{
    public class RegisteredDecorator : IDecorator
    {
        private readonly IDecorator _child;

        internal RegisteredDecorator(Priority priority, IDecorator child)
        {
            _child = child;
        }

        public Priority Priority { get; set; }

        public IListener Decorate(IListener child)
        {
            return _child.Decorate(child);
        }
    }
}