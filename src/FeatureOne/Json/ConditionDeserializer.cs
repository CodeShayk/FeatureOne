using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FeatureOne.Core;

namespace FeatureOne.Json
{
    public class ConditionDeserializer : IConditionDeserializer
    {
        private static Type[] loaddedTypes;

        private static Type[] LoaddedTypes
        {
            get
            {
                if (loaddedTypes != null && loaddedTypes.Length > 0)
                    return loaddedTypes;

                loaddedTypes = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(mytype => mytype.GetInterfaces().Contains(typeof(ICondition)))
                        .ToArray();

                return loaddedTypes;
            }
        }

        public ICondition Deserialize(JsonObject condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var typeName = condition?["type"]?.ToString();

            var toggle = CreateInstance(new NamePostFix(typeName, "Condition"));

            HydrateToggle(toggle, condition);

            return toggle;
        }

        private static ICondition CreateInstance(NamePostFix conditionName)
        {
            var type = LoaddedTypes
               .FirstOrDefault(p => p.Name.Equals(conditionName.Name, StringComparison.OrdinalIgnoreCase));

            if (type == null)
                throw new Exception($"Could not find a toggle type for: '{conditionName.Name}'");

            return (ICondition)Activator.CreateInstance(type, true);
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