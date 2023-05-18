using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeatureOne.Core.Stores
{
    public static class ConditionFactory
    {
        private static Type[] loaddedTypes;

        private static Type[] LoaddedTypes
        {
            get
            {
                if (loaddedTypes == null || loaddedTypes.Length == 0)
                    loaddedTypes = Assembly.GetExecutingAssembly().GetTypes();

                return loaddedTypes;
            }
        }

        public static ICondition Create(JsonObject JsonObject)
        {
            if (JsonObject == null)
                throw new ArgumentNullException(nameof(JsonObject));

            var typeName = JsonObject?["type"]?.ToString();

            var toggle = CreateInstance(new NamePostFix(typeName, "Condition"));

            HydrateToggle(toggle, JsonObject);

            return toggle;
        }

        public static ICondition[] Create(JsonObject[] conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));

            return conditions.Select(s => Create(s)).ToArray();
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

        private static ICondition CreateInstance(NamePostFix conditionName)
        {
            var type = LoaddedTypes
                .FirstOrDefault(p => typeof(ICondition).IsAssignableFrom(p) && p.Name.Equals(conditionName.Name, StringComparison.OrdinalIgnoreCase));

            if (type == null)
                throw new Exception($"Could not find a toggle type for: '{conditionName.Name}'");

            return (ICondition)Activator.CreateInstance(type, true);
        }
    }
}