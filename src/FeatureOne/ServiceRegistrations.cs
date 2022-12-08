using FeatureOne.Core.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureOne
{
    public static class ServiceRegistrations
    {
        public static void UseFeatureOne(this ServiceCollection services, Configuration configuration)
        {
            if (configuration?.Logger == null)
                services.AddTransient<IFeatureLogger, NullLogger>();

            if (configuration?.StoreProvider == null)
                services.AddTransient<IStoreProvider, NullStoreProvder>();

            services.AddTransient<IToggleDeserializer, ToggleDeserializer>();
            services.AddTransient<IStoreProvider>(c => configuration?.StoreProvider ?? c.GetService<IStoreProvider>());

            services.AddTransient<IFeatureStore>(c =>
                new FeatureStore(c.GetService<IStoreProvider>(), c.GetService<IToggleDeserializer>(), new Configuration
                {
                    Logger = configuration?.Logger ?? c.GetService<IFeatureLogger>(),
                    StoreProvider = configuration?.StoreProvider ?? c.GetService<IStoreProvider>()
                }));

            services.AddTransient<Features>(c =>
                new Features(c.GetService<IFeatureStore>(), new Configuration
                {
                    Logger = configuration?.Logger ?? c.GetService<IFeatureLogger>(),
                    StoreProvider = configuration?.StoreProvider ?? c.GetService<IStoreProvider>()
                }));
        }
    }
}