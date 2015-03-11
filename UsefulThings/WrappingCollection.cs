using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulThings
{
    public class WrappingCollection<T> : ICollection<T>, IList<T>
    {
        List<T> UnderlyingCollection = null;

        public int Count
        {
            get
            {
                return UnderlyingCollection != null ? UnderlyingCollection.Count : -1;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public WrappingCollection()
        {
            UnderlyingCollection = new List<T>();
        }

        public WrappingCollection(IEnumerable<T> enumerable) : this()
        {
            UnderlyingCollection.AddRange(enumerable);
        }

        public WrappingCollection(ICollection<T> collection) : this()
        {
            UnderlyingCollection.AddRange(collection);
        }


        public void Add(T item)
        {
            UnderlyingCollection.Add(item);
        }

        public void Clear()
        {
            UnderlyingCollection.Clear();
        }

        public bool Contains(T item)
        {
            return UnderlyingCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            UnderlyingCollection.CopyTo(array, arrayIndex);
        }

        

        public bool Remove(T item)
        {
            return UnderlyingCollection.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return UnderlyingCollection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return UnderlyingCollection.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return UnderlyingCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            UnderlyingCollection.Insert(WrapIndex(index), item);
        }

        public void RemoveAt(int index)
        {
            UnderlyingCollection.RemoveAt(WrapIndex(index));
        }

        public T this[int index]
        {
            get
            {
                return UnderlyingCollection[WrapIndex(index)];
            }
            set
            {
                UnderlyingCollection[WrapIndex(index)] = value;
            }
        }

        private int WrapIndex(int index)
        {
            if (UnderlyingCollection.Count != 0)
                return index % UnderlyingCollection.Count;
            else
                return -1;
        }
    }
}
