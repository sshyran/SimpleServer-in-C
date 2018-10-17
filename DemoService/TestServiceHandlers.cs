// TestServiceHandlers.cs - DemoService
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System.IO;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace DemoService
{
    public class TestServiceHandlers
    {
        [HttpGet]
        [HttpRedirectFrom("blarg")]
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
            sw.WriteLine("<h1>Hello, " + dat.Remove(0, 5) + "!</h1><a href=\"/\">< Return</a>");
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