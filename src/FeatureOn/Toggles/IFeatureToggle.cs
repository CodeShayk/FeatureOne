namespace FeatureOn.Toggle
{
    public interface IFeatureToggle
    {
        ToggleOperator Operator { get; set; }
        IToggleCondition[] Conditions { get; set; }
        bool Run(IDictionary<string, string> claims);
    }
}