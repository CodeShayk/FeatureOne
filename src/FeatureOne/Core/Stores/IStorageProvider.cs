using System.Collections.Generic;

namespace FeatureOne.Core.Stores
{
    /// <summary>
    /// Interface to implement storage provider.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Implement this method to get all feature toggless from storage.
        /// </summary>
        /// <remarks>
        /// Example:
        /// Key : "Feature-01"
        /// Value : "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"
        /// </remarks>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Get();
    }
}