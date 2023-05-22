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
        private Mock<IStorageProvider> storeProvider;
        private FeatureStore featureStore;
        private Mock<IFeatureLogger> logger;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<IFeatureLogger>();
            storeProvider = new Mock<IStorageProvider>();
            storeProvider.Setup(x => x.GetByName(It.IsAny<string>()))
                .Returns(new[]
                {
                    new  Feature("feature-01",new Toggle(Operator.Any, new[]{ new SimpleCondition{IsEnabled=true}})),
                    new  Feature("feature-02",new Toggle(Operator.All, new SimpleCondition { IsEnabled = false }, new RegexCondition{Claim="email", Expression= "*@gbk.com" }))
              });

            featureStore = new FeatureStore(storeProvider.Object, logger.Object);
        }

        [Test]
        public void TestFindToReturnCorrectFeaturesConfiguredStoreInProvider()
        {
            var features = featureStore.FindStartsWith("feature");

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
        public void TestFindToReturnAnyDeserializedFeaturesInStoreProvideAndLogErrorsForFailures()
        {
            storeProvider.Setup(x => x.GetByName(It.IsAny<string>()))
               .Returns(new[]
               {
                    new  Feature("feature-01",new Toggle(Operator.Any, new[]{ new SimpleCondition{IsEnabled=true}})),
                    new  Feature("feature-02",new Toggle(Operator.All, null))
                 });

            var features = featureStore.FindStartsWith("feature");

            Assert.That(features.Count(), Is.EqualTo(1));

            var feature01 = features.First(x => x.Name.Value == "feature-01");
            Assert.That(feature01.Toggle.Operator, Is.EqualTo(Operator.Any));
            Assert.That(feature01.Toggle.Conditions.Length, Is.EqualTo(1));

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<SimpleCondition>(feature01.Toggle.Conditions[0]);
                Assert.That(((SimpleCondition)feature01.Toggle.Conditions[0]).IsEnabled, Is.EqualTo(true));
            });
        }
    }
}