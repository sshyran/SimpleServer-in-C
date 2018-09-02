using System;
using System.IO;
using Ultz.SimpleServer.Internals.Http;

namespace DemoService
{
    public class TestServiceHandlers
    {
        [HttpGet]
        public void GetIndexPage(HttpContext ctx)
        {
            ctx.Response.Headers["content-type"] = "text/html";
            var sw = new StreamWriter(ctx.Response.OutputStream);
            sw.WriteLine(
                "<h1>What's your name?</h1><form method=\"POST\">Name: <input type=\"text\" name=\"name\" /><input type=\"submit\"></form>");
            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }
        [HttpPost]
        public void OnNameReceived(HttpContext ctx)
        {
            var sr = new StreamReader(ctx.Request.InputStream);
            var dat = sr.ReadToEnd();
            ctx.Response.Headers["content-type"] = "text/html";
            var sw = new StreamWriter(ctx.Response.OutputStream);
            sw.WriteLine("<h1>Hello, " + dat?.Remove(0, 5) + "!</h1><a href=\"/\">< Return</a>");
            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }

        [HttpGet("echo?text={text}")]
        public void Echo(HttpContext ctx, string text)
        {
            ctx.Response.Headers["content-type"] = "text/html";
            var sw = new StreamWriter(ctx.Response.OutputStream);
            sw.WriteLine("<h1>" + text + "</h1><a href=\"/\">< Return</a>");
            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }
    }
}