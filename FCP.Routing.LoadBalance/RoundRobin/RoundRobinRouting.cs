using System.Threading;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// RoundRobin Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RoundRobinRouting<T> : Routing<T>
    {
        private int _next;
        
        public RoundRobinRouting()
          : this(-1) 
        { }
        
        public RoundRobinRouting(int next)
        {
            _next = next;
        }
        
        protected override T selectInternal(T[] instances)
        {
            var roundNext = Interlocked.Increment(ref _next) & int.MaxValue;
            return instances[roundNext % instances.Length];
        }
    }
}
