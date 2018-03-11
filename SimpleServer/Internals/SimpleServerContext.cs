namespace SimpleServer.Internals
{
    public class SimpleServerContext
    {
        public SimpleServerRequest Request { get; set; }
        public SimpleServerResponse Response { get; set; }
        public SimpleServerConnection Connection { get; set; }
    }
}