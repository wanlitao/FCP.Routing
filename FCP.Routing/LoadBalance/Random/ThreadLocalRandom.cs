﻿using System;
using System.Threading;

namespace FCP.Routing.LoadBalance
{
    /// <summary>
    /// Create random numbers with Thread-specific seeds.
    /// 
    /// Borrowed form Jon Skeet's brilliant C# in Depth: http://csharpindepth.com/Articles/Chapter12/Random.aspx
    /// </summary>
    public static class ThreadLocalRandom
    {
        private static int _seed = Environment.TickCount;

        private static ThreadLocal<Random> _randomWrapper = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        /// <summary>
        /// The current random number seed available to this thread
        /// </summary>
        public static Random Current
        {
            get
            {
                return _randomWrapper.Value;
            }
        }
    }
}
