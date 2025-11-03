using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FeatureOne.Core;
using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.Json
{
    public class ConditionDeserializer : IConditionDeserializer
    {
        private static readonly Dictionary<string, Type> SafeConditionTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "Simple", typeof(SimpleCondition) },
            { "SimpleCondition", typeof(SimpleCondition) },
            { "Regex", typeof(RegexCondition) },
            { "RegexCondition", typeof(RegexCondition) },
            { "DateRange", typeof(DateRangeCondition) },
            { "DateRangeCondition", typeof(DateRangeCondition) }
        };

        public ICondition Deserialize(JsonObject condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var typeName = condition?["type"]?.ToString();

            var toggle = CreateInstance(new NamePostFix(typeName, "Condition"));

            HydrateToggle(toggle, condition);

            return toggle;
        }

        public ICondition CreateInstance(NamePostFix conditionName)
        {
            // NamePostFix transforms both "Simple" and "SimpleCondition" to "SimpleCondition"
            // So we look up the processed name
            var processedName = conditionName.Name;

            if (SafeConditionTypes.TryGetValue(processedName, out Type type))
            {
                return (ICondition)Activator.CreateInstance(type, true);
            }

            // This shouldn't normally happen with correct inputs since NamePostFix standardizes the format
            throw new Exception($"Could not find a toggle type for: '{processedName}'. Only supported types are: {string.Join(", ", SafeConditionTypes.Keys)}");
        }

        private static void HydrateToggle(ICondition toggleCondition, JsonObject state)
        {
            var properties = GetProperties(toggleCondition);
            foreach (var propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var keyValues = state.Deserialize<Dictionary<string, object>>();

                if (!keyValues.Keys.Contains(name,
                    new LambdaComparer<string>((x, y) => x.Equals(y, StringComparison.OrdinalIgnoreCase))))
                    continue;

                var value = state.Where(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                                 .First().Value?.ToString();

                var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                var propValue = converter.ConvertFrom(value);

                propertyInfo.SetValue(toggleCondition, propValue, null);
            }
        }

        private static PropertyInfo[] GetProperties(ICondition condition)
        {
            var propertyInfos = condition.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            return propertyInfos;
        }
    }
}