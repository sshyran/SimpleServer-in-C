using System.Text;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpMethod : IMethod
    {
        public HttpMethod(string name, bool payload)
        {
            Id = name;
            ExpectPayload = payload;
        }
        public string Id { get; }
        byte[] IMethod.Id => Encoding.UTF8.GetBytes(Id);
        public bool ExpectPayload { get; }
    }
}