namespace FeatureOne.Core.Stores
{
    /// <summary>
    /// Interface to implement feature store provider.
    /// </summary>
    public interface IStoreProvider
    {
        /// <summary>
        /// Implement this method to return all features from store provider.
        /// </summary>
        /// <remarks>
        /// Example:
        /// Key - Feature-01
        /// Value - Json string as
        /// {
		///   "operator":"all",
		///    "conditions":[{
		///	        "type":"Regex",
		///	        "claim":"email",
		///	        "expression":"*@gbk.com"
        ///     },
		///     {
		///	        "type":"RegexCondition",
		///	        "claim":"user_role",
		///	        "expression":"^administrator$"
		///     }]
	    /// }
        /// </remarks>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Get();
    }
}