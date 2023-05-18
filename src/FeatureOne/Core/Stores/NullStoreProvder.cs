using System.Collections.Generic;
using System.Linq;

namespace FeatureOne.Core.Stores
{
    public class NullStoreProvder : IStorageProvider
    {
        public IEnumerable<KeyValuePair<string, string>> Get()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}