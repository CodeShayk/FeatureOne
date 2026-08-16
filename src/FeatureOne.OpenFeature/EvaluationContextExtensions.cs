using System;
using System.Collections.Generic;
using System.Globalization;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature
{
    /// <summary>
    /// Extension methods for mapping OpenFeature EvaluationContext to FeatureOne Claims dictionary.
    /// </summary>
    public static class EvaluationContextExtensions
    {
        /// <summary>
        /// Converts an OpenFeature <see cref="EvaluationContext"/> into a FeatureOne claims dictionary.
        /// </summary>
        /// <param name="context">The OpenFeature evaluation context.</param>
        /// <returns>A string key-value dictionary representing claims for feature toggle evaluation.</returns>
        public static IDictionary<string, string> ToClaims(this EvaluationContext context)
        {
            var claims = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (context == null)
            {
                return claims;
            }

            // Map TargetingKey to standard user claim identifiers if present
            if (!string.IsNullOrEmpty(context.TargetingKey))
            {
                claims["targetingKey"] = context.TargetingKey;
                claims["sub"] = context.TargetingKey;
                claims["user_id"] = context.TargetingKey;
            }

            // Map attributes via context.AsDictionary()
            var dict = context.AsDictionary();
            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    if (kvp.Value != null)
                    {
                        claims[kvp.Key] = ConvertValueToString(kvp.Value);
                    }
                }
            }

            return claims;
        }

        private static string ConvertValueToString(Value value)
        {
            if (value == null || value.IsNull)
            {
                return string.Empty;
            }

            if (value.IsString)
            {
                return value.AsString;
            }

            if (value.IsBoolean)
            {
                return value.AsBoolean.ToString().ToLowerInvariant();
            }

            if (value.IsNumber)
            {
                return value.AsString ?? (value.AsDouble.HasValue ? value.AsDouble.Value.ToString(CultureInfo.InvariantCulture) : value.ToString());
            }

            if (value.IsDateTime)
            {
                return value.AsDateTime.HasValue ? value.AsDateTime.Value.ToString("o", CultureInfo.InvariantCulture) : string.Empty;
            }

            return value.AsString ?? value.ToString();
        }
    }
}
