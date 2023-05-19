using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.Core
{
    public class Toggle : IToggle
    {
        public Toggle() : this(Operator.Any, Array.Empty<ICondition>())
        {
        }

        public Toggle(Operator @operator, params ICondition[] conditions)
        {
            Operator = @operator;
            Conditions = conditions ?? Array.Empty<ICondition>();
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