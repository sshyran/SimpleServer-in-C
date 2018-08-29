using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Http2.Hpack;

namespace Ultz.SimpleServer.Internals.Http
{
    public class HttpHeaderCollection : ICollection<HeaderField>, IDictionary<string,string>
    {
        private List<HeaderField> _headerFields;

        public bool TryGetValue(string key, out string value)
        {
            value = _headerFields.FirstOrDefault(x => x.Name == key.ToLower()).Value;
            return value != null;
        }

        public string this[string key]
        {
            get { return _headerFields.FirstOrDefault(x => x.Name == key.ToLower()).Value; }
            set
            {
                if (!ContainsKey(key))
                {
                    Add(key,value);
                    return;
                }

                // ReSharper disable once NotAccessedVariable
                var headerField =
                    _headerFields[_headerFields.IndexOf(_headerFields.FirstOrDefault(x => x.Name == key.ToLower()))];
                headerField.Value =
                    value;
            }
        }

        public ICollection<string> Keys => _headerFields.Select(x => x.Name).ToList();
        public ICollection<string> Values  => _headerFields.Select(x => x.Value).ToList();

        public HttpHeaderCollection(IEnumerable<HeaderField> fields)
        {
            _headerFields = new List<HeaderField>();
            _headerFields.AddRange(fields);
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            // do not use this method, it will fail with duplicate header fields.
            return _headerFields.ToDictionary(x => x.Name, x => x.Value).GetEnumerator();
        }

        public IEnumerator<HeaderField> GetEnumerator()
        {
            return _headerFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(HeaderField item)
        {
            _headerFields.Add(item);
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _headerFields.Add(new HeaderField(){Name = item.Key.ToLower(), Value = item.Value});
        }

        public void Clear()
        {
            _headerFields.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _headerFields.Any(x => x.Name == item.Key.ToLower() && x.Value == item.Value);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _headerFields.ToDictionary(x => x.Name, x=> x.Value).ToArray().CopyTo(array,arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            var coll = _headerFields.Where(x => x.Name != item.Key.ToLower() && x.Value != item.Value).ToList();
            _headerFields = coll;
            return coll.Any();
        }

        public bool Contains(HeaderField item)
        {
            return _headerFields.Contains(item);
        }

        public void CopyTo(HeaderField[] array, int arrayIndex)
        {
            _headerFields.CopyTo(array, arrayIndex);
        }

        public bool Remove(HeaderField item)
        {
            return _headerFields.Remove(item);
        }

        public void Add(string key, string value)
        {
            _headerFields.Add(new HeaderField(){Name = key.ToLower(),Value = value});
        }

        public bool ContainsKey(string key)
        {
            return _headerFields.Any(x => x.Name == key.ToLower());
        }

        public bool Remove(string key)
        {
            var coll = _headerFields.Where(x => x.Name != key.ToLower()).ToList();
            _headerFields = coll;
            return coll.Any();
        }

        public int Count => _headerFields.Count;
        public bool IsReadOnly => false;
    }
}