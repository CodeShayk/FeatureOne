using FeatureOn.Toggle;

namespace Ninja.FeatureIt
{
    public interface IFeature
    {
        FeatureName Name { get; }
        IFeatureToggle Toggle { get; }
        bool IsEnabled(IDictionary<string, string> claims);
    }
}