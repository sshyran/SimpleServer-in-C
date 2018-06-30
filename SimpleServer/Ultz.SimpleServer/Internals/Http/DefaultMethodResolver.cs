using System.Linq;

namespace Ultz.SimpleServer.Internals.Http
{
    public class DefaultMethodResolver : IHttpMethodResolver
    {
        public HttpMethod GetMethod(string m)
        {
            return HttpMethod.DefaultMethods.FirstOrDefault(x => x.Name == m.ToUpper());
        }
    }
}