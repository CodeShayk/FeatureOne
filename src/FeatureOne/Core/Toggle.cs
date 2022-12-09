namespace FeatureOne.Core
{
    public class Toggle : IToggle
    {
        public Toggle(Operator @operator, ICondition[] conditions)
        {
            Operator = @operator;
            Conditions = conditions;
        }

        public Operator Operator { get; set; }
        public ICondition[] Conditions { get; set; }

        public bool Run(IDictionary<string, string> claims)
        {
            if (Conditions == null)
                return false;

            claims ??= new Dictionary<string, string>();

            return Operator == Operator.Any
                 ? Conditions.Any(x => x.Evaluate(claims))
                 : Conditions.All(x => x.Evaluate(claims));
        }
    }
}