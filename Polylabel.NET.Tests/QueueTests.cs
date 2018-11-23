namespace Polylabel.NET.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class QueueTests
    {
        private static double[] queueData = new double[100];
        private static double[] sorted = new double[queueData.Length];

        static QueueTests()
        {
            var random = new Random();
            for (int i = 0; i < queueData.Length; i++)
            {
                queueData[i] = Math.Floor(100d * random.Next());
            }

            Array.Copy(queueData, sorted, queueData.Length);
            Array.Sort(sorted);
        }

        [Test]
        public void MaintainsAPriorityQueue()
        {
            var queue = new PriorityQueue<double>();

            for (int i = 0; i < queueData.Length; i++)
            {
                queue.Enqueue(queueData[i]);
            }

            Assert.AreEqual(sorted[0], queue.Peek());

            var result = new List<double>();
            while (queue.Length > 0)
            {
                result.Add(queue.Dequeue());
            }

            Assert.AreEqual(sorted, result);
        }

        [Test]
        public void AcceptsDataInConstructor()
        {
            var queue = new PriorityQueue<double>(queueData);

            var result = new List<double>();
            while (queue.Length > 0)
            {
                result.Add(queue.Dequeue());
            }

            Assert.AreEqual(sorted, result);
        }

        [Test]
        public void HandlesEdgeCasesWithFewElements()
        {
            var queue = new PriorityQueue<double>();

            queue.Enqueue(2d);
            queue.Enqueue(1d);
            queue.Dequeue();
            queue.Dequeue();
            queue.Dequeue();
            queue.Enqueue(2d);
            queue.Enqueue(1d);

            Assert.AreEqual(1d, queue.Dequeue());
            Assert.AreEqual(2d, queue.Dequeue());
            Assert.AreEqual(default(double), queue.Dequeue());
        }

        [Test]
        public void HandlesInitWithEmptyArray()
        {
            var queue = new PriorityQueue<double>();

            var result = new List<double>();
            while(queue.Length > 0)
            {
                result.Add(queue.Dequeue());
            }

            Assert.AreEqual(new double[0], result);
        }
    }
}
