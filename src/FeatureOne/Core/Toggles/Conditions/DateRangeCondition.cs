using System;
using System.Collections.Generic;

namespace FeatureOne.Core.Toggles.Conditions
{
    public class DateRangeCondition : ICondition
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            var now = DateTime.Now.Date;  // Use just the date part for comparison
            
            if (StartDate.HasValue && now < StartDate.Value.Date)
                return false;
                
            if (EndDate.HasValue && now > EndDate.Value.Date)
                return false;
                
            return true;
        }
    }
}