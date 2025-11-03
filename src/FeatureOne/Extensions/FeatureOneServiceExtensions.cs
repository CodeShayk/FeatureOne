using System;
using FeatureOne;
using FeatureOne.Core.Stores;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding FeatureOne services to the DI container
    /// </summary>
    public static class FeatureOneServiceExtensions
    {
        /// <summary>
        /// Adds FeatureOne services to the DI container with the specified storage provider
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="storageProvider">The storage provider implementation</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddFeatureOne(this IServiceCollection services, Func<IServiceProvider, IStorageProvider> storageProviderFactory)
        {
            if (storageProviderFactory == null)
                throw new ArgumentNullException(nameof(storageProviderFactory));

            return services
                .AddSingleton<IStorageProvider>(provider => storageProviderFactory(provider))
                .AddSingleton<IFeatureLogger, DefaultLogger>(provider => new DefaultLogger(provider.GetService<ILogger<IFeatureLogger>>()))
                .AddSingleton<IFeatureStore>(provider => new FeatureStore(provider.GetRequiredService<IStorageProvider>(), provider.GetRequiredService<IFeatureLogger>()))
                .AddSingleton<IFeatures, Features>(provider => new Features(provider.GetRequiredService<IFeatureStore>(), provider.GetRequiredService<IFeatureLogger>()));
        }
    }
}