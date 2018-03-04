using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer
{
    public static class SsExt
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list) await func(value);
        }

        public static string UrlFormat(this string s)
        {
            if (s.Contains("?"))
                return s.Split('?')[0].Split('/').Last().Contains(".") ? s : s.EndsWith("/") ? s : s + "/";
            return s.Split('/').Last().Contains(".") ? s : s.EndsWith("/") ? s : s + "/";
        }

        public static Dictionary<string, string> AsFormParameters(this string s)
        {
            string[] parts;
            var result = new Dictionary<string, string>();
            if (s.Contains("&"))
                parts = s.Split('&');
            else
                parts = new string[1] {s};

            foreach (var part in parts)
            {
                var keyval = part.Split('=');
                result.Add(keyval[0], keyval[1]);
            }

            return result;
        }
    }
}