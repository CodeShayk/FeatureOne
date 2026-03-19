using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.Core.Toggles.Conditions
{
    public class RelationalCondition : ICondition
    {
        public string Claim { get; set; }
        public RelationalOperator Operator { get; set; }
        public string Value { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            if (claims == null)
                return false;

            if (!claims.Any(x => x.Key != null && x.Key.Equals(Claim)))
                return false;
            
            var claimValue = claims.First(x => x.Key.Equals(Claim)).Value?.Trim()    ?? string.Empty;    
            var comparisonValue = Value?.Trim() ?? string.Empty;

            switch (Operator)
            {
                case RelationalOperator.Equals:
                    return claimValue == comparisonValue;
                case RelationalOperator.NotEquals:
                    return claimValue != comparisonValue;
                case RelationalOperator.GreaterThan:
                    return string.Compare(claimValue, comparisonValue) > 0;
                case RelationalOperator.GreaterThanOrEqual:
                    return string.Compare(claimValue, comparisonValue) >= 0;
                case RelationalOperator.LessThanOrEqual:
                    return string.Compare(claimValue, comparisonValue) <= 0;
                default:
                    return false;
            }
            
        }
    }
    public enum RelationalOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
}