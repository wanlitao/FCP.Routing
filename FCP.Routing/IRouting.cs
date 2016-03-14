namespace FCP.Routing
{
    public interface IRouting<T>
    {
        /// <summary>
        /// pick a instance to receive the message
        /// </summary>
        /// <param name="message">The message that is being routed</param>
        /// <param name="instances">A collection of instance</param>
        /// <returns></returns>
        T select(object message, params T[] instances);
    }
}
