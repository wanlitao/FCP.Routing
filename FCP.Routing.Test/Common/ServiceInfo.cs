namespace FCP.Routing.Test
{
    public class ServiceInfo
    {
        public string Id { get; set; }

        public override int GetHashCode()
        {
            return ("service_" + Id).GetHashCode();
        }
    }
}
