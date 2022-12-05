namespace FeatureOn.Toggle
{
    public interface IToggleCondition
    {
        bool Evaluate(IDictionary<string, string> claims);
    }
}