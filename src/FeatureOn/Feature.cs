using FeatureOn.Toggle;
using Ninja.FeatureIt;

namespace FeatureOn
{
    public class Feature : IFeature
    {
        protected Feature()
        {
        }

        public Feature(string name, IFeatureToggle toggle)
        {
            Name = new FeatureName(name);
            Toggle = toggle;
        }

        public FeatureName Name { get; protected set; }
        public IFeatureToggle Toggle { get; protected set; }
        public bool IsEnabled(IDictionary<string, string> claims) => Toggle.Run(claims);
    }
}