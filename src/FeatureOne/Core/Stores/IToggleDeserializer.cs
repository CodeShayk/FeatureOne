namespace FeatureOne.Core.Stores
{
    public interface IToggleDeserializer
    {
        IToggle Deserializer(string input);
    }
}