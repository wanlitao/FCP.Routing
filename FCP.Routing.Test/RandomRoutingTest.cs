using FCP.Routing.LoadBalance;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using System.Linq;

namespace FCP.Routing.Test
{
    public class RandomRoutingTest
    {
        [Fact]
        public void randomRoute_Performance()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> randomRouting = new RandomRouting<ServiceInfo>();
            int routeCount = 10000;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < routeCount; i++)
            {
                randomRouting.select(null, serviceArray);
            }
            sw.Stop();

            var avgMicroSeconds = (sw.ElapsedMilliseconds * 1000) / routeCount;
            Assert.True(5 > avgMicroSeconds);
        }

        [Fact]
        public void randomRoute_Result()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> randomRouting = new RandomRouting<ServiceInfo>();

            var routeCountDic = new Dictionary<string, int>();
            foreach (var service in serviceArray)
            {
                routeCountDic.Add(service.Id, 0);
            }

            int routeCount = 10000;
            for (var i = 0; i < routeCount; i++)
            {
                var randomService = randomRouting.select(null, serviceArray);
                routeCountDic[randomService.Id] = ++routeCountDic[randomService.Id];
            }

            var routedServiceNum = routeCountDic.Where(m => m.Value > 0).Count();
            Assert.Equal(10, routedServiceNum);
        }
    }
}
