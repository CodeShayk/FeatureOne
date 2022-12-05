using FeatureOn.Toggle;
using FeatureOn.Toggle.Conditions;

namespace FeatureOn.Toggles
{
    public class FeatureToggle : IFeatureToggle
    {
        public FeatureToggle(ToggleOperator @operator, IToggleCondition[] conditions)
        {
            Operator = @operator;
            Conditions = conditions ?? new[] { new SimpleCondition() };
        }

        public ToggleOperator Operator { get; set; } = ToggleOperator.Any;
        public IToggleCondition[] Conditions { get; set; }
             
        public bool Run(IDictionary<string, string> claims)
        {
            if (claims == null)
                return false;

            return Operator == ToggleOperator.Any
                 ? Conditions.Any(x => x.Evaluate(claims))
                 : Conditions.All(x => x.Evaluate(claims));
        }
    }
}