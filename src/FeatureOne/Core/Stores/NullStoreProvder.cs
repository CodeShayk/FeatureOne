namespace FeatureOne.Core.Stores
{
    public class NullStoreProvder : IStoreProvider
    {
        public IEnumerable<KeyValuePair<string, string>> Get()
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}