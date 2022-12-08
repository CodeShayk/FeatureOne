using FeatureOne.Core.Stores;

namespace FeatureOne
{
    public class Configuration
    {
        public IFeatureLogger Logger { get; set; } = new NullLogger();
        public IStoreProvider StoreProvider { get; set; }
    }
}