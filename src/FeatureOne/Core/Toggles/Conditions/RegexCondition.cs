using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FeatureOne.Core.Toggles.Conditions
{
    public class RegexCondition : ICondition
    {
        public string Claim { get; set; }
        public string Expression { get; set; }
        public TimeSpan Timeout { get; set; } = Constants.DefaultRegExTimeout;

        public bool Evaluate(IDictionary<string, string> claims)
        {
            if (claims == null)
                return false;

            if (!claims.Any(x => x.Key != null && x.Key.Equals(Claim)))
                return false;

            try
            {
                var value = claims.First(x => x.Key.Equals(Claim)).Value;
                var regex = new Regex(
                    Expression,
                    RegexOptions.None,
                    Timeout
                );
                return regex.IsMatch(value);
            }
            catch (RegexMatchTimeoutException)
            {
                // Return false when regex times out to prevent ReDoS
                return false;
            }
            catch (ArgumentException)
            {
                // Invalid regex pattern
                return false;
            }
        }
    }
}