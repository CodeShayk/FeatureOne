using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.Core.Stores
{
    public class FeatureStore : IFeatureStore
    {
        private readonly IStorageProvider storageProvider;
        private readonly IFeatureLogger logger;

        public FeatureStore(IStorageProvider storageProvider) : this(storageProvider, new DefaultLogger(null))
        {
        }

        public FeatureStore(IStorageProvider storageProvider, IFeatureLogger logger)
        {
            this.storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<IFeature> FindStartsWith(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    logger?.Info($"FeatureOne, Action='StorageProvider.Get', Message='The provided feature name was null or whitespace.'");
                    return Enumerable.Empty<IFeature>();
                }

                var features = storageProvider.GetByName(name);
                if (features == null || !features.Any())
                {
                    logger?.Info($"FeatureOne, Action='StorageProvider.Get', Message='Retrieved Features list was empty.'");
                    return Enumerable.Empty<IFeature>();
                }

                var result = new List<IFeature>();

                foreach (var feature in features
                    .Where(x => x.Toggle?.Conditions != null && x.Toggle.Conditions.Any())
                    .Where(x => x.Name.Value.StartsWith(name, StringComparison.OrdinalIgnoreCase)))
                    result.Add(feature);

                return result;
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOne, Action='StorageProvider.Get'", ex);
            }

            return Enumerable.Empty<IFeature>();
        }
    }
}