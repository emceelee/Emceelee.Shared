using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Emceelee.Shared.Test
{
    [TestClass]
    public class IndexedCollectionTest
    {
        private static List<TestObject> _objects;

        [ClassInitialize]
        public static void ClassInit(TestContext tx)
        {
            _objects = new List<TestObject>();
            for(int i = 0; i < 1000; ++i)
            {
                _objects.Add(new TestObject() { Sequence = i, Name = $"Test{i}" });
            }
        }

        [TestMethod]
        public void IndexedCollection_Add()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.Add(_objects[0]);
            collection.Add(_objects[1]);

            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual("Test0", collection[0].Name);
            Assert.AreEqual("Test1", collection[1].Name);
        }

        [TestMethod]
        public void IndexedCollection_Clear()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.Add(_objects[0]);
            collection.Add(_objects[1]);

            Assert.AreEqual(2, collection.Count);

            collection.Clear();
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IndexedCollection_AddDuplicate()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.Add(_objects[0]);
            collection.Add(_objects[0]);
        }

        [TestMethod]
        public void IndexedCollection_AddRange()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects);

            Assert.AreEqual(1000, collection.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IndexedCollection_AddRange_Duplicate()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects);
            collection.AddRange(_objects.GetRange(0,1));
        }

        [TestMethod]
        public void IndexedCollection_IsReadyOnly()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);

            Assert.AreEqual(false, collection.IsReadOnly);
        }

        [TestMethod]
        public void IndexedCollection_Contains()
        {
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.Add(_objects[0]);

            Assert.AreEqual(true, collection.Contains(_objects[0]));
            Assert.AreEqual(false, collection.Contains(_objects[1]));
        }

        [TestMethod]
        public void IndexedCollection_CopyToSuccess()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            var array = new TestObject[count];
            collection.CopyTo(array, 0);

            for (int i = 0; i < array.Length; ++i)
            {
                var item = array[i];
                Assert.IsNotNull(item);
                Assert.AreEqual(i, item.Sequence);
                Assert.AreEqual($"Test{i}", item.Name);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IndexedCollection_CopyToNullArray()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            TestObject[] array = null;
            collection.CopyTo(array, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexedCollection_CopyToOutOfRange()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            var array = new TestObject[count];
            collection.CopyTo(array, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void IndexedCollection_CopyToNotEnoughSpace()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            var array = new TestObject[count];
            collection.CopyTo(array, 1);
        }

        [TestMethod]
        public void IndexedCollection_Enumerator()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            IEnumerator enumerator = ((IEnumerable)collection).GetEnumerator();

            Assert.IsNotNull(enumerator);

            int iterations = 0;
            while (enumerator.MoveNext())
            {
                var obj = (TestObject)enumerator.Current;
                ++iterations;
            }
            Assert.AreEqual(count, iterations);
        }

        [TestMethod]
        public void IndexedCollection_EnumeratorGeneric()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            IEnumerator<TestObject> enumerator = collection.GetEnumerator();

            Assert.IsNotNull(enumerator);

            int iterations = 0;
            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                ++iterations;
            }
            Assert.AreEqual(count, iterations);
        }

        [TestMethod]
        public void IndexedCollection_Remove()
        {
            int count = 10;
            var collection = new IndexedCollection<int, TestObject>(t => t.Sequence);
            collection.AddRange(_objects.GetRange(0, count));

            Assert.AreEqual(true, collection.Remove(_objects[9]));
            Assert.AreEqual(false, collection.Remove(_objects[9]));
            Assert.AreEqual(count - 1, collection.Count);
        }
    }

    public class TestObject
    {
        public int Sequence { get; set; }
        public string Name { get; set; }
    }
}
