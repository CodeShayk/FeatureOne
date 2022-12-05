using System.Reflection;
using System.Text.Json.Nodes;
using FeatureOn.Toggle;

namespace FeatureOn.Toggles
{
    public static class ToggleConditionFactory
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

        public static IToggleCondition Create(JsonObject JsonObject)
        {
            
            if (JsonObject == null)
                throw new ArgumentNullException(nameof(JsonObject));
                        
            var typeName = JsonObject?["type"]?.ToString();

            var toggle = CreateInstance(typeName);

            HydrateToggle(toggle, JsonObject);


            return toggle;
        }
        public static IToggleCondition[] Create(JsonObject[] conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));

            return conditions.Select(s => Create(s)).ToArray();
        }
        private static void HydrateToggle(IToggleCondition toggleCondition, JsonObject state)
        {
            foreach (var propertyInfo in GetProperties(toggleCondition))
            {
                var name = propertyInfo.Name.ToLower();
                if (state.ContainsKey(name) == false)
                    continue;

                var storedValue = state[name]?.GetValue<string>();
                propertyInfo.SetValue(toggleCondition, storedValue, null);
            }
        }

        private static PropertyInfo[] GetProperties(IToggleCondition condition)
        {
            var propertyInfos = condition.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            return propertyInfos;
        }

        private static IToggleCondition CreateInstance(string typeName)
        {
            var type = LoaddedTypes
                .FirstOrDefault(p => typeof(IToggleCondition).IsAssignableFrom(p) && p.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

            if (type == null)
                throw new Exception($"Could not find a toggle type for: '{typeName}'");

            return (IToggleCondition)Activator.CreateInstance(type, true);
        }
    }
}