using System.Collections.Generic;

namespace FeatureOne
{
    public interface IFeatureStore
    {
        IEnumerable<IFeature> FindStartsWith(string key);
    }
}