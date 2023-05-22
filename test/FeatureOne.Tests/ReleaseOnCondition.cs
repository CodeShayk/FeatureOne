using FeatureOne.Core;

namespace FeatureOne.Tests
{
    internal class ReleaseOnCondition : ICondition
    {
        /// <summary>
        /// UTC Release date & time.
        /// </summary>
        public DateTime ReleaseOn { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            return (DateTime.UtcNow >= ReleaseOn);
        }
    }
}