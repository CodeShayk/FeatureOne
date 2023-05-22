using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.Tests
{
    public class CustomStoreProvider : IStorageProvider
    {
        private List<Tuple<string, Feature>> list = new List<Tuple<string, Feature>>()
        {
            new Tuple<string, Feature>("feature-01", new  Feature("feature-01",new Toggle(Operator.Any, new[]{ new SimpleCondition{IsEnabled=true}}))),
            new Tuple<string, Feature>("feature-02", new  Feature("feature-02",new Toggle(Operator.Any, new SimpleCondition { IsEnabled = false }, new RegexCondition{Claim="email", Expression= "^[a-zA-Z0-9_.+-]+@gbk.com" }))),
            new Tuple<string, Feature>("feature-03", new  Feature("feature-03",new Toggle(Operator.Any, new[]{ new ReleaseOnCondition{ ReleaseOn=new DateTime(3022,12, 1)}}))),
        };

        public IFeature[] GetByName(string name)
        {
            return list.Where(t => t.Item1 == name).Select(f => f.Item2).ToArray();
        }
    }
}