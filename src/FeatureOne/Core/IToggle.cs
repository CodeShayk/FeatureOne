using System.Collections.Generic;

namespace FeatureOne.Core
{
    public interface IToggle
    {
        Operator Operator { get; set; }
        ICondition[] Conditions { get; set; }

        bool Run(IDictionary<string, string> claims);
    }
}