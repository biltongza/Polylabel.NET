namespace Polylabel.NET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// .NET reimplementation of TinyQueue <see cref="https://github.com/mourner/tinyqueue/blob/3a212a4f73ad9c39caeb27922c86ff4115e59c66/index.js"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T> 
        where T : IComparable<T>
    {
        private List<T> internalData;
        private Func<T, T, int> comparator;

        public int Length { get; private set; }

        /// <summary>
        /// Initializes a new instance of PriorityQueue.
        /// </summary>
        public PriorityQueue()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of PriorityQueue.
        /// </summary>
        /// <param name="data">Initial data to load into the queue.</param>
        public PriorityQueue(IEnumerable<T> data)
            : this(data, DefaultComparisonFuction)
        {
        }

        /// <summary>
        /// Initializese a new instance of PriorityQueue.
        /// </summary>
        /// <param name="data">Initial data to load into the queue.</param>
        /// <param name="comparator">The comparator fuction used to determine priority.</param>
        public PriorityQueue(IEnumerable<T> data, Func<T, T, int> comparator)
        {
            this.internalData = new List<T>();
            if (data?.Any() ?? false)
            {
                this.internalData.AddRange(data);
            }

            this.Length = this.internalData.Count();
            this.comparator = comparator;

            if (this.Length > 0)
            {
                for (int i = (this.Length >> 1) - 1; i >= 0; i--)
                {
                    this._down(i);
                }
            }
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item">Item to add to the queue.</param>
        public void Enqueue(T item)
        {
            this.internalData.Add(item);
            this.Length++;
            this._up(this.Length - 1);
        }

        /// <summary>
        /// Removes the last item in the queue and returns it.
        /// </summary>
        /// <returns>The last item in the queue.</returns>
        public T Dequeue()
        {
            if (this.Length == 0) return default(T);

            var top = this.internalData[0];
            var bottom = this.internalData[this.internalData.Count - 1];
            this.internalData.RemoveAt(this.internalData.Count - 1);
            this.Length--;

            if (this.Length > 0)
            {
                this.internalData[0] = bottom;
                this._down(0);
            }

            return top;
        }

        /// <summary>
        /// Returns the last item in the queue without removing it.
        /// </summary>
        /// <returns>The last item in the queue.</returns>
        public T Peek()
        {
            return this.internalData[0];
        }

        private void _up(int pos)
        {
            var item = internalData[pos];

            while (pos > 0)
            {
                var parent = (pos - 1) >> 1;
                var current = internalData[parent];
                if (comparator(item, current) >= 0)
                {
                    break;
                }

                internalData[pos] = current;
                pos = parent;
            }

            internalData[pos] = item;
        }

        private void _down(int pos)
        {
            var halfLength = this.Length >> 1;
            var item = internalData[pos];

            while (pos < halfLength)
            {
                var left = (pos << 1) + 1;
                var best = internalData[left];
                var right = left + 1;

                if (right < this.Length && comparator(internalData[right], best) < 0)
                {
                    left = right;
                    best = internalData[right];
                }
                if (comparator(best, item) >= 0)
                {
                    break;
                }

                internalData[pos] = best;
                pos = left;
            }

            internalData[pos] = item;
        }

        private static int DefaultComparisonFuction(T a, T b)
        {
            return a?.CompareTo(b) ?? 0;
        }
    }
}
