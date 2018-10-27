// HttpCookieCollection.cs - Ultz.SimpleServer.Minimal
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpCookieCollection : IList<Cookie>, IDictionary<string, Cookie>
    {
        private List<Cookie> _cookies;

        public HttpCookieCollection(HttpHeaderCollection headers)
        {
            _cookies = new List<Cookie>();
            foreach (var headerField in headers)
            {
                if (headerField.Name == "cookie")
                    _cookies.AddRange(Cookie.Parse(headerField.Value));
            }
        }

        IEnumerator<KeyValuePair<string, Cookie>> IEnumerable<KeyValuePair<string, Cookie>>.GetEnumerator()
        {
            return _cookies.ToDictionary(x => x.Name, x => x).GetEnumerator();
        }

        public IEnumerator<Cookie> GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Cookie item)
        {
            _cookies.Add(item);
        }

        void ICollection<KeyValuePair<string, Cookie>>.Add(KeyValuePair<string, Cookie> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, Cookie>>.Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, Cookie> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, Cookie>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, Cookie> item)
        {
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, Cookie>>.Count => _count1;

        bool ICollection<KeyValuePair<string, Cookie>>.IsReadOnly => _isReadOnly1;

        void ICollection<Cookie>.Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Cookie item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Cookie[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Cookie item)
        {
            throw new NotImplementedException();
        }

        int ICollection<Cookie>.Count => _count;

        bool ICollection<Cookie>.IsReadOnly => _isReadOnly;

        public int IndexOf(Cookie item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Cookie item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Cookie this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Add(string key, Cookie value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out Cookie value)
        {
            throw new NotImplementedException();
        }

        public Cookie this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ICollection<string> Keys { get; }
        public ICollection<Cookie> Values { get; }
    }

    public struct Cookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public string Domain { get; set; }
        public DateTime? Expires { get; set; }
        public SameSiteMode SameSite { get; set; }

        public override string ToString()
        {
            // id=a3fWa; Expires=Wed, 21 Oct 2015 07:28:00 GMT; Secure; HttpOnly
            var val = Name + "=" + Value + "; ";
            if (Expires != null)
                val += "Expires=" + Expires.Value.ToString("R", new CultureInfo("en-GB")) + "; ";
            if (Domain != null)
                val += "Domain=" + Domain + "; ";
            if (Path != null)
                val += "Path=" + Path + "; ";
            if (SameSite != SameSiteMode.None)
                val += "SameSite=" + SameSite.ToString().ToLower() + "; ";
            if (Secure)
                val += "Secure; ";
            if (HttpOnly)
                val += "HttpOnly; ";
            return val.TrimEnd();
        }

        // ReSharper disable ConvertIfStatementToSwitchStatement
        public static IEnumerable<Cookie> Parse(string s)
        {
            foreach (var part in s.Split(';').Select(x => x.Trim()))
            {
                if (!part.Contains('='))
                    continue;
                var kvp = part.Split('=');
                var key = kvp[0];
                var value = kvp.Where(x => x != kvp[0]).Aggregate("", (current, s1) => current + s1 + "=")
                    .TrimEnd('=');
                yield return new Cookie(){Name=key,Value = value};
            }
        }
        // ReSharper restore ConvertIfStatementToSwitchStatement
    }

    public enum SameSiteMode
    {
        None,
        Lax,
        Strict
    }
}