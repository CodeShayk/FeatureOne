
using FeatureOne.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FeatureOne.Tests
{
    public class FeaturesTests
    {
        private Mock<IFeatureStore> store;
        private Mock<IFeatureLogger> logger;
        private Mock<IFeature> feature;
        private Features features;
        private string featureName;
        private ClaimsPrincipal principal;

        [SetUp]
        public void Setup()
        {
            featureName = "Feature-01";
            store = new Mock<IFeatureStore>();
            logger = new Mock<IFeatureLogger>();
            feature = new Mock<IFeature>();

            feature.Setup(x => x.Name).Returns(new FeatureName(featureName));
            feature.Setup(x => x.IsEnabled(It.IsAny<IDictionary<string,string>>())).Returns(true);

            store.Setup(x => x.FindStartsWith(It.IsAny<string>())).Returns(new[] { feature.Object });

            features = new Features(store.Object, new FeatureConfiguration
            {
                Logger = logger.Object,
                SlidingExpiry = TimeSpan.FromSeconds(10),
                UseCache = true
            });

            principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user", "ninja")
            }));
        }

        [Test]
        public void TestIsEnabledWithClaimsWhenFeatureExistsAsEnabledRetureFeatureIsEnabled()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("user", "ninja"));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var output = features.IsEnabled(featureName, principal);

            Assert.That(output, Is.EqualTo(true));
            
            store.Verify(x => x.FindStartsWith(featureName));
            feature.Verify(x => x.IsEnabled(It.IsAny<IDictionary<string, string>>()));
        }

        [Test]
        public void TestIsEnabledWithClaimsWhenFeatureDoesNotExistsReturnFalse()
        {
            featureName = "non-existing-feature";
            var output = features.IsEnabled(featureName, principal);

            Assert.That(output, Is.EqualTo(false));

            store.Verify(x => x.FindStartsWith(featureName));
        }

        [Test]
        public void TestIsEnabledWithExceptionLogErrorReturnFalse()
        {
            featureName = "errored-feature";
            store.Setup(x => x.FindStartsWith(featureName)).Throws<Exception>();

            var output = features.IsEnabled(featureName, principal);

            Assert.That(output, Is.EqualTo(false));

            logger.Verify(x => x.Error(It.Is<string>(msg=> msg.Contains(featureName))));
        }
    }
}
