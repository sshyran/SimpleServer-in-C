// HttpHeaderCollection.cs - Ultz.SimpleServer.Minimal
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Http2.Hpack;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <summary>
    ///     Represents a collection of <see cref="HeaderField" />s
    /// </summary>
    public class HttpHeaderCollection : ICollection<HeaderField>, IDictionary<string, string>
    {
        private List<HeaderField> _headerFields;

        /// <inheritdoc />
        public HttpHeaderCollection(IEnumerable<HeaderField> fields)
        {
            _headerFields = new List<HeaderField>();
            _headerFields.AddRange(fields);
        }

        /// <inheritdoc />
        public IEnumerator<HeaderField> GetEnumerator()
        {
            return _headerFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(HeaderField item)
        {
            _headerFields.Add(item);
        }

        /// <summary>
        ///     Clears this connection
        /// </summary>
        public void Clear()
        {
            _headerFields.Clear();
        }

        /// <inheritdoc />
        public bool Contains(HeaderField item)
        {
            return _headerFields.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(HeaderField[] array, int arrayIndex)
        {
            _headerFields.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(HeaderField item)
        {
            return _headerFields.Remove(item);
        }

        /// <summary>
        ///     The amount of items in this collection.
        /// </summary>
        public int Count => _headerFields.Count;

        /// <summary>
        ///     Gets a value indicating if this collection is writable or read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public bool TryGetValue(string key, out string value)
        {
            value = _headerFields.FirstOrDefault(x => x.Name == key.ToLower()).Value;
            return value != null;
        }

        /// <inheritdoc />
        public string this[string key]
        {
            get { return _headerFields.FirstOrDefault(x => x.Name == key.ToLower()).Value; }
            set
            {
                if (!ContainsKey(key))
                {
                    Add(key, value);
                    return;
                }

                // ReSharper disable once NotAccessedVariable
                var headerField =
                    _headerFields[_headerFields.IndexOf(_headerFields.FirstOrDefault(x => x.Name == key.ToLower()))];
                headerField.Value =
                    value;
            }
        }

        /// <inheritdoc />
        public ICollection<string> Keys => _headerFields.Select(x => x.Name).ToList();

        /// <inheritdoc />
        public ICollection<string> Values => _headerFields.Select(x => x.Value).ToList();

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            // do not use this method, it will fail with duplicate header fields.
            return _headerFields.ToDictionary(x => x.Name, x => x.Value).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<string, string> item)
        {
            _headerFields.Add(new HeaderField {Name = item.Key.ToLower(), Value = item.Value});
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<string, string> item)
        {
            return _headerFields.Any(x => x.Name == item.Key.ToLower() && x.Value == item.Value);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _headerFields.ToDictionary(x => x.Name, x => x.Value).ToArray().CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<string, string> item)
        {
            var coll = _headerFields.Where(x => x.Name != item.Key.ToLower() && x.Value != item.Value).ToList();
            _headerFields = coll;
            return coll.Any();
        }

        /// <inheritdoc />
        public void Add(string key, string value)
        {
            _headerFields.Add(new HeaderField {Name = key.ToLower(), Value = value});
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            return _headerFields.Any(x => x.Name == key.ToLower());
        }

        /// <inheritdoc />
        public bool Remove(string key)
        {
            var coll = _headerFields.Where(x => x.Name != key.ToLower()).ToList();
            _headerFields = coll;
            return coll.Any();
        }
    }
}