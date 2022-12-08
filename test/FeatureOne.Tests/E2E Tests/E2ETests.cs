using FeatureOne.Core.Stores;
using System.Security.Claims;

namespace FeatureOne.Tests.Registeration
{
    [TestFixture]
    internal class E2ETests
    {
                        
        [Test]
        public void TestE2EOfServices()
        {
            var logger = new ConsoleLogger();
            var storageProvider = new StorageProvider();

            Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));

            
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user", "ninja")
            }));

            var isEnabled = Features.Current.IsEnabled("feature-01", principal);

            Assert.That(isEnabled, Is.True);
        }        
    }
}
