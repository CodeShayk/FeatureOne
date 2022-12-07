namespace FeatureOne.Core
{
    /// <summary>
    /// Interface to implement toggle condition.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Implement method to evaulate toggle condition.
        /// </summary>
        /// <param name="claims">List of user claims; could be null</param>
        /// <returns></returns>
        bool Evaluate(IDictionary<string, string> claims);
    }
}