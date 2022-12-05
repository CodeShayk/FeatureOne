using System.Text.Json.Nodes;

namespace FeatureOn.Stores
{
    internal interface IStoreProvider
    {
        string[] Get();
    }

    public class FeatureRaw
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public JsonObject[] Conditions { get; set; }

    }
}