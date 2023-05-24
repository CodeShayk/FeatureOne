using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.Core.Stores
{
    public class FeatureStore : IFeatureStore
    {
        private readonly IStorageProvider storageProvider;
        private readonly IFeatureLogger logger;

        public FeatureStore(IStorageProvider storageProvider) : this(storageProvider, new NullLogger())
        {
        }

        public FeatureStore(IStorageProvider storageProvider, IFeatureLogger logger)
        {
            this.storageProvider = storageProvider;
            this.logger = logger;
        }

        public IEnumerable<IFeature> FindStartsWith(string name)
        {
            try
            {
                var features = storageProvider.GetByName(name);
                if (features == null || !features.Any())
                {
                    logger?.Info($"FeatureOne, Action='StorageProvider.Get', Message='Retrieved Features list was empty.'");
                    return Enumerable.Empty<IFeature>();
                }

                var result = new List<IFeature>();

                foreach (var feature in features.Where(x => x.Toggle?.Conditions != null && x.Toggle.Conditions.Any()))
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