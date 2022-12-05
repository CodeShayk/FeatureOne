using System.Text.Json.Serialization;

namespace FeatureOn.Toggles
{
    public class ToggleConditionState
    {
        public string Type { get; }
        public IDictionary<string, object> Values { get; } = new Dictionary<string, object>();

        [JsonConstructor]
        public ToggleConditionState(string type, IDictionary<string, object> values)
        {
            Type = type.Trim();
            Values = values;
        }
    }
}