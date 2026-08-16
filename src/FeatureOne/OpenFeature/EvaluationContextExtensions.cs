using System;
using System.Collections.Generic;
using System.Globalization;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature
{
    /// <summary>
    /// Extension methods for converting OpenFeature <see cref="EvaluationContext"/> to FeatureOne user claims dictionary.
    /// </summary>
    public static class EvaluationContextExtensions
    {
        /// <summary>
        /// Converts an OpenFeature <see cref="EvaluationContext"/> to a dictionary of claims compatible with FeatureOne.
        /// </summary>
        /// <param name="context">The OpenFeature evaluation context.</param>
        /// <returns>A dictionary of string claim key-value pairs.</returns>
        public static Dictionary<string, string> ToClaims(this EvaluationContext context)
        {
            var claims = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (context == null)
            {
                return claims;
            }

            if (!string.IsNullOrEmpty(context.TargetingKey))
            {
                claims["targetingKey"] = context.TargetingKey;
                claims["sub"] = context.TargetingKey;
                claims["user_id"] = context.TargetingKey;
            }

            foreach (var kvp in context)
            {
                if (kvp.Value == null || kvp.Value.IsNull)
                {
                    continue;
                }

                string stringValue = FormatValue(kvp.Value);
                if (stringValue != null)
                {
                    claims[kvp.Key] = stringValue;
                }
            }

            return claims;
        }

        private static string FormatValue(Value value)
        {
            if (value == null || value.IsNull)
            {
                return null;
            }

            if (value.IsBoolean)
            {
                return value.AsBoolean == true ? "true" : "false";
            }

            if (value.IsDateTime)
            {
                return value.AsDateTime.HasValue ? value.AsDateTime.Value.ToString("o", CultureInfo.InvariantCulture) : null;
            }

            return value.AsString;
        }
    }
}
