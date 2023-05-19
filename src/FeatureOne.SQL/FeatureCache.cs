using System;
using System.Runtime.Caching;

namespace FeatureOne.SQL
{
    internal class FeatureCache : ICache
    {
        public void Add(string key, object value, int expiry)
            => MemoryCache.Default.Add(key, value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(expiry) });

        public object Get(string key) => MemoryCache.Default.Get(key);
    }
}