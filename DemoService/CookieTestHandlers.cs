// CookieTestHandlers.cs - DemoService
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

using System;
using System.Text;
using Ultz.SimpleServer.Internals.Http;

namespace DemoService
{
    [HttpRoute("cookies")]
    public class CookieTestHandlers
    {
        [HttpGet]
        public void GetCookiePage(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("ss_cookie_test_"))
            {
                if (context.Request.Cookies.TryGetValue("ss_cookie_test_", out var cookie))
                {
                    context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("<h1>Cookie Value: "+cookie+"</h1><form method=\"POST\"><input type=\"submit\" value=\"Reset cookie\" /></form>"));
                    context.Response.Close();
                }
                else
                {
                    context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("<h1>WTF!</h1><form method=\"POST\"><input type=\"submit\" value=\"Reset cookie\" /></form>"));
                    context.Response.Close();
                }
            }
            else
            {
                context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("<h1>Cookie not set!</h1><form method=\"POST\"><input type=\"submit\" value=\"Reset cookie\" /></form>"));
                context.Response.Close();
            }
        }

        [HttpPost]
        public void ResetCookie(HttpContext context)
        {
            context.Response.Cookies.Add("ss_cookie_test_",DateTime.Now.ToString("R"));
            context.Response.Cookies.Add("ss_cookie_test2_",DateTime.Now.ToString("R"));
            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("<h1>Set!</h1><form method=\"GET\"><input type=\"submit\" value=\"See if it worked\" /></form>"));
            context.Response.Close();
        }
    }
}