using System.Text.Json;
using System.Text.Json.Nodes;

namespace FeatureOne.Core.Stores
{
    public class ToggleDeserializer : IToggleDeserializer
    {
        public IToggle Deserializer(string input)
        {
            var jObject = JsonNode.Parse(input);

            var toggleOperator = jObject["operator"]?.ToString() ?? Operator.Any.ToString();
            var toggleConditions = jObject["conditions"].Deserialize<JsonObject[]>();

            return new Toggle
            (
                Enum.TryParse<Operator>(toggleOperator, true, out var @operator) ? @operator : Operator.Any,
                ConditionFactory.Create(toggleConditions)
            );
        }
    }
}