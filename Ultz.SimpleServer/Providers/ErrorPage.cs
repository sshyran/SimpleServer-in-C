#region

using System;
using System.IO;
using System.Reflection;
using Ultz.SimpleServer.Internals.Http;

#endregion

namespace Ultz.SimpleServer.Providers
{
    public static class Error
    {
        public static ErrorPage ErrorPage { get; set; } = new ErrorPage
        {
            TypeInAssembly = typeof(Error),
            NamespaceUrlOfType = "Ultz.SimpleServer.Providers.ErrorPages.ErrorPage.html"
        };

        public static void WriteError(this HttpContext context, string errorDetail, int code,
            string reason, Exception ex = null)
        {
            context.Response.StatusCode = code;
            context.Response.ReasonPhrase = reason;
            var stream = Assembly.GetAssembly(ErrorPage.TypeInAssembly)
                .GetManifestResourceStream(ErrorPage.NamespaceUrlOfType);
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(context.Response.OutputStream);
            sw.WriteLine(sr.ReadToEnd().Replace("[Header]", code + " " + reason).Replace("[ErrorDetail]", errorDetail)
                .Replace("[Method]", context.Request.Method.Id).Replace("[Url]", context.Request.RawUrl)
                .Replace("[Connection]", context.Connection.Id.ToString())
                .Replace("[Exception]", ex == null ? string.Empty : ex.ToString()));
            sw.Flush();
            context.Response.Close();
        }
    }

    public class ErrorPage
    {
        /// <summary>
        ///     A type in the target assembly where the error page is stored.
        /// </summary>
        public Type TypeInAssembly { get; set; }

        /// <summary>
        ///     The namespace and filename of the error page template. Example: SimpleServer.Providers.ErrorPages.ErrorPage.html
        /// </summary>
        public string NamespaceUrlOfType { get; set; }
    }
}