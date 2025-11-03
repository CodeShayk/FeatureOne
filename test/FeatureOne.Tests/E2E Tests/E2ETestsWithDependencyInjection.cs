using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureOne.Tests.E2ETests
{
    [TestFixture]
    internal class E2ETestsWithDependencyInjection
    {
        [Test]
        public void TestE2EOfServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(services =>
            {
                services.AddConsole();
            });

            var storageProvider = new CustomStoreProvider();

            services.AddFeatureOne(provider => storageProvider);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user", "ninja")
            }));

            var serviceProvider = services.BuildServiceProvider();

            var features = serviceProvider.GetRequiredService<IFeatures>();

            // feature-01 -> simple condition as enabled.
            var isEnabled = features.IsEnabled("feature-01");
            Assert.That(isEnabled, Is.True);
            // feature-01 -> simple condition as enabled. Principal should not affect.
            isEnabled = features.IsEnabled("feature-01", principal);
            Assert.That(isEnabled, Is.True);

            // feature-02 -> simple condition as disabled.
            isEnabled = features.IsEnabled("feature-02");
            Assert.That(isEnabled, Is.False);

            // feature-02 -> simple condition as disabled. Principal should affect only regex condition.
            isEnabled = features.IsEnabled("feature-02", principal);
            Assert.That(isEnabled, Is.False);

            var principal2 = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("email", "ninja@gbk.com")
            }));

            isEnabled = features.IsEnabled("feature-02", principal2);
            Assert.That(isEnabled, Is.True);

            isEnabled = features.IsEnabled("feature-03");
            Assert.That(isEnabled, Is.False);
        }
    }
}