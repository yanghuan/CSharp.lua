using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bridge.ClientTest;
using Bridge.Test.NUnit;

namespace Batch1.src.Collections.Generic
{
    [Category(Constants.PREFIX_COLLECTIONS)]
    [TestFixture(TestNameFormat = "PriorityQueue - {0}")]
    public class PriorityQueueTest
    {
        [Test]
        public void DefaultConstructorWorks()
        {
            var l = new PriorityQueue<int, int>();
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void ConstructorWithCapacityWorks()
        {
            var l = new PriorityQueue<int, int>(12);
            Assert.AreEqual(0, l.Count);
        }

        [Test]
        public void ConstructorWithIComparer()
        {
            IComparer<string> comparer = Comparer<string>.Default;
            var set = new PriorityQueue<string, string>(comparer);
            Assert.AreEqual(comparer, set.Comparer);
        }

        [Test]
        public void CountWorks()
        {
            var q = new PriorityQueue<int, int>();
            Assert.AreEqual(0, q.Count);
            q.Enqueue(1, 100);
            Assert.AreEqual(1, q.Count);
            q.Enqueue(10, 1000);
            Assert.AreEqual(2, q.Count);
        }

        [Test]
        public void EnqueueAndDequeueWork()
        {
            var q = new PriorityQueue<int, int>();
            q.Enqueue(10, 100);
            q.Enqueue(4, 400);
            q.Enqueue(2, 200);
            Assert.AreEqual(10, q.Dequeue());
            Assert.AreEqual(2, q.Dequeue());
            Assert.AreEqual(4, q.Dequeue());
        }

        [Test]
        public void PeekWorks()
        {
            var q = new PriorityQueue<int, int>();
            q.Enqueue(10, 100);
            Assert.AreEqual(10, q.Peek());
            q.Enqueue(2, 200);
            Assert.AreEqual(10, q.Peek());
            q.Dequeue();
            Assert.AreEqual(2, q.Peek());
        }

        [Test]
        public void ClearWorks()
        {
            var q = new PriorityQueue<int, int>();
            q.Enqueue(10, 2);
            q.Enqueue(2, 3);
            q.Enqueue(4, 5);
            q.Clear();
            Assert.AreEqual(0, q.Count);
        }

        [Test]
        public void EnqueueDequeueWorks()
        {
            var q = new PriorityQueue<int, int>(new List<(int, int)>() {
                (1, 20),
                (2, 2),
                (3, 7),
                (4, 11),
                (6, 18),
                (9, 5),
            });
            Assert.AreEqual(6, q.Count);
            Assert.AreEqual(2, q.Peek());
            Assert.AreEqual(2, q.Dequeue());
            int min = q.EnqueueDequeue(7, 13);
            Assert.AreEqual(9, min);
            Assert.AreEqual(3, q.Dequeue());
            Assert.AreEqual(4, q.Dequeue());
            Assert.AreEqual(7, q.Dequeue());
            min = q.EnqueueDequeue(8, 22);
            Assert.AreEqual(6, min);
            Assert.AreEqual(2, q.Count);
        }
    }
}
