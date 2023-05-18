using System.Collections.Generic;
using FeatureOne.Core;

namespace FeatureOne
{
    public interface IFeature
    {
        FeatureName Name { get; }
        IToggle Toggle { get; }

        bool IsEnabled(IDictionary<string, string> claims);
    }
}