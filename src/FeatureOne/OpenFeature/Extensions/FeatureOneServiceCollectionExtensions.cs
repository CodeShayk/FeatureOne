using System;
using System.Collections.Generic;
using FeatureOne.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OpenFeature;

namespace FeatureOne.OpenFeature.Extensions
{
    /// <summary>
    /// Options controlling how <see cref="FeatureOneProvider"/> is registered.
    /// </summary>
    public class FeatureOneOpenFeatureOptions
    {
        /// <summary>
        /// Whether to register the provider as the OpenFeature <c>Api.Instance</c> global provider on
        /// application start. Defaults to true.
        /// </summary>
        public bool SetAsGlobalProvider { get; set; } = true;

        /// <summary>
        /// Whether to attach <see cref="FeatureOneLoggingHook"/>, routing OpenFeature evaluation
        /// lifecycle events to the registered <see cref="IFeatureLogger"/>. Defaults to false.
        /// Ignored when no <see cref="IFeatureLogger"/> is registered.
        /// </summary>
        public bool EnableLoggingHook { get; set; }

        /// <summary>
        /// Additional provider-level hooks to attach.
        /// </summary>
        public IList<Hook> Hooks { get; } = new List<Hook>();

        /// <summary>
        /// Adds a provider-level hook.
        /// </summary>
        /// <param name="hook">The hook instance.</param>
        /// <returns>The options instance for chaining.</returns>
        public FeatureOneOpenFeatureOptions AddHook(Hook hook)
        {
            if (hook == null)
            {
                throw new ArgumentNullException(nameof(hook));
            }

            Hooks.Add(hook);
            return this;
        }
    }

    /// <summary>
    /// Extension methods for registering <see cref="FeatureOneProvider"/> in <see cref="IServiceCollection"/>.
    /// </summary>
    public static class FeatureOneServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/> in the dependency injection container, resolving the
        /// <see cref="IFeatureStore"/> from the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="setAsGlobalProvider">Whether to register this provider as the OpenFeature global provider on application start. Defaults to true.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, bool setAsGlobalProvider = true)
            => services.AddFeatureOneOpenFeature(options => options.SetAsGlobalProvider = setAsGlobalProvider);

        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/> with an explicit <see cref="IFeatureStore"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="featureStore">The explicit FeatureOne feature store instance.</param>
        /// <param name="setAsGlobalProvider">Whether to register this provider as the OpenFeature global provider on application start. Defaults to true.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, IFeatureStore featureStore, bool setAsGlobalProvider = true)
        {
            if (featureStore == null)
            {
                throw new ArgumentNullException(nameof(featureStore));
            }

            return services.AddFeatureOneOpenFeature(featureStore, options => options.SetAsGlobalProvider = setAsGlobalProvider);
        }

        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/>, resolving the <see cref="IFeatureStore"/> from the
        /// container, with hooks and global-provider behaviour configured via <paramref name="configure"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">Callback to configure registration options.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, Action<FeatureOneOpenFeatureOptions> configure)
            => services.AddFeatureOneOpenFeature(featureStore: null, configure: configure);

        /// <summary>
        /// Registers <see cref="FeatureOneProvider"/> with hooks and global-provider behaviour configured
        /// via <paramref name="configure"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="featureStore">The FeatureOne feature store, or null to resolve it from the container.</param>
        /// <param name="configure">Callback to configure registration options. May be null.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddFeatureOneOpenFeature(this IServiceCollection services, IFeatureStore featureStore, Action<FeatureOneOpenFeatureOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new FeatureOneOpenFeatureOptions();
            configure?.Invoke(options);

            services.TryAddSingleton(sp =>
            {
                var store = featureStore ?? sp.GetService<IFeatureStore>();
                var logger = sp.GetService<IFeatureLogger>();
                var provider = new FeatureOneProvider(store, logger);

                if (options.EnableLoggingHook && logger != null)
                {
                    provider.AddHook(new FeatureOneLoggingHook(logger));
                }

                foreach (var hook in options.Hooks)
                {
                    provider.AddHook(hook);
                }

                return provider;
            });

            if (options.SetAsGlobalProvider)
            {
                // Registering the provider globally is a startup concern. Doing it from a hosted service
                // rather than from the factory above means it happens whether or not anything resolves
                // FeatureOneProvider, and lets the async call be awaited rather than blocked on.
                services.AddSingleton<IHostedService, FeatureOneProviderInitializer>();
            }

            return services;
        }
    }
}
