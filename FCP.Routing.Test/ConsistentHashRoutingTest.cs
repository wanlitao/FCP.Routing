using Xunit;
using FCP.Routing.LoadBalance;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Routing.Test
{
    public class ConsistentHashRoutingTest
    {
        [Fact]
        public void consistentHashRoute_Performance()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> consistentHashRouting = new ConsistentHashRouting<ServiceInfo>();
            consistentHashRouting.select("abc", serviceArray);
            int routeCount = 10000;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for(var i = 0; i < routeCount; i++)
            {
                consistentHashRouting.select(i.ToString(), serviceArray);
            }
            sw.Stop();

            var avgMicroSeconds = (sw.ElapsedMilliseconds * 1000) / routeCount;
            Assert.True(5 > avgMicroSeconds);                       
        }

        [Fact]
        public void consistentHashRoute_Result()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> consistentHashRouting = new ConsistentHashRouting<ServiceInfo>();

            var routeCountDic = new Dictionary<string, int>();
            foreach (var service in serviceArray)
            {
                routeCountDic.Add(service.Id, 0);
            }

            int routeCount = 10000;
            for (var i = 0; i < routeCount; i++)
            {
                var consistentHashService = consistentHashRouting.select(i.ToString(), serviceArray);
                routeCountDic[consistentHashService.Id] = ++routeCountDic[consistentHashService.Id];
            }

            var routedServiceNum = routeCountDic.Where(m => m.Value > 0).Count();
            Assert.Equal(10, routedServiceNum);
        }

        [Fact]
        public void consistentHashRoute_Result_Remove()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> consistentHashRouting = new ConsistentHashRouting<ServiceInfo>();

            var routeServiceIdList = new SortedList<int, string>();
            int routeCount = 10000;
            for (var i = 0; i < routeCount; i++)
            {
                var consistentHashService = consistentHashRouting.select(i.ToString(), serviceArray);
                routeServiceIdList[i] = consistentHashService.Id;
            }

            //Remove a Service
            serviceArray = serviceArray.Take(3).Concat(serviceArray.Skip(4)).ToArray();
            int hitCount = 0;
            for (var i = 0; i < routeCount; i++)
            {
                var consistentHashService = consistentHashRouting.select(i.ToString(), serviceArray);
                if (string.Compare(routeServiceIdList[i], consistentHashService.Id, true) == 0)
                {
                    hitCount++;
                }
            }

            var hitPercent = (hitCount * 100) / routeCount;
            Assert.True(hitPercent >= 90);
        }

        [Fact]
        public void consistentHashRoute_Result_Add()
        {
            var serviceArray = ServiceInfoHelper.getServiceInfoArray(10);

            IRouting<ServiceInfo> consistentHashRouting = new ConsistentHashRouting<ServiceInfo>();

            var routeServiceIdList = new SortedList<int, string>();
            int routeCount = 10000;
            for (var i = 0; i < routeCount; i++)
            {
                var consistentHashService = consistentHashRouting.select(i.ToString(), serviceArray);
                routeServiceIdList[i] = consistentHashService.Id;
            }

            //Add a Service
            serviceArray = serviceArray.Concat(new ServiceInfo[] { new ServiceInfo { Id = "11" } }).ToArray();
            int hitCount = 0;
            for (var i = 0; i < routeCount; i++)
            {
                var consistentHashService = consistentHashRouting.select(i.ToString(), serviceArray);
                if (string.Compare(routeServiceIdList[i], consistentHashService.Id, true) == 0)
                {
                    hitCount++;
                }
            }

            var hitPercent = (hitCount * 100) / routeCount;
            Assert.True(hitPercent >= 90);
        }
    }
}
