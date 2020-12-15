using System;
using System.Linq;
using NUnit.Framework;

namespace BTree_lib.Test
{
    [TestFixture]
    public class BTreeTest
    {
        private const int Degree = 2;

        private readonly int[] testKeyData = new int[] { 10, 20, 30, 50 };
        private readonly int[] testPointerData = new int[] { 50, 60, 40, 20 };

        [Test]
        public void CreateBTree()
        {
            var btree = new BTree<int, int>(Degree);

            Node<int, int> root = btree.Root;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.Entries);
            Assert.IsNotNull(root.Children);
            Assert.AreEqual(0, root.Entries.Count);
            Assert.AreEqual(0, root.Children.Count);
        }

        [Test]
        public void InsertOneNode()
        {
            var btree = new BTree<int, int>(Degree);
            InsertTestDataAndValidateTree(btree, 0);
            Assert.AreEqual(1, btree.Height);
        }

        [Test]
        public void InsertMultipleNodesToSplit()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                InsertTestDataAndValidateTree(btree, i);
            }

            Assert.AreEqual(2, btree.Height);
        }

        [Test]
        public void DeleteNodes()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
            }

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                btree.Delete(this.testKeyData[i]);
                TreeValidation.ValidateTree(btree.Root, Degree, this.testKeyData.Skip(i + 1).ToArray());
            }

            Assert.AreEqual(1, btree.Height);
        }

        [Test]
        public void DeleteNonExistingNode()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
            }

            btree.Delete(99999);
            TreeValidation.ValidateTree(btree.Root, Degree, this.testKeyData.ToArray());
        }

        [Test]
        public void SearchNodes()
        {
            var btree = new BTree<int, int>(Degree);

            for (int i = 0; i < this.testKeyData.Length; i++)
            {
                this.InsertTestData(btree, i);
                this.SearchTestData(btree, i);
            }
        }

        [Test]
        public void SearchNonExistingNode()
        {
            var btree = new BTree<int, int>(Degree);

            // search an empty tree
            Entry<int, int> nonExisting = btree.Search(9999);
            Assert.IsNull(nonExisting);

            for (int i = 0; i < testKeyData.Length; i++)
            {
                InsertTestData(btree, i);
                SearchTestData(btree, i);
            }

            // search a populated tree
            nonExisting = btree.Search(9999);
            Assert.IsNull(nonExisting);
        }

        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        public void CheckCorrectRootEntriesLength(int n)
        {
            Random rnd = new Random();
            var btree = new BTree<int, int>(4);
            for (int i = 0; i < n; i++)
            {
                btree.Insert(rnd.Next(1000), rnd.Next(1000000));
            }
            var rootEntryLengthLessMaxCountOfEntry = btree.Root.Entries.Count() <= btree.Degree * 2 - 1;
            Assert.IsTrue(rootEntryLengthLessMaxCountOfEntry);

        }

        private void InsertTestData(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(testKeyData[testDataIndex], testPointerData[testDataIndex]);
        }

        private void InsertTestDataAndValidateTree(BTree<int, int> btree, int testDataIndex)
        {
            btree.Insert(testKeyData[testDataIndex], testPointerData[testDataIndex]);
            TreeValidation.ValidateTree(btree.Root, Degree, testKeyData.Take(testDataIndex + 1).ToArray());
        }

        private void SearchTestData(BTree<int, int> btree, int testKeyDataIndex)
        {
            for (int i = 0; i <= testKeyDataIndex; i++)
            {
                Entry<int, int> entry = btree.Search(testKeyData[i]);
                Assert.IsNotNull(testKeyData[i]);
                Assert.AreEqual(testKeyData[i], entry.Key);
                Assert.AreEqual(testPointerData[i], entry.Value);
            }
        }
    }
}