using System;

namespace BTree_lib
{
    /// <summary>
    /// Вхождение в вершину, пара ключ-значение.
    /// </summary>
    /// <typeparam name="K">Ключ</typeparam>
    /// <typeparam name="V">Значение</typeparam>
    public class Entry<K, V>
    {
        public K Key { get; set; }

        public V Value { get; set; }

        public bool Equals(Entry<K, V> other)
        {
            return Key.Equals(other.Key) && Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
    }
}
