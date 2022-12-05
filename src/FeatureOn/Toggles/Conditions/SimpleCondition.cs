namespace FeatureOn.Toggle.Conditions
{
    public class SimpleCondition : IToggleCondition
    {
        public bool IsEnabled { get; set; }
        public bool Evaluate(IDictionary<string, string> claims)
        {
            return IsEnabled;
        }
    }
}