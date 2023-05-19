namespace FeatureOne.Core.Stores
{
    /// <summary>
    /// Interface to implement storage provider.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Implement to get storage feature toggles by a given name.
        /// </summary>
        /// <returns>Array of Features</returns>
        IFeature[] GetByName(string name);
    }
}