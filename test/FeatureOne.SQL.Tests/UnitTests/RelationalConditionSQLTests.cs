using System.Security.Claims;
using FeatureOne.Core.Stores;
using FeatureOne.Json;
using FeatureOne.SQL.StorageProvider;
using Moq;

namespace FeatureOne.SQL.Tests.UnitTests
{
    [TestFixture]
    public class RelationalConditionSQLTests
    {
        private Features _features;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var repository = new Mock<IDbRepository>();

            repository.Setup(x => x.GetByName(It.Is<string>(n => n.StartsWith("tier_feature"))))
                .Returns(new[]
                {
                    new DbRecord
                    {
                        Name = "tier_feature",
                        Toggle = @"{""conditions"":[{""type"":""Relational"",""claim"":""tier"",""operator"":""GreaterThanOrEqual"",""value"":""gold""}]}"
                    }
                });

            var provider = new SQLStorageProvider(repository.Object, new ToggleDeserializer(new ConditionDeserializer()), new FeatureOne.Cache.FeatureCache(), null);

            _features = new Features(new FeatureStore(provider));
        }

        [Test]
        public void TierFeature_WhenTierIsBronze_ShouldBeDisabled()
        {
            var claims = new[] { new Claim("tier", "bronze") };
            Assert.That(_features.IsEnabled("tier_feature", claims), Is.False);
        }

        [Test]
        public void TierFeature_WhenTierIsGold_ShouldBeEnabled()
        {
            var claims = new[] { new Claim("tier", "gold") };
            Assert.That(_features.IsEnabled("tier_feature", claims), Is.True);
        }

        [Test]
        public void TierFeature_WhenTierIsPlatinum_ShouldBeEnabled()
        {
            var claims = new[] { new Claim("tier", "platinum") };
            Assert.That(_features.IsEnabled("tier_feature", claims), Is.True);
        }

        [Test]
        public void TierFeature_WhenNoTierClaim_ShouldBeDisabled()
        {
            var claims = new[] { new Claim("email", "user@example.com") };
            Assert.That(_features.IsEnabled("tier_feature", claims), Is.False);
        }
    }
}
