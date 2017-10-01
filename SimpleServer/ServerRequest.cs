using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleServer
{
    public class ServerRequest
    {
        internal NetworkStream stream;
        internal ServerRequest(NetworkStream s)
        {
            stream = s;
            StreamReader sr = new StreamReader(s,Encoding.UTF8,false,4096,true);
            string line;
            List<string> lines = new List<string>();
            while (string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                lines.Add(line);
            }
        }
        public string Method { get; }
        public string Version { get; }
        public string Url { get; }
        public Dictionary<string, string> Headers { get; }
        public Stream Body { get; }
    }
}
