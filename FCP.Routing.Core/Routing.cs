namespace FCP.Routing
{
    public abstract class Routing<T> : IRouting<T>
    {
        /// <summary>
        /// pick a instance to receive the message
        /// </summary>
        /// <param name="instances">A collection of instance</param>
        /// <returns></returns>
        public virtual T select(params T[] instances)
        {
            if (instances == null || instances.Length == 0)
                return default(T);

            return selectInternal(instances);
        }

        /// <summary>
        /// internal select method
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        protected abstract T selectInternal(T[] instance);
    }
}
