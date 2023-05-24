namespace FeatureOne.Cache
{
    /// <summary>
    /// Cache settings for storage provider.
    /// </summary>
    public class CacheSettings
    {
        public CacheSettings()
        {
            Expiry = new CacheExpiry();
        }

        /// <summary>
        /// Use cache when true, default is false for no cache.
        /// </summary>
        public bool EnableCache { get; set; } = false;

        public CacheExpiry Expiry { get; set; }
    }

    public class CacheExpiry
    {
        /// <summary>
        /// Cache item expiry value in minutes.
        /// </summary>
        public int InMinutes { get; set; } = 60;

        public CacheExpiryType Type { get; set; } = CacheExpiryType.Absolute;
    }

    public enum CacheExpiryType
    {
        Absolute,
        Sliding
    }
}