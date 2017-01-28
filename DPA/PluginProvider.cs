using SimpleServer.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DMP9Labs.IO.Plugins
{
    public abstract class PluginProvider
    {
        protected static PluginBase ReadPlugin(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryReader r = new BinaryReader(ms);
            PluginBase p = new PluginBase();
            List<byte> rasm = new List<byte>(); 
            string c = new string(r.ReadChars(int.Parse(ms.Length.ToString())));
            string index = c.Split('$')[0]; // Obtain encoded index prior to $ splitter
            string asm = c.Split('$')[1]; // Obtain encoded assembly after $ splitter
            rasm.Add(0x4d); rasm.Add(0x5a); // Mark Zbikowski
            foreach (byte b in new DlfEncoder().DecodeString(asm))
            {
                rasm.Add(b);
            }
            p.Assembly = Assembly.Load(rasm.ToArray());
            p.RawAssembly = rasm.ToArray();

            return p;
        } 
    }
    public class PluginBase
    {
        public PluginIndex Index { get; set; }
        public Assembly Assembly { get; set; }
        public byte[] RawAssembly { get; set; }
    }
    public class PluginIndex
    {
        private string name;
        public string Name { get { return name; } }
        private string author;
        public string Author { get { return author; } }
        private string description;
        public string Description { get { return description; } }
        private string pluginpage;
        public string PluginPage { get { return pluginpage; } }
        public PluginIndex(string s)
        {
            StringReader sr = new StringReader(s);
            string l = "";
            while ((l = sr.ReadLine())!=null)
            {
                if (l.Split('=')[0]=="plugin.name")
                {
                    name = l.Split('=')[1];
                }
            }
        }
    }
}
