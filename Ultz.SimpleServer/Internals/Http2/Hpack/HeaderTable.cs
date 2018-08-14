#region

using System;

#endregion

namespace Ultz.SimpleServer.Internals.Http2.Hpack
{
    /// <summary>
    ///     The combination of the static and a dynamic header table
    /// </summary>
    public class HeaderTable
    {
        private readonly DynamicTable dynamic;

        public HeaderTable(int dynamicTableSize)
        {
            dynamic = new DynamicTable(dynamicTableSize);
        }

        public int MaxDynamicTableSize
        {
            get => dynamic.MaxTableSize;
            set => dynamic.MaxTableSize = value;
        }

        /// <summary>Gets the occupied size in bytes for the dynamic table</summary>
        public int UsedDynamicTableSize => dynamic.UsedSize;

        /// <summary>Gets the current length of the dynamic table</summary>
        public int DynamicTableLength => dynamic.Length;

        /// <summary>
        ///     Inserts a new element into the header table
        /// </summary>
        public bool Insert(string name, int nameBytes, string value, int valueBytes)
        {
            return dynamic.Insert(name, nameBytes, value, valueBytes);
        }

        public TableEntry GetAt(int index)
        {
            // 0 is not a valid index
            if (index < 1) throw new IndexOutOfRangeException();

            // Relate index to start of static table
            // and look if element is in there
            index--;
            if (index < StaticTable.Entries.Length) return StaticTable.Entries[index];

            // Relate index to start of dynamic table
            // and look if element is in there
            index -= StaticTable.Entries.Length;
            if (index < dynamic.Length) return dynamic.GetAt(index);

            // Element is not in static or dynamic table
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        ///     Returns the index of the best matching element in the header table.
        ///     If no index was found the return value is -1.
        ///     If an index was found and the name as well as the value match
        ///     isFullMatch will be set to true.
        /// </summary>
        public int GetBestMatchingIndex(HeaderField field, out bool isFullMatch)
        {
            var bestMatch = -1;
            isFullMatch = false;

            var i = 1;
            foreach (var entry in StaticTable.Entries)
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

            // If we don't have a full match search on in the dynamic table
            bool dynamicHasFullMatch;
            var di = dynamic.GetBestMatchingIndex(field, out dynamicHasFullMatch);
            if (dynamicHasFullMatch)
            {
                isFullMatch = true;
                return di + 1 + StaticTable.Length;
            }

            // If the dynamic table has a match at all use it's index and normalize it
            if (di != -1 && bestMatch == -1) bestMatch = di + 1 + StaticTable.Length;

            return bestMatch;
        }
    }
}