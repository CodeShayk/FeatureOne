namespace FeatureOne.SQL
{
    public interface ICache
    {
        /// <summary>
        /// Add item to cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        void Add(string key, object value, int expiry);

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
    }
}