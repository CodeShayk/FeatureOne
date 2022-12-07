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
        /// Name - Feature-01
        /// Toggle - Json string as
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
        FeatureRecord[] Get();
    }

    public class FeatureRecord
    {
        public FeatureRecord(string name, string toggle)
        {
            Name = name;
            Toggle = toggle;
        }

        public string Name { get; set; }
        public string Toggle { get; set; }
    }
}