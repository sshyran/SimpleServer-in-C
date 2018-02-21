using SimpleServer.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer.Handlers
{
    public interface IHandler
    {
        bool CanHandle(SimpleServerRequest request);
        void Handle(SimpleServerContext context);
    }
}
