using System.Runtime.Caching;

namespace FeatureOne.Cache
{
    /// <summary>
    /// Implement to provide cache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Add item to cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        void Add(string key, object value, CacheItemPolicy policy);

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
    }
}