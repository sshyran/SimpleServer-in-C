using Microsoft.Extensions.Logging;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

namespace DemoService
{
    public class TestService : Service
    {
        public override IProtocol Protocol { get; }=  Http.Create(HttpMode.Dual);

        protected override void BeforeStart()
        {
            RegisterHandlers(new TestServiceHandlers());
            Logger.LogInformation("Go for launch.");
        }

        protected override void OnStart()
        {
            Logger.LogInformation("Starting");
        }

        protected override void AfterStart()
        {
            Logger.LogInformation("Started");
        }

        protected override void OnStop()
        {
            Logger.LogInformation("Stopping");
        }

        protected override void BeforeStop()
        {
        }

        protected override void AfterStop()
        {
            Logger.LogInformation("Stopped.");
        }

        protected override void OnError(ErrorType type, IContext context)
        {
            Logger.LogInformation(type+": "+CurrentError);
        }
    }
}