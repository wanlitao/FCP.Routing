namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Random Select
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomRouting<T> : Routing<T>
    {
        protected override T selectInternal(T[] instance)
        {
            return instance[ThreadLocalRandom.Current.Next(instance.Length - 1) % instance.Length];
        }
    }
}
