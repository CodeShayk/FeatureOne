using System;
using System.Linq;
using FeatureOne.Core;
using FeatureOne.Core.Stores;

namespace FeatureOne.SQL.StorageProvider
{
    public class SQLStorageProvider : IStorageProvider
    {
        internal IDbRepository repository;
        internal IToggleDeserializer deserializer;
        internal ICache cache;

        internal CacheSettings cacheSettings;

        public SQLStorageProvider(SQLConfiguration sqlConfiguration, IFeatureLogger logger = null, ICache cache = null, IConditionDeserializer conditionDeserializer = null)
        {
            this.cacheSettings = sqlConfiguration?.CacheSettings ?? new CacheSettings();
            this.repository = new DbRepository(sqlConfiguration, logger ?? new NullLogger());
            this.deserializer = new ToggleDeserializer(conditionDeserializer ?? new ConditionDeserializer());
            this.cache = cache ?? new FeatureCache();
        }

        public SQLStorageProvider(IDbRepository repository, IToggleDeserializer deserializer, ICache cache, CacheSettings cacheSettings)
        {
            this.repository = repository;
            this.deserializer = deserializer;
            this.cache = cache;
            this.cacheSettings = cacheSettings ?? new CacheSettings();
        }

        public IFeature[] GetByName(string name)
        {
            DbRecord[] dbFeatures = null;

            if (cacheSettings.EnableCache)
                dbFeatures = (DbRecord[])cache?.Get(name);

            if (dbFeatures == null)
                dbFeatures = repository.GetByName(name);

            if (cacheSettings.EnableCache)
                cache.Add(name, dbFeatures, cacheSettings.ExpiryInMinutes);

            return dbFeatures != null && dbFeatures.Any()
                ? dbFeatures.Where(x => !string.IsNullOrEmpty(x.Toggle)).Select(f => new Feature(f.Name, deserializer.Deserialize(f.Toggle))).ToArray()
                : Array.Empty<IFeature>();
        }
    }
}