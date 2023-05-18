using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FeatureOne.Core.Stores
{
    public class FeatureStore : IFeatureStore
    {
        private IStorageProvider storageProvider;
        private IFeatureLogger logger;
        private IToggleDeserializer toggleDeserializer;

        public FeatureStore(IStorageProvider storageProvider) : this(storageProvider, new NullLogger(), new ToggleDeserializer())
        {
        }

        public FeatureStore(IStorageProvider storageProvider, IFeatureLogger logger) : this(storageProvider, logger, new ToggleDeserializer())
        {
        }

        public FeatureStore(IStorageProvider storageProvider, IFeatureLogger logger, IToggleDeserializer toggleDeserializer)
        {
            this.storageProvider = storageProvider;
            this.logger = logger;
            this.toggleDeserializer = toggleDeserializer;
        }

        public IEnumerable<IFeature> FindStartsWith(string key)
        {
            return GetAll().Where(x => x.Name.Value.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IFeature> GetAll()
        {
            var features = storageProvider.Get();
            if (features == null || !features.Any())
                return Enumerable.Empty<IFeature>();

            var result = new List<IFeature>();

            foreach (var feature in features)
            {
                try
                {
                    result.Add(new Feature(feature.Key, toggleDeserializer.Deserializer(feature.Value)));
                    logger?.Info($"FeatureOne, Action='StorageProvider.Get', Feature='{feature.Key}', Message='Reterieved Success'");
                }
                catch (Exception ex)
                {
                    logger?.Error($"FeatureOne, Action='StorageProvider.Get', Feature='{feature.Key}', Toggle='{feature.Value}' Exception='{ex}'.");
                }
            }

            return result;
        }
    }
}