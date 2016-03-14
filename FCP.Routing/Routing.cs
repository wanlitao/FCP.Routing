namespace FCP.Routing
{
    public abstract class Routing<T> : IRouting<T>
    {
        /// <summary>
        /// pick a instance to receive the message
        /// </summary>
        /// <param name="message">The message that is being routed</param>
        /// <param name="instances">A collection of instance</param>
        /// <returns></returns>
        public virtual T select(object message, params T[] instances)
        {
            if (instances == null || instances.Length == 0)
                return default(T);

            if (instances.Length == 1)
                return instances[0];

            return selectInternal(message, instances);
        }

        /// <summary>
        /// internal select method
        /// </summary>
        /// <param name="message">The message that is being routed</param>
        /// <param name="instances">A collection of instance</param>
        /// <returns></returns>
        protected abstract T selectInternal(object message, T[] instances);
    }
}
