using System.Collections.Generic;
using System.Linq;

namespace FCP.Routing.Test
{
    public static class ServiceInfoHelper
    {
        public static ServiceInfo[] getServiceInfoArray(int count)
        {
            count = count < 1 ? 10 : count;

            var services = new List<ServiceInfo>();
            services.AddRange(Enumerable.Range(1, count).Select(m => new ServiceInfo { Id = m.ToString() }));
            return services.ToArray();
        }
    }
}
