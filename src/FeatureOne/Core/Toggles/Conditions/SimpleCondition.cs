using System.Collections.Generic;

namespace FeatureOne.Core.Toggles.Conditions
{
    public class SimpleCondition : ICondition
    {
        public bool IsEnabled { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            return IsEnabled;
        }
    }
}