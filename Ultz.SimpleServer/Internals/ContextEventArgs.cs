namespace Ultz.SimpleServer.Internals
{
    public class ContextEventArgs
    {
        public ContextEventArgs(IContext context)
        {
            Context = context;
        }
        public IContext Context { get; }
    }
}