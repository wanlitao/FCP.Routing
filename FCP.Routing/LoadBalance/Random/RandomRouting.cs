namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Random Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomRouting<T> : Routing<T>
    {
        protected override T selectInternal(object message, T[] instances)
        {
            var randNext = ThreadLocalRandom.Current.Next(instances.Length - 1);
            return instances[randNext % instances.Length];
        }
    }
}
