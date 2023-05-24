using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using FeatureOne.Core;

namespace FeatureOne.Json
{
    public class ToggleDeserializer : IToggleDeserializer
    {
        private readonly IConditionDeserializer conditionDeserializer;

        public ToggleDeserializer() : this(new ConditionDeserializer())
        {
        }

        public ToggleDeserializer(IConditionDeserializer conditionDeserializer) =>
            this.conditionDeserializer = conditionDeserializer;

        public IToggle Deserialize(string toggle)
        {
            var jObject = JsonNode.Parse(toggle);

            var toggleOperator = jObject["operator"]?.ToString() ?? Operator.Any.ToString();
            var toggleConditions = jObject["conditions"].Deserialize<JsonObject[]>();

            return new Toggle
            (
                Enum.TryParse<Operator>(toggleOperator, true, out var @operator) ? @operator : Operator.Any,
                toggleConditions.Select(t => conditionDeserializer.Deserialize(t)).ToArray()
            );
        }
    }
}