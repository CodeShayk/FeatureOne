using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using FeatureOne.Core;
using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.Json
{
    /// <summary>
    /// Deserializes toggle conditions from JSON into <see cref="ICondition"/> instances.
    /// </summary>
    /// <remarks>
    /// Condition types are resolved from an explicit allow list rather than by loading arbitrary type
    /// names from configuration, so a compromised or malformed store cannot instantiate unexpected types.
    /// Custom conditions are supported by registering them explicitly:
    /// <code>
    /// var deserializer = new ConditionDeserializer()
    ///     .Register&lt;PercentageCondition&gt;("Percentage");
    ///
    /// services.AddFeatureOneWithFileStorage(config, new ToggleDeserializer(deserializer));
    /// </code>
    /// </remarks>
    public class ConditionDeserializer : IConditionDeserializer
    {
        private static readonly IReadOnlyDictionary<string, Type> BuiltInConditionTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "SimpleCondition", typeof(SimpleCondition) },
            { "RegexCondition", typeof(RegexCondition) },
            { "DateRangeCondition", typeof(DateRangeCondition) },
            { "RelationalCondition", typeof(RelationalCondition) }
        };

        private readonly Dictionary<string, Type> conditionTypes;

        /// <summary>
        /// Initializes a new instance supporting the built-in condition types.
        /// </summary>
        public ConditionDeserializer() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance supporting the built-in condition types plus the supplied custom types.
        /// </summary>
        /// <param name="customConditionTypes">
        /// Custom condition types keyed by the <c>type</c> discriminator used in JSON. Keys are normalised
        /// so that both <c>"Percentage"</c> and <c>"PercentageCondition"</c> resolve to the same entry.
        /// A custom entry may override a built-in one.
        /// </param>
        public ConditionDeserializer(IEnumerable<KeyValuePair<string, Type>> customConditionTypes)
        {
            conditionTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var builtIn in BuiltInConditionTypes)
                conditionTypes[builtIn.Key] = builtIn.Value;

            if (customConditionTypes == null)
                return;

            foreach (var custom in customConditionTypes)
                Register(custom.Key, custom.Value);
        }

        /// <summary>
        /// Registers a custom condition type against the given JSON <c>type</c> discriminator.
        /// </summary>
        /// <typeparam name="TCondition">The condition type. Must expose a parameterless constructor.</typeparam>
        /// <param name="name">
        /// The discriminator used in JSON. Both <c>"Percentage"</c> and <c>"PercentageCondition"</c> are
        /// accepted and normalise to the same registration.
        /// </param>
        /// <returns>This instance, for chaining.</returns>
        public ConditionDeserializer Register<TCondition>(string name) where TCondition : ICondition
            => Register(name, typeof(TCondition));

        /// <summary>
        /// Registers a custom condition type against the given JSON <c>type</c> discriminator.
        /// </summary>
        /// <param name="name">The discriminator used in JSON.</param>
        /// <param name="conditionType">The condition type. Must implement <see cref="ICondition"/> and expose a parameterless constructor.</param>
        /// <returns>This instance, for chaining.</returns>
        public ConditionDeserializer Register(string name, Type conditionType)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (conditionType == null)
                throw new ArgumentNullException(nameof(conditionType));

            if (!typeof(ICondition).IsAssignableFrom(conditionType))
                throw new FeatureOneConfigurationException($"Condition type '{conditionType.FullName}' registered as '{name}' does not implement {nameof(ICondition)}.");

            if (conditionType.IsAbstract || conditionType.IsInterface)
                throw new FeatureOneConfigurationException($"Condition type '{conditionType.FullName}' registered as '{name}' must be a concrete type.");

            if (GetParameterlessConstructor(conditionType) == null)
                throw new FeatureOneConfigurationException($"Condition type '{conditionType.FullName}' registered as '{name}' must expose a parameterless constructor.");

            conditionTypes[new NamePostFix(name, "Condition").Name] = conditionType;
            return this;
        }

        /// <inheritdoc />
        public ICondition Deserialize(JsonObject condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            var typeName = condition["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(typeName))
                throw new FeatureOneConfigurationException("Toggle condition is missing the required 'type' property.");

            var toggle = CreateInstance(new NamePostFix(typeName, "Condition"));

            HydrateToggle(toggle, condition);

            return toggle;
        }

        /// <summary>
        /// Creates an instance of the registered condition type matching the given normalised name.
        /// </summary>
        /// <param name="conditionName">The normalised condition name.</param>
        /// <returns>A new condition instance.</returns>
        public ICondition CreateInstance(NamePostFix conditionName)
        {
            if (conditionName == null)
                throw new ArgumentNullException(nameof(conditionName));

            // NamePostFix transforms both "Simple" and "SimpleCondition" to "SimpleCondition",
            // so registrations and lookups always agree on the postfixed form.
            var processedName = conditionName.Name;

            if (!conditionTypes.TryGetValue(processedName, out var type))
                throw new FeatureOneConfigurationException(
                    $"Could not find a condition type for: '{processedName}'. Registered types are: {string.Join(", ", conditionTypes.Keys.OrderBy(x => x))}. " +
                    $"Register custom conditions with {nameof(ConditionDeserializer)}.{nameof(Register)}().");

            try
            {
                return (ICondition)Activator.CreateInstance(type, true);
            }
            catch (Exception ex)
            {
                throw new FeatureOneConfigurationException($"Failed to construct condition type '{type.FullName}' registered as '{processedName}'.", ex);
            }
        }

        private static ConstructorInfo GetParameterlessConstructor(Type type)
            => type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

        private static void HydrateToggle(ICondition toggleCondition, JsonObject state)
        {
            foreach (var propertyInfo in GetProperties(toggleCondition))
            {
                var name = propertyInfo.Name;

                var entry = state.FirstOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (entry.Key == null)
                    continue;

                var value = entry.Value?.ToString();

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
