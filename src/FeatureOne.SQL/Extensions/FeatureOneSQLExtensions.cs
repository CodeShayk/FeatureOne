using System;
using FeatureOne.Cache;
using FeatureOne.Json;
using FeatureOne.SQL.StorageProvider;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureOne.SQL.Extensions
{
    /// <summary>
    /// Extension methods for adding FeatureOne services to the DI container
    /// </summary>
    public static class FeatureOneSQLExtensions
    {
        /// <summary>
        /// Add Feature One with SQL storage.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Required: SQL Configuration.</param>
        /// <param name="deserializer">Optional: Custom Deserializer for Toggles. Pass Null to use default.</param>
        /// <param name="cache">Optional: Custom Cache for Toggles. Pass Null to use default memCache.</param>
        /// <returns></returns>
        public static IServiceCollection AddFeatureOneWithSQLStorage(this IServiceCollection services,
            SQLConfiguration configuration, IToggleDeserializer deserializer = null, ICache cache = null)
        {
            if (configuration == null)
                throw new ArgumentNullException("SQLConfiguration is required.");

            return services
                    .AddFeatureOne(provider =>
                    new SQLStorageProvider(repository: new DbRepository(configuration),
                    deserializer: deserializer ?? new ToggleDeserializer(new ConditionDeserializer()),
                    cache: cache ?? new FeatureCache(),
                    cacheSettings: configuration.CacheSettings));
        }
    }
}