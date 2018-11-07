using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emceelee.Shared
{
    public class IndexedCollection<TKey, TValue> : ICollection<TValue>
    {
        private readonly IDictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
        private readonly Func<TValue, TKey> key;

        public IndexedCollection(Func<TValue, TKey> key)
        {
            this.key = key;
        }

        public TValue this[TKey key]
        {
            get { return dict[key]; }
        }

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public void Add(TValue item)
        {
            if (dict.ContainsKey(key(item)))
            {
                throw new ArgumentException("Key already exists in collection.");
            }

            dict.Add(key(item), item);
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            foreach(var item in items)
            {
                if(dict.ContainsKey(key(item)))
                {
                    throw new ArgumentException("Key already exists in collection.");
                }
            }
            foreach (var item in items)
            {
                dict.Add(key(item), item);
            }
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(TValue item)
        {
            return dict.Values.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if(array == null)
            {
                throw new ArgumentNullException();
            }
            if(arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            //array length 2 - arrayIndex 0 = 2
            int remaining = array.Length - arrayIndex;
            if(remaining < Count)
            {
                throw new ArgumentException($"The number of elements is greater than the available space from index to the end of the array. ");
            }

            int currentIndex = arrayIndex;
            foreach(var value in dict.Values)
            {
                array[currentIndex++] = value;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        public bool Remove(TValue item)
        {
            bool value = false;
            var keys = dict.Where(p => p.Value.Equals(item)).Select(p => p.Key).ToList();
            foreach(var key in keys)
            {
                dict.Remove(key);
                value = true;
            }

            return value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
