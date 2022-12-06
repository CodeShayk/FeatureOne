using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;
using Moq;
using NUnit.Framework.Internal;

namespace FeatureOne.Tests.Stores
{
    [TestFixture]
    internal class FeatureStoreTests
    {
        private Mock<IStoreProvider> storeProvider;
        private FeatureStore featureStore;
        private Mock<IFeatureLogger> logger;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<IFeatureLogger>();
            storeProvider = new Mock<IStoreProvider>();
            storeProvider.Setup(x => x.Get())
                .Returns(new[]
                {
                    new FeatureRecord("feature-01", "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"),
                    new FeatureRecord("feature-02", "{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}")
                });

            featureStore = new FeatureStore(storeProvider.Object, new ToggleDeserializer(), new FeatureConfiguration
            {
                Logger = logger.Object
            });
        }

        [Test]
        public void TestGetAllToReturnCorrectFeaturesConfiguredStoreInProvider()
        {
            var features = featureStore.GetAll();

            Assert.That(features.Count(), Is.EqualTo(2));

            var feature01 = features.First(x => x.Name.Value == "feature-01");
            Assert.That(feature01.Toggle.Operator, Is.EqualTo(Operator.Any));
            Assert.That(feature01.Toggle.Conditions.Length, Is.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<SimpleCondition>(feature01.Toggle.Conditions[0]);
                Assert.That(((SimpleCondition)feature01.Toggle.Conditions[0]).IsEnabled, Is.EqualTo(true));
            });

            var feature02 = features.First(x => x.Name.Value == "feature-02");
            Assert.That(feature02.Toggle.Operator, Is.EqualTo(Operator.All));
            Assert.That(feature02.Toggle.Conditions.Length, Is.EqualTo(2));

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<SimpleCondition>(feature02.Toggle.Conditions[0]);
                Assert.That(((SimpleCondition)feature02.Toggle.Conditions[0]).IsEnabled, Is.EqualTo(false));
            });
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RegexCondition>(feature02.Toggle.Conditions[1]);
                Assert.That(((RegexCondition)feature02.Toggle.Conditions[1]).Claim, Is.EqualTo("email"));
                Assert.That(((RegexCondition)feature02.Toggle.Conditions[1]).Expression, Is.EqualTo("*@gbk.com"));
            });
        }

        [Test]
        public void TestGetAllToReturnAnyDeserializedFeaturesInStoreProvideAndLogErrorsForFailures()
        {
            storeProvider.Setup(x => x.Get())
               .Returns(new[]
               {
                    new FeatureRecord("feature-01", "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"),
                    new FeatureRecord("feature-02", "Invalid Toggle String")
               });

            var features = featureStore.GetAll();

            Assert.That(features.Count(), Is.EqualTo(1));

            var feature01 = features.First(x => x.Name.Value == "feature-01");
            Assert.That(feature01.Toggle.Operator, Is.EqualTo(Operator.Any));
            Assert.That(feature01.Toggle.Conditions.Length, Is.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<SimpleCondition>(feature01.Toggle.Conditions[0]);
                Assert.That(((SimpleCondition)feature01.Toggle.Conditions[0]).IsEnabled, Is.EqualTo(true));
            });

            logger.Verify(x => x.Error(It.Is<string>(msg => msg.Contains("feature-02"))), Times.Once());
        }
    }
}