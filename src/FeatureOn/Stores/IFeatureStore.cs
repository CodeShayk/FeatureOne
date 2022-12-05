using Ninja.FeatureIt;

namespace FeatureOn.Stores
{
    public interface IFeatureStore
    {
        IEnumerable<IFeature> FindStartsWith(string key);
        IEnumerable<IFeature> GetAll();
    }
}