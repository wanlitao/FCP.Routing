using System;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// RoundRobin Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RoundRobinRouting<T> : Routing<T>
    {
        protected override T selectInternal(T[] instance)
        {
            throw new NotImplementedException();
        }
    }
}
