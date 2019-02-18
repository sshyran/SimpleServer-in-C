// Program.cs - DemoProject
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ultz.Extensions.PrivacyEnhancedMail;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals;
using Ultz.SimpleServer.Internals.Http;

// ReSharper disable HeuristicUnreachableCode
// ReSharper disable ConditionIsAlwaysTrueOrFalse

#endregion

namespace DemoProject
{
    internal class Program
    {
        private const bool IsSslTest = false;

        private static void Main(string[] args)
        {
#pragma warning disable 162
            Server server;
            var logger = new ConsoleLoggerProvider((s, level) => true, true);
            if (IsSslTest)
                server = new Server(Http.Create(HttpMode.Dual),
                    new SslListener(new TcpConnectionListener(IPAddress.Loopback, 11112),
                        new SslServerAuthenticationOptions
                        {
                            EnabledSslProtocols = SslProtocols.Tls12,
                            ServerCertificate =
                                Pem.GetCertificate(
                                    "-----BEGIN CERTIFICATE-----MIID8zCCAtugAwIBAgIgdsp/a6adzYJ9SXdO19c9NX+xgW5VufVWrFK/Ddi5d7kwDQYJKoZIhvcNAQEFBQAwgYoxEjAQBgNVBAYTCVdvcmNlc3RlcjEVMBMGA1UECgwMVWx0eiBMaW1pdGVkMRQwEgYDVQQLDAtEZXZlbG9wbWVudDESMBAGA1UEAwwJbG9jYWxob3N0MR8wHQYJKoZIhvcNAQkBFhBsZWdhbEB1bHR6LmNvLnVrMRIwEAYDVQQDDAlsb2NhbGhvc3QwHhcNMTgwODE0MTY1OTM4WhcNMjgwODE0MTY1OTM4WjB2MRIwEAYDVQQGEwlXb3JjZXN0ZXIxFTATBgNVBAoMDFVsdHogTGltaXRlZDEUMBIGA1UECwwLRGV2ZWxvcG1lbnQxEjAQBgNVBAMMCWxvY2FsaG9zdDEfMB0GCSqGSIb3DQEJARYQbGVnYWxAdWx0ei5jby51azCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAI7zzCylJ4oSXuj1y4MmhRHBS0t5K8kwOQbrzRLFsbRU6vuMU/ZjiDZfNbTgfDvl/USaTxxRERnd+4SpNEOp5lMzjHDHCDtl54F00+7NJiGmA6KTcxS8u0oOnxBmr+cB7gNm/VdFTopq1xaXZ/W+066zllRPtGdPCpBO2irr4mHeCCyoCBwixj+Yrz03V8Ilr0g4gBlz1FqD/2B5bZ6pGiAVivecfT1wJKCBxjZSxLd2xmRtHeFxTUBbpkFiRcTE1PYz34g0EvhdWYRQmmjfy2DQomK3sHPLZj7OOLoyEaLzO+IVv2eN9SIx+JLPyOtuMF1AwVuswDJHODdrPGlqlcMCAwEAAaNYMFYwHQYDVR0OBBYEFJaAdm+WoLHxQBO1GEr5i4jhQ0R3MB8GA1UdIwQYMBaAFJaAdm+WoLHxQBO1GEr5i4jhQ0R3MBQGA1UdEQQNMAuCCWxvY2FsaG9zdDANBgkqhkiG9w0BAQUFAAOCAQEAGdmBS9KmR1iJSRa2lp7ZzWYawnuuMbxG+vsW9VOEpRFzP9697hQWdC3oyf+L8rxAD8dMO87kmZq46A7qxelt2cbdTtQj2ElYZzWr2SA0TrvG7F1SBBNGkAKwdbhXSa17bni1HbYi74EqLg4VHGE+vBl0ZSxe6QGcxsBasQty6K1Fks8Uul4eXcLFn5+dbspLdgfE8HFApXlsGeZ2AgAToztoDWuX90WdJpRmpEHUWnwpuXkrXVbSd4aMwdJ+AZeUPLkanqi/HpZkTFKcZdz5zDxQV8CRqh+dpGTnkF3rA6lWrQWGsOaNAS0DHNYlF6MeSG7x6/kK4aapHSf6dTKtJA==-----END CERTIFICATE-----",
                                    "-----BEGIN RSA PRIVATE KEY-----MIIEowIBAAKCAQEAjvPMLKUnihJe6PXLgyaFEcFLS3kryTA5BuvNEsWxtFTq+4xT9mOINl81tOB8O+X9RJpPHFERGd37hKk0Q6nmUzOMcMcIO2XngXTT7s0mIaYDopNzFLy7Sg6fEGav5wHuA2b9V0VOimrXFpdn9b7TrrOWVE+0Z08KkE7aKuviYd4ILKgIHCLGP5ivPTdXwiWvSDiAGXPUWoP/YHltnqkaIBWK95x9PXAkoIHGNlLEt3bGZG0d4XFNQFumQWJFxMTU9jPfiDQS+F1ZhFCaaN/LYNCiYrewc8tmPs44ujIRovM74hW/Z431IjH4ks/I624wXUDBW6zAMkc4N2s8aWqVwwIDAQABAoIBAAh4K55w3IJYK0pcbFslWENLM0r8mgUDTTr6k9MAZn0Q05PlZlYV4zdBddJKbeHd8vRHu4HUexG6w/uXS3LqKSzAbu+BmJxtoa/a/EbM3cJRmSvegODM0RXb/ug/BT3yWyfVcr4NwPpUxns0AONKPoq+YMeMVLES5EUqKXqF24rrwNss81VjKusHCu6ghhNLK8+WOlljqMaWeFJ+rtILaajPEjwtCQhoMU+Z8+pdXwfU2BMvSTx/srEzYDgr8w/1pk9Id08zDje7Dn6gZBN/4NNF66dtMt6/87acRKEFRE0bbLDpY0nX9Pm++EfGZmLApH84XypREnRMddTQgTMTVpkCgYEAxt1rwigGWSdjY6gzJ9b6c0iwTPOvIk/u1F37dTrnfZoyKGlcxAy+tZmt+CvxG0W/J/Y2sYvrXt8ldIrVeAJul+KbtXVmJseZpeIAZ4XCHmb8zJxCffGNtZHqrTKmKwJQKbbzczQImi/WdinVzjIMyQgnipOaKP1B6LmxpQ1W+wsCgYEAuAX80yfu01dOlSDQwiqI1r5mgI3XGfRkQMIkkZZ/Oik05/ODMLSS3DYdmk67uEmP+nf39WLminIV6kYBWAA+SayA81FQ7CZOqK1og5gfRQOrAabMbT7yqa65pBSwriea9rgLuZlQRUOQ/Shq/GU01vO6Ce+VLryR7BqH/8GpwykCgYEAtJnWCSfMTB9HVfQlMSM9pID5C4mrHaA2J8uKWHa8UQc+UhEN3EYu1EHTCrTtbHU1GxexqCCIC0rgeyyynSCoS2vTOUJ7GPDgixPqhhmlp3KkVzX59OLwbVstI0oCOsEJCDlMcu1oeo7DV+C6eV5e2ht7vZA6ysrllnM978Vjnu0CgYAiun6MGu0nVUKvQhIjkoNgg240tI/zhfulfP4Ju60m/L/PRlVry6grhsrvZAxpKvjQ+/L/jDqVxhH8tFlskh8vKC7tvFrZNiGCE7e1ne/IxnhvR1stAsQo4aCHJqBxPWgxR2pvDE/pwmaKYCZQm4jtR/HEDkLJHy0qsZcY3SN8gQKBgEz04YAwWA529u232rEyoI3xOd/VuQbEiXlXrYTldMZVE4cyATP8aNjtJT5s8J2gCRsr9yrFNhk92AX0LWtnESoQg7RPIDHEB90Y+S+mPmV+Il92u8iim3y3kz1QnJ221TgzghaikYCiI1iv16muH4UgjRiq6O5h9iW8zgJwspMU-----END RSA PRIVATE KEY-----"),
                            ApplicationProtocols = new List<SslApplicationProtocol> {SslApplicationProtocol.Http2},
                            ClientCertificateRequired = false
                        }), logger
                );
            else
                server = new Server(Http.Create(HttpMode.Dual),
                    new TcpConnectionListener(IPAddress.Any, 11111),
          //        new ConsoleLoggerProvider((s, level) => true, true));
                    null);

            server.RequestReceived += ServerOnRequestReceived;
            var exceptionLogger = logger.CreateLogger("AppDomain-EX");
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                exceptionLogger.LogError((Exception) eventArgs.ExceptionObject, "Unhandled Exception");
            server.Start();
            Console.ReadLine();
            server.Stop();
#pragma warning restore 162
        }

        private static void ServerOnRequestReceived(object sender, ContextEventArgs e)
        {
            var ctx = (HttpContext) e.Context;
            foreach (var header in ctx.Request.ToDictionary())
                Console.WriteLine(header.Name + ": " + header.Value);
            ctx.Response.Headers["content-type"] = "text/html";
            var sw = new StreamWriter(ctx.Response.OutputStream);
            if (ctx.Request.Method.Id != "POST")
            {
                sw.WriteLine(
                    "<h1>What's your name?</h1><form method=\"POST\">Name: <input type=\"text\" name=\"name\" /><input type=\"submit\"></form>");
            }
            else
            {
                var sr = new StreamReader(ctx.Request.InputStream);
                var dat = sr.ReadToEnd();
                Console.WriteLine(dat);
                sw.WriteLine("<h1>Hello, " + dat.Remove(0, 5) + "!</h1><a href=\"/\">< Return</a>");
            }

            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }
    }
}