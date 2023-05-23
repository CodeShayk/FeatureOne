using FeatureOne.Core;

namespace FeatureOne.Json
{
    public interface IToggleDeserializer
    {
        IToggle Deserialize(string toggle);
    }
}