using System;
using FeatureOne.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureOne.OpenFeature.Extensions
{
    /// <summary>
    /// Extension methods for registering FeatureOne OpenFeature provider in <see cref="IServiceCollection"/>.
    /// </summary>
    public static class FeatureOneServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/> in the dependency injection container and sets it as the OpenFeature global provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="setAsGlobalProvider">Whether to register this provider as OpenFeature.Api.Instance global provider. Defaults to true.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, bool setAsGlobalProvider = true)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<FeatureOneProvider>(sp =>
            {
                var featureStore = sp.GetService<IFeatureStore>();
                var logger = sp.GetService<IFeatureLogger>();
                var provider = new FeatureOneProvider(featureStore, logger);

                if (setAsGlobalProvider)
                {
                    global::OpenFeature.Api.Instance.SetProviderAsync(provider).GetAwaiter().GetResult();
                }

                return provider;
            });

            return services;
        }

        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/> with an explicit <see cref="IFeatureStore"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="featureStore">The explicit FeatureOne feature store instance.</param>
        /// <param name="setAsGlobalProvider">Whether to register this provider as OpenFeature.Api.Instance global provider. Defaults to true.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, IFeatureStore featureStore, bool setAsGlobalProvider = true)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (featureStore == null)
            {
                throw new ArgumentNullException(nameof(featureStore));
            }

            services.TryAddSingleton<FeatureOneProvider>(sp =>
            {
                var logger = sp.GetService<IFeatureLogger>();
                var provider = new FeatureOneProvider(featureStore, logger);

                if (setAsGlobalProvider)
                {
                    global::OpenFeature.Api.Instance.SetProviderAsync(provider).GetAwaiter().GetResult();
                }

                return provider;
            });

            return services;
        }
    }
}
