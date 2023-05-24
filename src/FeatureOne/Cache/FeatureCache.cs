using System;
using System.Runtime.Caching;

namespace FeatureOne.Cache
{
    public class FeatureCache : ICache
    {
        public void Add(string key, object value, CacheItemPolicy policy)
            => MemoryCache.Default.Add(key, value, policy);

        public object Get(string key) => MemoryCache.Default.Get(key);
    }
}