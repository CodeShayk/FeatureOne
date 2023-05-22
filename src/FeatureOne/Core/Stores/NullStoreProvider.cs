using System;

namespace FeatureOne.Core.Stores
{
    public class NullStoreProvider : IStorageProvider
    {
        public IFeature[] GetByName(string name) => Array.Empty<IFeature>();
    }
}