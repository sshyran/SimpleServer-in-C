using System.Collections.Generic;
using System.IO;

namespace Ultz.SimpleServer.Internals
{
    public interface IResponse
    {
        Stream Data { get; set; }
    }
}