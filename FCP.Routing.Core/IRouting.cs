namespace FCP.Routing
{
    public interface IRouting<T>
    {
        /// <summary>
        /// pick a instance to receive the message
        /// </summary>
        /// <param name="instances">A collection of instance</param>
        /// <returns></returns>
        T select(params T[] instances);
    }
}
