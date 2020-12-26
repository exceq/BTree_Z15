using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BTree_lib
{
    /// <typeparam name="K">Тип ключа B-дерева.</typeparam>
    /// <typeparam name="V">Тип значения B-дерева связанный с каждым ключем.</typeparam>
    public class BTree<K, V> : IEnumerable <V>
        where K : IComparable<K> 
    {
        public BTree(int degree)
        {
            if (degree < 2)
            {
                throw new ArgumentException("BTree degree must be at least 2", "degree");
            }

            Root = new Node<K, V>(degree);
            Degree = degree;
            Height = 1;
        }

        public Node<K, V> Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Метод ищет ключ в B-дереве, возвращающий Entry.
        /// </summary>
        /// <param name="key">Ключ, который необходимо найти.</param>
        /// <returns>Вхождение Entry, либо null если такого ключа нет.</returns>
        public Entry<K, V> Search(K key)
        {
            return SearchInternal(Root, key);
        }

        /// <summary>
        /// Вставляет новый ключ связанный со значением в B-дерево.
        /// Этот метод разбивает вершины по мере необходимости, чтобы сохранить свойства B-дерева.
        /// </summary>
        /// <param name="newKey">Новый ключ для вставки.</param>
        /// <param name="newValue">Новое значение.</param>
        public void Insert(K newKey, V newValue)
        {
            //list.Add(newValue);
            if (!Root.HasReachedMaxEntries)
                InsertNonFull(Root, newKey, newValue);
            else
            {
                // Создание новой вершины и разделение этой вершины
                var oldRoot = Root;
                Root = new Node<K, V>(Degree);
                Root.Children.Add(oldRoot);
                SplitChild(Root, 0, oldRoot);
                InsertNonFull(Root, newKey, newValue);

                Height++;
            }
        }

        /// <summary>
        /// Удаляет ключ из B-дерева. 
        /// Этот метод разбивает вершины по мере необходимости, чтобы сохранить свойства B-дерева.
        /// </summary>
        /// <param name="keyToDelete">Ключ, который необходимо удалить.</param>
        public void Delete(K keyToDelete)
        {
            //list.Remove(Search(keyToDelete).Value);
            Delete(Root, keyToDelete);
            // если последнее Вхождение корня было перемещено в Вершину потомка, удалить корень
            if (Root.Entries.Count == 0 && !Root.IsLeaf)
            {
                Root = Root.Children.Single();
                Height--;
            }
        }

        /// <summary>
        /// Удаляет ключ из B-дерева. 
        /// </summary>
        /// <param name="node">Вершина, используемая для начала поиска ключа.</param>
        /// <param name="keyToDelete">Ключ, который необходимо удалить.</param>
        private void Delete(Node<K, V> node, K keyToDelete)
        {
            int i = node.Entries.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

            if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(keyToDelete) == 0)
            {
                DeleteKeyFromNode(node, keyToDelete, i);
                return;
            }

            if (!node.IsLeaf)
            {
                DeleteKeyFromSubtree(node, keyToDelete, i);
            }
        }

        /// <summary>
        /// Метод, удаляющий ключ из поддерева.
        /// </summary>
        /// <param name="parentNode">Родительская вершина используемая для начала поиска ключа.</param>
        /// <param name="keyToDelete">Ключ, который необходимо удалить.</param>
        /// <param name="subtreeIndexInNode">Индекс вершины поддерева относительно родительской вершины.</param>
        private void DeleteKeyFromSubtree(Node<K, V> parentNode, K keyToDelete, int subtreeIndexInNode)
        {
            Node<K, V> childNode = parentNode.Children[subtreeIndexInNode];
            if (childNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInNode - 1;
                Node<K, V> leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                Node<K, V> rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;

                if (leftSibling != null && leftSibling.Entries.Count > Degree - 1)
                {
                    childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = leftSibling.Entries.Last();
                    leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Entries.Count > Degree - 1)
                {
                    childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = rightSibling.Entries.First();
                    rightSibling.Entries.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    if (leftSibling != null)
                    {
                        childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                        var oldEntries = childNode.Entries;
                        childNode.Entries = leftSibling.Entries;
                        childNode.Entries.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                        childNode.Entries.AddRange(rightSibling.Entries);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                }
            }

            Delete(childNode, keyToDelete);
        }

        private void DeleteKeyFromNode(Node<K, V> node, K keyToDelete, int keyIndexInNode)
        {
            if (node.IsLeaf)
            {
                node.Entries.RemoveAt(keyIndexInNode);
                return;
            }

            Node<K, V> predecessorChild = node.Children[keyIndexInNode];
            if (predecessorChild.Entries.Count >= Degree)
            {
                Entry<K, V> predecessor = DeletePredecessor(predecessorChild);
                node.Entries[keyIndexInNode] = predecessor;
            }
            else
            {
                Node<K, V> successorChild = node.Children[keyIndexInNode + 1];
                if (successorChild.Entries.Count >= Degree)
                {
                    Entry<K, V> successor = DeleteSuccessor(predecessorChild);
                    node.Entries[keyIndexInNode] = successor;
                }
                else
                {
                    predecessorChild.Entries.Add(node.Entries[keyIndexInNode]);
                    predecessorChild.Entries.AddRange(successorChild.Entries);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    node.Entries.RemoveAt(keyIndexInNode);
                    node.Children.RemoveAt(keyIndexInNode + 1);

                    Delete(predecessorChild, keyToDelete);
                }
            }
        }

        private Entry<K, V> DeletePredecessor(Node<K, V> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Entries[node.Entries.Count - 1];
                node.Entries.RemoveAt(node.Entries.Count - 1);
                return result;
            }

            return DeletePredecessor(node.Children.Last());
        }

        private Entry<K, V> DeleteSuccessor(Node<K, V> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Entries[0];
                node.Entries.RemoveAt(0);
                return result;
            }

            return DeletePredecessor(node.Children.First());
        }

        private Entry<K, V> SearchInternal(Node<K, V> node, K key)
        {
            int i = node.Entries.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

            if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(key) == 0)
            {
                return node.Entries[i];
            }

            return node.IsLeaf ? null : SearchInternal(node.Children[i], key);
        }

        private void SplitChild(Node<K, V> parentNode, int nodeToBeSplitIndex, Node<K, V> nodeToBeSplit)
        {
            var newNode = new Node<K, V>(Degree);

            parentNode.Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[Degree - 1]);
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(Degree, Degree - 1));

            nodeToBeSplit.Entries.RemoveRange(Degree - 1, Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(Degree, Degree));
                nodeToBeSplit.Children.RemoveRange(Degree, Degree);
            }
        }

        private void InsertNonFull(Node<K, V> node, K newKey, V newValue)
        {
            int positionToInsert = node.Entries.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();

            if (node.IsLeaf)
            {
                node.Entries.Insert(positionToInsert, new Entry<K, V>() { Key = newKey, Value = newValue });
                return;
            }

            Node<K, V> child = node.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                SplitChild(node, positionToInsert, child);
                if (newKey.CompareTo(node.Entries[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }

            InsertNonFull(node.Children[positionToInsert], newKey, newValue);
        }

        public bool Contains(K id)
        {
            return Search(id) != null;
        }


        List<V> list = new List<V>(); 
        void Num(Node<K, V> node, List<V> list)
        {
            if (node.Children.Count() != 0)
                foreach (var c in node.Children)
                    Num(c, list);
            foreach (var e in node.Entries)
                list.Add(e.Value);
        }

        public IEnumerator<V> GetEnumerator()
        {
            list = new List<V>();
            Num(Root, list);
            foreach (var e in list)
                yield return e;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }    
}