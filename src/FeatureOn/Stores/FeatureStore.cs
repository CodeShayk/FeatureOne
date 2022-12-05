using FeatureOn.Configuration;
using FeatureOn.Toggle;
using FeatureOn.Toggles;
using Ninja.FeatureIt;
using System.Text.Json;

namespace FeatureOn.Stores
{
    internal class FeatureStore : IFeatureStore
    {
        IStoreProvider storeProvider;
        private readonly FeatureConfiguration Configuration;

        public FeatureStore(IStoreProvider storeProvider):this(storeProvider, new FeatureConfiguration()) { }
        public FeatureStore(IStoreProvider storeProvider, FeatureConfiguration configuration)
        {
            this.storeProvider = storeProvider;
            this.Configuration = configuration;
        }
        public IEnumerable<IFeature> FindStartsWith(string key)
        {
            return GetAll().Where(x => x.Name.Value.StartsWith(key, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IFeature> GetAll()
        {
            var jsonStrings = storeProvider.Get();
            if (jsonStrings ==null || !jsonStrings.Any()) return Enumerable.Empty<IFeature>();

            var result = new List<IFeature>();

            foreach (var jsonString in jsonStrings)
            {
                FeatureRaw? rawfeature = null;
                try
                {
                    rawfeature = JsonSerializer.Deserialize<FeatureRaw>(jsonString);
                    if (rawfeature == null) continue;

                    result.Add(new Feature(rawfeature.Name, new FeatureToggle(
                            Enum.TryParse<ToggleOperator>(rawfeature.Operator, out var @operator)
                              ? @operator
                              : ToggleOperator.All,
                          ToggleConditionFactory.Create(rawfeature.Conditions))));
                }
                catch (Exception ex)
                {
                    Configuration.Logger?.Warn($"Action='Failed to Deserialize', Feature='{rawfeature?.Name}', Exception='{ex}'.");
                }
            }
            return result;
        }
    }
}
