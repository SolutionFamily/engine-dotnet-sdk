using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SolutionFamily
{
    public class DataItemCollection : IEnumerable<DataItem>
    {
        private ReadOnlyDictionary<string, DataItem> Items { get; }

        internal DataItemCollection(Dictionary<string, DataItem> values)
        {
            Items = new ReadOnlyDictionary<string, DataItem>(values);
        }

        public IEnumerator<DataItem> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string id)
        {
            return Items.ContainsKey(id);
        }

        public int Count
        {
            get => Items.Count;
        }

        public DataItem this[string id]
        {
            get => Items[id];
        }
    }
}
