using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
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

        /// <summary>
        /// Renders an OpenFeature <see cref="Value"/> as the string form FeatureOne conditions evaluate against.
        /// Numbers are formatted with the invariant culture so that <c>RelationalCondition</c> parses them
        /// consistently regardless of the ambient culture. Lists and structures have no scalar claim
        /// representation and are serialized to JSON so they are at least addressable by a RegexCondition.
        /// </summary>
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

            if (value.IsNumber)
            {
                var number = value.AsDouble;
                if (!number.HasValue)
                {
                    return null;
                }

                // AsInteger rounds rather than reporting non-integral values, so decide here instead:
                // whole numbers render as "95", not "95.0", while 1.5 must not become "2".
                var d = number.Value;
                if (!double.IsNaN(d) && !double.IsInfinity(d) && d == Math.Floor(d)
                    && d >= long.MinValue && d <= long.MaxValue)
                {
                    return ((long)d).ToString(CultureInfo.InvariantCulture);
                }

                return d.ToString("R", CultureInfo.InvariantCulture);
            }

            if (value.IsString)
            {
                return value.AsString;
            }

            if (value.IsList || value.IsStructure)
            {
                return SerializeComposite(value);
            }

            return value.AsString;
        }

        private static string SerializeComposite(Value value)
        {
            if (value.IsList)
            {
                var items = value.AsList;
                if (items == null)
                {
                    return null;
                }

                return "[" + string.Join(",", items.Select(FormatValueForJson)) + "]";
            }

            var structure = value.AsStructure;
            if (structure == null)
            {
                return null;
            }

            return "{" + string.Join(",", structure.AsDictionary()
                .Select(kvp => $"{JsonEncode(kvp.Key)}:{FormatValueForJson(kvp.Value)}")) + "}";
        }

        private static string FormatValueForJson(Value value)
        {
            if (value == null || value.IsNull)
            {
                return "null";
            }

            if (value.IsBoolean || value.IsNumber)
            {
                return FormatValue(value);
            }

            if (value.IsList || value.IsStructure)
            {
                return SerializeComposite(value);
            }

            return JsonEncode(FormatValue(value));
        }

        private static string JsonEncode(string value)
            => value == null ? "null" : JsonSerializer.Serialize(value);
    }
}
