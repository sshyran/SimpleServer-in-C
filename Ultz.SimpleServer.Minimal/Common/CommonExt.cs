namespace Ultz.SimpleServer.Common
{
    public static class MinimalExt
    {
        public static string ToUrlFormat(this string s)
        {
            if (!s.StartsWith("/"))
                s = "/" + s;
            if (s.EndsWith("/"))
                s = s.Remove(s.Length - 1);
            return s;
        }
    }
}