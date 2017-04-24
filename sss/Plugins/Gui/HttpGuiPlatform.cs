using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SimpleServer.Plugins.Gui
{
    public class HttpGuiPlatform : IPlatform
    {
        private string url;
        internal HttpGuiPlatform(int port)
        {
            url = "http://127.0.0.1:" + port + "/";
        }

        public bool IsEmpty()
        {
            return false;
        }

        public async void SendUpdateAsync(string controlJson)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url + "update/");
            request.Method = "POST";
            request.Headers["X-SimpleServer-UpdateType"] = "TabUpdate";
            StreamWriter sw = new StreamWriter(await request.GetRequestStreamAsync());
            await sw.WriteAsync(controlJson);
            await sw.FlushAsync();
            sw.Dispose();
        }
    }
}
