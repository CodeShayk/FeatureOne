using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FeatureOne.Core.Toggles.Conditions
{
    /// <summary>
    /// Compares a user claim against a fixed target value using a relational operator.
    /// </summary>
    /// <remarks>
    /// Claims are strings, so the comparison strategy is chosen from the values themselves:
    /// when both the claim and the target parse as numbers they are compared numerically,
    /// otherwise they are compared as ordinal strings. This keeps lexical targets such as
    /// user tiers working while giving numeric claims (<c>age</c>, <c>seats</c>, <c>score</c>)
    /// arithmetic ordering rather than the digit-by-digit ordering that would rank "9" above "18".
    /// Ordinal comparison is used for the string case so results do not vary with the ambient culture.
    /// </remarks>
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

            var claimValue = claims.First(x => x.Key.Equals(Claim)).Value?.Trim() ?? string.Empty;
            var comparisonValue = Value?.Trim() ?? string.Empty;

            var comparison = Compare(claimValue, comparisonValue);

            switch (Operator)
            {
                case RelationalOperator.Equals:
                    return comparison == 0;
                case RelationalOperator.NotEquals:
                    return comparison != 0;
                case RelationalOperator.GreaterThan:
                    return comparison > 0;
                case RelationalOperator.GreaterThanOrEqual:
                    return comparison >= 0;
                case RelationalOperator.LessThan:
                    return comparison < 0;
                case RelationalOperator.LessThanOrEqual:
                    return comparison <= 0;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Compares two claim values, numerically when both are numbers and ordinally otherwise.
        /// </summary>
        /// <returns>A negative value, zero, or a positive value, following <see cref="IComparable"/> conventions.</returns>
        private static int Compare(string claimValue, string comparisonValue)
            => TryCompareNumeric(claimValue, comparisonValue, out var numeric)
                ? numeric
                : string.Compare(claimValue, comparisonValue, StringComparison.Ordinal);

        private static bool TryCompareNumeric(string left, string right, out int result)
        {
            result = 0;

            // decimal first: it compares integers and fixed-point values exactly, where double
            // would lose precision on large integers.
            if (decimal.TryParse(left, NumberStyles.Number, CultureInfo.InvariantCulture, out var leftDecimal) &&
                decimal.TryParse(right, NumberStyles.Number, CultureInfo.InvariantCulture, out var rightDecimal))
            {
                result = leftDecimal.CompareTo(rightDecimal);
                return true;
            }

            // Fall back to double for magnitudes and exponent notation decimal cannot represent.
            if (double.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out var leftDouble) &&
                double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out var rightDouble) &&
                !double.IsNaN(leftDouble) && !double.IsNaN(rightDouble))
            {
                result = leftDouble.CompareTo(rightDouble);
                return true;
            }

            return false;
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
