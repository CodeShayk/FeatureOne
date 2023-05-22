using System.Text.Json.Nodes;
using FeatureOne.Core;

namespace FeatureOne.SQL
{
    /// <summary>
    /// Implement to provide deserialization strategy of condition types.
    /// </summary>
    public interface IConditionDeserializer
    {
        ICondition Deserialize(JsonObject condition);
    }
}