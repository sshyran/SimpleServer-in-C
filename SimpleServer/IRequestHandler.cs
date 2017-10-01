using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleServer
{
    public interface IRequestHandler
    {
        string[] GetEndpoints();
        void Handle();
    }
}
