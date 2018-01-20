using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleServer.Net.Handlers;

namespace SimpleServer.Net.Controllers
{
    public class ErrorContainer : IErrorContainer
    {
        private readonly IList<string> _errors = new List<string>();

        public void Log(string description)
        {
            _errors.Add(description);
        }

        public IEnumerable<string> Errors
        {
            get { return _errors; }
        }
        public bool Any
        {
            get { return _errors.Count != 0; }
        }
        public Task<IControllerResponse> GetResponse()
        {
            return
                Task.FromResult<IControllerResponse>(new RenderResponse(HttpResponseCode.MethodNotAllowed,
                    new {Message = string.Join(", ", _errors)}));
        }
    }
}