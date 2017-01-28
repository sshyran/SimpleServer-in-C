using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleServer.Core.Platform.Net.Listeners
{
    public interface Listener
    {
        void StartListening();
        void StopListening();
    }
}
