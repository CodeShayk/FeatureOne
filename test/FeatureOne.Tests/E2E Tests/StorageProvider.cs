using FeatureOne.Core.Stores;

namespace FeatureOne.Tests.Registeration
{
    public class StorageProvider : IStorageProvider
    {
        public IEnumerable<KeyValuePair<string, string>> Get()
        {
            return new[]
                {
                    new KeyValuePair<string,string>("feature-01", "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"),
                    new KeyValuePair<string,string>("feature-02", "{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}")
                };
        }
    }
}
