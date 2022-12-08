namespace FeatureOne.Core.Stores
{
    public class FeatureStore : IFeatureStore
    {
        private IStoreProvider storeProvider;
        private readonly Configuration Configuration;
        private IToggleDeserializer toggleDeserializer;

        public FeatureStore(IStoreProvider storeProvider) : this(storeProvider, new ToggleDeserializer(), new Configuration())
        {
        }

        public FeatureStore(IStoreProvider storeProvider, IToggleDeserializer toggleDeserializer) : this(storeProvider, toggleDeserializer, new Configuration())
        {
        }

        public FeatureStore(IStoreProvider storeProvider, IToggleDeserializer toggleDeserializer, Configuration configuration)
        {
            this.storeProvider = storeProvider;
            Configuration = configuration;
            this.toggleDeserializer = toggleDeserializer;
        }

        public IEnumerable<IFeature> FindStartsWith(string key)
        {
            return GetAll().Where(x => x.Name.Value.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IFeature> GetAll()
        {
            var features = storeProvider.Get();
            if (features == null || !features.Any()) return Enumerable.Empty<IFeature>();

            var result = new List<IFeature>();

            foreach (var feature in features)
            {
                try
                {
                    result.Add(new Feature(feature.Key, toggleDeserializer.Deserializer(feature.Value)));
                }
                catch (Exception ex)
                {
                    Configuration.Logger?.Error($"Action='Failed to Deserialize', Feature='{feature.Key}', Exception='{ex}'.");
                }
            }

            return result;
        }
    }
}