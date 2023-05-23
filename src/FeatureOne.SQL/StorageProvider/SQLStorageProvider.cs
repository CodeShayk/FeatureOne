using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FeatureOne.Cache;
using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Json;

[assembly: InternalsVisibleTo("FeatureOne.SQL.Tests")]

namespace FeatureOne.SQL.StorageProvider
{
    public class SQLStorageProvider : IStorageProvider
    {
        internal readonly IDbRepository repository;
        internal readonly IToggleDeserializer deserializer;
        internal readonly ICache cache;
        internal readonly CacheSettings cacheSettings;

        public SQLStorageProvider(SQLConfiguration configuration, ICache cache = null, IConditionDeserializer conditionDeserializer = null)
        {
            this.cacheSettings = configuration?.CacheSettings ?? new CacheSettings();
            this.repository = new DbRepository(configuration);
            this.deserializer = new ToggleDeserializer(conditionDeserializer ?? new ConditionDeserializer());
            this.cache = cache ?? new FeatureCache();
        }

        public SQLStorageProvider(IDbRepository repository, IToggleDeserializer deserializer, ICache cache, CacheSettings cacheSettings)
        {
            this.repository = repository;
            this.deserializer = deserializer ?? new ToggleDeserializer(new ConditionDeserializer());
            this.cache = cache ?? new FeatureCache();
            this.cacheSettings = cacheSettings ?? new CacheSettings();
        }

        public IFeature[] GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            DbRecord[] dbFeatures = null;

            if (cacheSettings.EnableCache)
                dbFeatures = (DbRecord[])cache?.Get(name);

            if (dbFeatures == null)
                dbFeatures = repository.GetByName(name);

            if (cacheSettings.EnableCache)
            {
                var policy = cacheSettings.Expiry.GetPolicy();
                cache.Add(name, dbFeatures, policy);
            }

            return dbFeatures != null && dbFeatures.Any()
                ? dbFeatures.Where(x => !string.IsNullOrEmpty(x.Toggle)).Select(f => new Feature(f.Name, deserializer.Deserialize(f.Toggle))).ToArray()
                : Array.Empty<IFeature>();
        }
    }
}