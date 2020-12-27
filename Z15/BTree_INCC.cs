using BTree_lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Z15
{
    public class BTree_INCC<K,V> : INotifyCollectionChanged, IEnumerable<V>
        where K : IComparable<K>
    {
        public BTree<K, V> Tree { get; private set; }
        public BTree_INCC(int Degree)
        {
            Tree = new BTree<K, V>(Degree);
        }

        public int Count => Tree.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public void Delete(K keyToDelete)
        {
            Tree.Delete(keyToDelete);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Insert(K newKey, V newValue)
        {
            Tree.Insert(newKey, newValue);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public Entry<K,V> Search(K newKey)
        {
            return Tree.Search(newKey);
        }

        public bool Contains(K key)
        {
            return Tree.Contains(key);
        }

        public IEnumerator<V> GetEnumerator()
        {
            return ((IEnumerable<V>)Tree).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Tree).GetEnumerator();
        }
    }
}
