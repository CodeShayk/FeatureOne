using System;
using System.Runtime.Caching;

namespace FeatureOne.Cache
{
    public static class ExpiryPolicyExtension
    {
        public static CacheItemPolicy GetPolicy(this CacheExpiry expiry)
        {
            return expiry.Type == CacheExpiryType.Absolute
                    ? new CacheItemPolicy { AbsoluteExpiration = new DateTimeOffset(DateTime.Now.Ticks, TimeSpan.FromMinutes(expiry.InMinutes)) }
                    : new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(expiry.InMinutes) };
        }
    }
}