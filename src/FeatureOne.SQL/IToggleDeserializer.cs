using FeatureOne.Core;

namespace FeatureOne.SQL
{
    public interface IToggleDeserializer
    {
        IToggle Deserialize(string toggle);
    }
}