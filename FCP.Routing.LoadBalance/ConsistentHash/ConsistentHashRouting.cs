using System;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Consistent Hash Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsistentHashRouting<T> : Routing<T>
    {
        private readonly int _vnodes;

        public ConsistentHashRouting()
            : this(0)
        { }

        public ConsistentHashRouting(int virtualNodesFactor)
        {
            _vnodes = virtualNodesFactor < 1 ? LoadBalanceConstants.DefaultVirtualNodesFactor : virtualNodesFactor;
        }

        protected override T selectInternal(T[] instances)
        {
            throw new NotImplementedException();
        }
    }
}
