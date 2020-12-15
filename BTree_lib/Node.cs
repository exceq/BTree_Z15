using System.Collections.Generic;


namespace BTree_lib
{
    public class Node<K, V>
    {
        private int degree;

        public Node(int degree)
        {
            this.degree = degree;
            Children = new List<Node<K, V>>(degree);
            Entries = new List<Entry<K, V>>(degree);
        }

        public List<Node<K, V>> Children { get; set; }

        public List<Entry<K, V>> Entries { get; set; }

        public bool IsLeaf
        {
            get
            {
                return Children.Count == 0;
            }
        }

        public bool HasReachedMaxEntries
        {
            get
            {
                return Entries.Count == (2 * degree) - 1;
            }
        }

        public bool HasReachedMinEntries
        {
            get
            {
                return Entries.Count == degree - 1;
            }
        }
    }
}