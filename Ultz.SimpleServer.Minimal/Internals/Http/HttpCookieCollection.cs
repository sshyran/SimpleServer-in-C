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
    /// <summary>
    /// Represents a collection of <see cref="HttpCookie"/>s. Can be read-only.
    /// </summary>
    public class HttpCookieCollection : IList<HttpCookie>, IDictionary<string, HttpCookie>, IDictionary<string, string>
    {
        private List<HttpCookie> _cookies;
        private bool _readOnly;

        /// <summary>
        /// Creates and populates this collection from a <see cref="HttpHeaderCollection"/>, and optionally make it read-only 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="readOnly"></param>
        public HttpCookieCollection(HttpHeaderCollection headers, bool readOnly = false)
        {
            _cookies = new List<HttpCookie>();
            foreach (var headerField in headers)
            {
                if (headerField.Name == "cookie")
                    _cookies.AddRange(HttpCookie.Parse(headerField.Value));
            }

            _readOnly = readOnly;
        }

        /// <summary>
        /// Creates an empty instance of this collection 
        /// </summary>
        public HttpCookieCollection()
        {
            _cookies = new List<HttpCookie>();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return _cookies.ToDictionary(x => x.Name, x => x.Value).GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, HttpCookie>> IEnumerable<KeyValuePair<string, HttpCookie>>.GetEnumerator()
        {
            return _cookies.ToDictionary(x => x.Name, x => x).GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<HttpCookie> GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(HttpCookie item)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            _cookies.Add(item);
        }

        /// <inheritdoc />
        void ICollection<KeyValuePair<string, HttpCookie>>.Add(KeyValuePair<string, HttpCookie> item)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            var itemValue = item.Value;
            itemValue.Name = item.Key;
            _cookies.Add(item.Value);
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<string, string> item)
        {
            Add(new HttpCookie() {Name = item.Key, Value = item.Value});
        }

        /// <summary>
        /// Clears this collection
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public void Clear()
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            _cookies.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, string> item)
        {
            return _cookies.Any(x => x.Name == item.Key && x.Value == item.Value);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _cookies.ToDictionary(x => x.Name, x => x.Value).ToArray().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, string> item)
        {
            if (!Contains(item))
                return false;
            _cookies = _cookies.Where(x => x.Name != item.Key && x.Value != item.Value).ToList();
            return true;
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, HttpCookie> item)
        {
            return _cookies.Any(x => x.Name == item.Key && x == item.Value);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, HttpCookie>[] array, int arrayIndex)
        {
            _cookies.Select(x => new KeyValuePair<string, HttpCookie>(x.Name, x)).ToArray().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, HttpCookie> item)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            return Contains(item) && _cookies.Remove(_cookies.First(x => x.Name == item.Key && x == item.Value));
        }

        /// <summary>
        /// Gets the amount of items in this collection
        /// </summary>
        public int Count => _cookies.Count;

        /// <summary>
        /// Returns true if this collection is read-only, otherwise false
        /// </summary>
        public bool IsReadOnly => ((ICollection<HttpCookie>) _cookies).IsReadOnly;

        /// <inheritdoc />
        public bool Contains(HttpCookie item)
        {
            return _cookies.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(HttpCookie[] array, int arrayIndex)
        {
            _cookies.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(HttpCookie item)
        {
            return _cookies.Contains(item);
        }

        /// <inheritdoc />
        public int IndexOf(HttpCookie item)
        {
            return _cookies.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, HttpCookie item)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            _cookies.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            _cookies.RemoveAt(index);
        }

        /// <inheritdoc />
        public HttpCookie this[int index]
        {
            get => _cookies[index];
            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException("Can't write to a read-only collection");
                }
                else
                {
                    _cookies[index] = value;
                }
            }
        }

        /// <inheritdoc />
        public void Add(string key, HttpCookie value)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            var cookie = value;
            cookie.Name = key;
            _cookies.Add(cookie);
        }

        /// <inheritdoc />
        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Returns a value indicating whether there's a cookie in this collection with the matching key
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the result</returns>
        public bool ContainsKey(string key)
        {
            return _cookies.Any(x => x.Name == key);
        }

        /// <summary>
        /// Removes all cookies with a matching key
        /// </summary>
        /// <param name="key">the key to match</param>
        /// <returns>whether any values were removed</returns>
        /// <exception cref="NotSupportedException">this collection <see cref="IsReadOnly"/></exception>
        public bool Remove(string key)
        {
            if (_readOnly)
                throw new NotSupportedException("Can't write to a read-only collection");
            return ContainsKey(key) && _cookies.Remove(_cookies.First(x => x.Name == key));
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out string value)
        {
            var op = ((IDictionary<string,HttpCookie>)this).TryGetValue(key, out HttpCookie cookie);
            value = cookie?.Value;
            return op;
        }

        /// <inheritdoc />
        public string this[string key]
        {
            get => ((IDictionary<string, HttpCookie>) this)[key].Value;
            set => ((IDictionary<string, HttpCookie>) this)[key].Value = value;
        }

        /// <inheritdoc />
        bool IDictionary<string,HttpCookie>.TryGetValue(string key, out HttpCookie value)
        {
            if (!ContainsKey(key))
            {
                value = new HttpCookie();
                return false;
            }

            value = ((IDictionary<string, HttpCookie>) this)[key];
            return true;
        }

        /// <inheritdoc />
        HttpCookie IDictionary<string, HttpCookie>.this[string key]
        {
            get
            {
                try
                {
                    return _cookies.First(x => x.Name == key);
                }
                catch
                {
                    throw new KeyNotFoundException();
                }
            }
            set
            {
                if (!ContainsKey(key))
                {
                    if (!_readOnly)
                        Add(key, value);
                    else
                        throw new KeyNotFoundException();
                }

                if (_readOnly)
                    throw new NotSupportedException("Can't write to a read-only collection");
                _cookies[_cookies.FindIndex(x => x.Name == key)] = value;
            }
        }

        ICollection<string> IDictionary<string, HttpCookie>.Keys => _cookies.Select(x => x.Name).ToList();

        /// <inheritdoc />
        public ICollection<string> Values => _cookies.Select(x => x.Value).ToList();

        /// <inheritdoc />
        public ICollection<string> Keys => _cookies.Select(x => x.Name).ToList();

        ICollection<HttpCookie> IDictionary<string, HttpCookie>.Values => _cookies;
    }

    /// <summary>
    /// Represents a cookie as defined by the HTTP protocol
    /// </summary>
    public class HttpCookie
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HttpCookie) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Secure.GetHashCode();
                hashCode = (hashCode * 397) ^ HttpOnly.GetHashCode();
                hashCode = (hashCode * 397) ^ (Domain != null ? Domain.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Expires.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxAge.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) SameSite;
                return hashCode;
            }
        }

        /// <summary>
        /// Returns true if the contents of this cookie exactly matches the other cookie provided
        /// </summary>
        /// <param name="other">the other cookie</param>
        /// <returns>the comparison result</returns>
        public bool Equals(HttpCookie other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value) &&
                   string.Equals(Path, other.Path) && Secure == other.Secure && HttpOnly == other.HttpOnly &&
                   string.Equals(Domain, other.Domain) && Expires.Equals(other.Expires) && SameSite == other.SameSite;
        }

        /// <summary>
        /// Returns true if the contents of the left cookie exactly matches the right cookie
        /// </summary>
        /// <param name="right">the right cookie</param>
        /// <returns>the comparison result</returns>
        public static bool operator ==(HttpCookie left, HttpCookie right)
        {
            return left?.Equals(right) ?? right is null;
        }

        /// <summary>
        /// Returns true if the contents of the left cookie exactly doesn't match the right cookie
        /// </summary>
        /// <param name="right">the right cookie</param>
        /// <returns>the comparison result</returns>
        public static bool operator !=(HttpCookie left, HttpCookie right)
        {
            return !(left == right);
        }

        /// <summary>
        /// The name/key of this cookie
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value/content of this cookie
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The path that this cookie should be set at. See the HTTP RFC for more info.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// If this cookie should only be set & sent on a secure endpoint. See the HTTP RFC for more info.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// If this cookie should only be accessed by the server, and not client-side scripts. See the HTTP RFC for more info.
        /// </summary>
        public bool HttpOnly { get; set; }

        /// <summary>
        /// The domain that this cookie should be set at. See the HTTP RFC for more info.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// The date that this cookie should expire. See the HTTP RFC for more info.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// The amount of seconds before this cookie should expire. Takes precedence over <see cref="Expires"/>, see the HTTP RFC for more info.
        /// </summary>
        public long MaxAge { get; set; }

        /// <summary>
        /// The mode of prevention of this cookie being sent in cross-site requests. See the HTTP RFC for more info.
        /// </summary>
        public SameSiteMode SameSite { get; set; }

        /// <summary>
        /// Creates a Set-Cookie header value from this instance.
        /// </summary>
        /// <returns>a Set-Cookie header value</returns>
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
            if (MaxAge > 0)
                val += "Max-Age=" + MaxAge + "; ";
            return val.TrimEnd();
        }

        // ReSharper disable ConvertIfStatementToSwitchStatement
        /// <summary>
        /// Parses a cookie from a Cookie request header value. See the HTTP RFC for more info. The returned cookie does not contain any response header values such as <see cref="Secure"/> or <see cref="HttpOnly"/>
        /// </summary>
        /// <param name="s">the request header</param>
        /// <returns>the parsed cookie(s)</returns>
        public static IEnumerable<HttpCookie> Parse(string s)
        {
            foreach (var part in s.Split(';').Select(x => x.Trim()))
            {
                if (!part.Contains('='))
                    continue;
                var kvp = part.Split('=');
                var key = kvp[0];
                var value = kvp.Where(x => x != kvp[0]).Aggregate("", (current, s1) => current + s1 + "=")
                    .TrimEnd('=');
                yield return new HttpCookie() {Name = key, Value = value};
            }
        }
        // ReSharper restore ConvertIfStatementToSwitchStatement
    }

    /// <summary>
    /// Represents values of the SameSite attribute of a Set-Cookie header. See the HTTP RFC for more info.
    /// </summary>
    public enum SameSiteMode
    {
        /// <summary>
        /// Strips the SameSite property from the Set-Cookie header
        /// </summary>
        None,

        /// <summary>
        /// Represents the lax value of the Set-Cookie header. See the HTTP RFC for more info.
        /// </summary>
        Lax,

        /// <summary>
        /// Represents the strict value of the Set-Cookie header. See the HTTP RFC for more info.
        /// </summary>
        Strict
    }
}