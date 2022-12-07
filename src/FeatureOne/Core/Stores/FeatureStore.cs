namespace FeatureOne.Core.Stores
{
    public class FeatureStore : IFeatureStore
    {
        private IStoreProvider storeProvider;
        private readonly FeatureConfiguration Configuration;
        private IToggleDeserializer toggleDeserializer;

        public FeatureStore(IStoreProvider storeProvider) : this(storeProvider, new ToggleDeserializer(), new FeatureConfiguration())
        {
        }

        public FeatureStore(IStoreProvider storeProvider, IToggleDeserializer toggleDeserializer) : this(storeProvider, toggleDeserializer, new FeatureConfiguration())
        {
        }

        public FeatureStore(IStoreProvider storeProvider, IToggleDeserializer toggleDeserializer, FeatureConfiguration configuration)
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
                    result.Add(new Feature(feature.Name, toggleDeserializer.Deserializer(feature.Toggle)));
                }
                catch (Exception ex)
                {
                    Configuration.Logger?.Error($"Action='Failed to Deserialize', Feature='{feature.Name}', Exception='{ex}'.");
                }
            }

            return result;
        }
    }
}