using System;
using System.Linq;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Consistent Hash Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsistentHashRouting<T> : Routing<T>
    {
        private Tuple<T[], ConsistentHash<T>> _cacheConsistentHashTuple;
        private readonly int _vnodes;

        public ConsistentHashRouting()
            : this(0)
        { }

        public ConsistentHashRouting(int virtualNodesFactor)
        {
            _vnodes = virtualNodesFactor < 1 ? LoadBalanceConstants.DefaultVirtualNodesFactor : virtualNodesFactor;
        }

        protected override T selectInternal(object message, T[] instances)
        {
            if (message == null)
                return default(T);

            string messageHash = message.GetHashCode().ToString();
            var consistentHashCircle = getInstanceConsistentHashCircle(instances);

            return consistentHashCircle.NodeFor(messageHash);
        }

        /// <summary>
        /// Create instance Consistent Hash Circle, if instance no change will direct read the cache
        /// </summary>
        /// <param name="instances"></param>
        /// <returns></returns>
        private ConsistentHash<T> getInstanceConsistentHashCircle(T[] instances)
        {
            if (_cacheConsistentHashTuple == null || !_cacheConsistentHashTuple.Item1.SequenceEqual(instances))
            {
                _cacheConsistentHashTuple = new Tuple<T[], ConsistentHash<T>>(instances, ConsistentHash.Create<T>(instances, _vnodes));
            }
            return _cacheConsistentHashTuple.Item2;
        }
    }
}
