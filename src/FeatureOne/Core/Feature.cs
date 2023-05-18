using System.Collections.Generic;

namespace FeatureOne.Core
{
    public class Feature : IFeature
    {
        protected Feature()
        {
        }

        public Feature(string name, IToggle toggle)
        {
            Name = new FeatureName(name);
            Toggle = toggle;
        }

        public FeatureName Name { get; protected set; }
        public IToggle Toggle { get; protected set; }

        public bool IsEnabled(IDictionary<string, string> claims) => Toggle.Run(claims);
    }
}