using Ninja.FeatureIt;
using System.Text.RegularExpressions;

namespace FeatureOn.Toggle.Conditions
{
    public class RegexCondition : IToggleCondition
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