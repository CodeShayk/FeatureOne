using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FeatureOne.Core.Toggles.Conditions
{
    public class RegexCondition : ICondition
    {
        public string Claim { get; set; }
        public string Expression { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            if (claims == null)
                return false;

            if (!claims.Any(x => x.Key != null && x.Key.Equals(Claim)))
                return false;

            var result = Regex.IsMatch(
                claims.First(x => x.Key.Equals(Claim)).Value,
                Expression,
                RegexOptions.None,
                Constants.DefaultRegExTimeout
            );
            return result;
        }
    }
}