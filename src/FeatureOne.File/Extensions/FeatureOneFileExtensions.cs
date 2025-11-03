using System;
using FeatureOne.Cache;
using FeatureOne.File.StorageProvider;
using FeatureOne.Json;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureOne.File.Extensions
{
    /// <summary>
    /// Extension methods for adding FeatureOne services to the DI container
    /// </summary>
    public static class FeatureOneFileExtensions
    {
        /// <summary>
        /// Add Feature One with File storage.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Required: Configuration.</param>
        /// <param name="deserializer">Optional: Custom Deserializer for Toggles. Pass Null to use default.</param>
        /// <param name="cache">Optional: Custom Cache for Toggles. Pass Null to use default memCache.</param>
        /// <returns></returns>
        public static IServiceCollection AddFeatureOneWithFileStorage(this IServiceCollection services,
            FileConfiguration configuration, IToggleDeserializer deserializer = null, ICache cache = null)
        {
            if (configuration == null)
                throw new ArgumentNullException("FileConfiguration is required.");

            return services
                    .AddFeatureOne(provider=>
                    new FileStorageProvider(configuration,
                    new FileReader(configuration),
                    deserializer?? new ToggleDeserializer(new ConditionDeserializer()),
                    cache ?? new FeatureCache()));
        }
    }
}