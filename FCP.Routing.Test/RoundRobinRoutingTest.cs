using FCP.Routing.LoadBalance;
using System.Diagnostics;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Routing.Test
{
    public class RoundRobinRoutingTest
    {
        [Fact]
        public void roundRobinRoute_Performance()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> roundRobinRouting = new RoundRobinRouting<ServiceInfo>();
            int routeCount = 10000;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < routeCount; i++)
            {
                roundRobinRouting.select(null, serviceArray);
            }
            sw.Stop();

            var avgMicroSeconds = (sw.ElapsedMilliseconds * 1000) / routeCount;
            Assert.True(5 > avgMicroSeconds);
        }

        [Fact]
        public void roundRobinRoute_Result()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> roundRobinRouting = new RoundRobinRouting<ServiceInfo>();            

            var routeCountDic = new Dictionary<string, int>();
            foreach(var service in serviceArray)
            {
                routeCountDic.Add(service.Id, 0);
            }

            int routeCount = 10000;
            for (var i = 0; i < routeCount; i++)
            {
                var roundRobinService = roundRobinRouting.select(null, serviceArray);                
                routeCountDic[roundRobinService.Id] = ++routeCountDic[roundRobinService.Id];
            }

            Assert.Equal(1000, routeCountDic.Select(m => m.Value).Distinct().Single());
        }
    }
}
