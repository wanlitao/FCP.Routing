using System;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Consistent Hashing node ring implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsistentHash<T>
    {
        private readonly SortedDictionary<int, T> _circle;
        private readonly int _virtualNodesFactor;

        public ConsistentHash(SortedDictionary<int, T> nodeHashCircle, int virtualNodesFactor)
        {
            if (nodeHashCircle == null)
                throw new ArgumentNullException("nodeHashCircle");

            if (virtualNodesFactor < 1)
                throw new ArgumentException("virtualNodesFactor must be >= 1");

            _circle = nodeHashCircle;
            _virtualNodesFactor = virtualNodesFactor;
        }

        /// <summary>
        /// arrays for fast binary search access
        /// </summary>
        private Tuple<int[], T[]> _ring = null;
        private Tuple<int[], T[]> RingTuple
        {
            get { return _ring ?? (_ring = Tuple.Create(_circle.Keys.ToArray(), _circle.Values.ToArray())); }
        }

        /// <summary>
        /// Sorted hash values of the nodes
        /// </summary>
        private int[] NodeHashRing
        {
            get { return RingTuple.Item1; }
        }

        /// <summary>
        /// NodeRing is the nodes sorted in the same order as <see cref="NodeHashRing"/>, i.e. same index
        /// </summary>
        private T[] NodeRing
        {
            get { return RingTuple.Item2; }
        }

        /// <summary>
        /// Add a node to the hash ring.
        /// 
        /// Note that <see cref="ConsistentHash{T}"/> is immutable and
        /// this operation returns a new instance.
        /// </summary>
        public ConsistentHash<T> Add(T node)
        {
            return this + node;
        }

        /// <summary>
        /// Removes a node from the hash ring.
        /// 
        /// Note that <see cref="ConsistentHash{T}"/> is immutable and
        /// this operation returns a new instance.
        /// </summary>
        public ConsistentHash<T> Remove(T node)
        {
            return this - node;
        }

        /// <summary>
        /// Converts the result of <see cref="Array.BinarySearch{T}(T[], T)"/> into an index in the 
        /// <see cref="RingTuple"/> array.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int Idx(int i)
        {
            if (i >= 0)
                return i; //exact match
            else
            {
                var j = Math.Abs(i + 1);
                if (j >= NodeHashRing.Length)
                    return 0; //after last, use first
                else
                    return j; //next node clockwise
            }
        }

        /// <summary>
        /// Get the node responsible for the data key.
        /// Can only be used if nodes exist in the node ring.
        /// Otherwise throws <see cref="ArgumentException"/>.
        /// </summary>
        public T NodeFor(byte[] key)
        {
            if (IsEmpty)
                throw new InvalidOperationException(string.Format("Can't get node for [{0}] from an empty node ring", key));

            return NodeRing[Idx(Array.BinarySearch(NodeHashRing, ConsistentHash.HashFor(key)))];
        }

        /// <summary>
        /// Get the node responsible for the data key.
        /// Can only be used if nodes exist in the node ring.
        /// Otherwise throws <see cref="ArgumentException"/>.
        /// </summary>
        public T NodeFor(string key)
        {
            if (IsEmpty)
                throw new InvalidOperationException(string.Format("Can't get node for [{0}] from an empty node ring", key));

            return NodeRing[Idx(Array.BinarySearch(NodeHashRing, ConsistentHash.HashFor(key)))];
        }

        /// <summary>
        /// Is the node ring empty? i.e. no nodes added or all removed
        /// </summary>
        public bool IsEmpty
        {
            get { return !_circle.Any(); }
        }

        #region Operator overloads
        /// <summary>
        /// Add a node to the hash ring.
        /// 
        /// Note that <see cref="ConsistentHash{T}"/> is immutable and
        /// this operation returns a new instance.
        /// </summary>s
        public static ConsistentHash<T> operator +(ConsistentHash<T> hash, T node)
        {
            var nodeHash = ConsistentHash.HashFor(node.GetHashCode().ToString());
            var vNodeHashCircle = Enumerable.Range(1, hash._virtualNodesFactor)
                .Select(x => new KeyValuePair<int, T>(ConsistentHash.ConcatenateNodeHash(nodeHash, x), node));

            var newNodeHashCircle = new SortedDictionary<int, T>();
            foreach (var item in hash._circle.Concat(vNodeHashCircle))
                newNodeHashCircle.Add(item.Key, item.Value);

            return new ConsistentHash<T>(newNodeHashCircle, hash._virtualNodesFactor);
        }

        /// <summary>
        /// Removes a node from the hash ring.
        /// 
        /// Note that <see cref="ConsistentHash{T}"/> is immutable and
        /// this operation returns a new instance.
        /// </summary>
        public static ConsistentHash<T> operator -(ConsistentHash<T> hash, T node)
        {
            var nodeHash = ConsistentHash.HashFor(node.GetHashCode().ToString());
            var vNodeHashCircle = Enumerable.Range(1, hash._virtualNodesFactor)
                .Select(x => new KeyValuePair<int, T>(ConsistentHash.ConcatenateNodeHash(nodeHash, x), node));

            var newNodeHashCircle = new SortedDictionary<int, T>();
            foreach (var item in hash._circle.Except(vNodeHashCircle))
                newNodeHashCircle.Add(item.Key, item.Value);

            return new ConsistentHash<T>(newNodeHashCircle, hash._virtualNodesFactor);
        }
        #endregion
    }

    /// <summary>
    /// Static helper class for creating <see cref="ConsistentHash{T}"/> instances.
    /// </summary>
    public static class ConsistentHash
    {
        /// <summary>
        /// Factory method to create a <see cref="ConsistentHash{T}"/> instance.
        /// </summary>
        public static ConsistentHash<T> Create<T>(IEnumerable<T> nodes, int virtualNodesFactor)
        {
            var nodeHashCircle = new SortedDictionary<int, T>();
            foreach (var node in nodes)
            {
                var nodeHash = HashFor(node.GetHashCode().ToString());
                var vNodeHashs = Enumerable.Range(1, virtualNodesFactor)
                    .Select(x => ConcatenateNodeHash(nodeHash, x)).ToList();
                foreach (var vNodeHash in vNodeHashs)
                    nodeHashCircle.Add(vNodeHash, node);
            }

            return new ConsistentHash<T>(nodeHashCircle, virtualNodesFactor);
        }

        #region Hashing methods
        internal static int ConcatenateNodeHash(int nodeHash, int vnode)
        {
            unchecked
            {
                var h = MurmurHash.StartHash((uint)nodeHash);
                h = MurmurHash.ExtendHash(h, (uint)vnode, MurmurHash.StartMagicA, MurmurHash.StartMagicB);
                return (int)MurmurHash.FinalizeHash(h);
            }
        }

        internal static int HashFor(string hashKey)
        {
            return MurmurHash.StringHash(hashKey);
        }

        internal static int HashFor(byte[] bytes)
        {
            return MurmurHash.ByteHash(bytes);
        }
        #endregion
    }
}
