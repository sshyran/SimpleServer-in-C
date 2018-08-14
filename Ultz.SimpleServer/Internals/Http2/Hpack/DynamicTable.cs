#region

using System;
using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Internals.Http2.Hpack
{
    /// <summary>
    ///     A dynamic header table
    /// </summary>
    public class DynamicTable
    {
        private readonly List<TableEntry> _entries = new List<TableEntry>();

        /// <summary> The maximum size of the dynamic table</summary>
        private int _maxTableSize;

        /// <summary> The current used amount of bytes for the table</summary>
        private int _usedSize;

        public DynamicTable(int maxTableSize)
        {
            _maxTableSize = maxTableSize;
        }

        /// <summary> Get the current maximum size of the dynamic table</summary>
        public int MaxTableSize
        {
            get => _maxTableSize;
            /// <summary>
            /// Sets a new maximum size of the dynamic table.
            /// The content will be evicted to fit into the new size.
            /// </summary>
            set
            {
                if (value >= _maxTableSize)
                {
                    _maxTableSize = value;
                    return;
                }

                // Table is shrinked, which means entries must be evicted
                _maxTableSize = value;
                EvictTo(value);
            }
        }

        /// <summary>The size that is currently occupied by the table</summary>
        public int UsedSize => _usedSize;

        /// <summary>
        ///     Get the current length of the dynamic table
        /// </summary>
        public int Length => _entries.Count;

        public TableEntry GetAt(int index)
        {
            if (index < 0 || index >= _entries.Count)
                throw new IndexOutOfRangeException();
            var elem = _entries[index];
            return elem;
        }

        private void EvictTo(int newSize)
        {
            if (newSize < 0) newSize = 0;
            // Delete as many entries as needed to conform to the new size
            // Start by counting how many entries need to be deleted
            var delCount = 0;
            var used = _usedSize;
            var index = _entries.Count - 1; // Start at end of the table
            while (used > newSize && index >= 0)
            {
                var item = _entries[index];
                used -= 32 + item.NameLen + item.ValueLen;
                index--;
                delCount++;
            }

            if (delCount == 0) return;

            if (delCount == _entries.Count)
            {
                _entries.Clear();
                _usedSize = 0;
            }
            else
            {
                _entries.RemoveRange(_entries.Count - delCount, delCount);
                _usedSize = used;
            }
        }

        /// <summary>
        ///     Inserts a new element into the dynamic header table
        /// </summary>
        public bool Insert(string name, int nameBytes, string value, int valueBytes)
        {
            // Calculate the size that this dynamic table entry occupies according to the spec
            var entrySize = 32 + nameBytes + valueBytes;

            // Evict the dynamic table to have enough space for new entry - or to 0
            var maxUsedSize = _maxTableSize - entrySize;
            if (maxUsedSize < 0) maxUsedSize = 0;
            EvictTo(maxUsedSize);

            // Return if entry doesn't fit into table
            if (entrySize > _maxTableSize) return false;

            // Add the new entry at the beginning of the table
            var entry = new TableEntry
            {
                Name = name,
                NameLen = nameBytes,
                Value = value,
                ValueLen = valueBytes
            };
            _entries.Insert(0, entry);
            _usedSize += entrySize;
            return true;
        }

        /// <summary>
        ///     Returns the index of the best matching element in the dynamic table.
        ///     The index will be 0-based, means it is relative to the start of the
        ///     dynamic table.
        ///     If no index was found the return value is -1.
        ///     If an index was found and the name as well as the value match
        ///     isFullMatch will be set to true.
        /// </summary>
        public int GetBestMatchingIndex(HeaderField field, out bool isFullMatch)
        {
            var bestMatch = -1;
            isFullMatch = false;

            var i = 0;
            foreach (var entry in _entries)
            {
                if (entry.Name == field.Name)
                {
                    if (bestMatch == -1) bestMatch = i;

                    if (entry.Value == field.Value)
                    {
                        // It's a perfect match!
                        isFullMatch = true;
                        return i;
                    }
                }

                i++;
            }

            return bestMatch;
        }
    }
}