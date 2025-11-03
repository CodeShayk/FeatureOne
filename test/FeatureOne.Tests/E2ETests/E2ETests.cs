using System.Security.Claims;

namespace FeatureOne.Tests.E2ETests
{
    [TestFixture]
    internal class E2ETests
    {
        [Test]
        public void TestE2EOfServices()
        {
            var logger = new ConsoleLogger();
            var storageProvider = new CustomStoreProvider();

            // feature-01 -> simple condition as enabled. Principal should not affect.

            // feature-02 -> consists of two conditions.
            // enabled = (simple condition as disable) OR (regex condition enabled for email with domain gbk.com).
            // Principal affects regex condition included.

            Features.Initialize(() => new Features(new FeatureStore(storageProvider, logger), logger));

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user", "ninja")
            }));

            // feature-01 -> simple condition as enabled.
            var isEnabled = Features.Current.IsEnabled("feature-01");
            Assert.That(isEnabled, Is.True);
            // feature-01 -> simple condition as enabled. Principal should not affect.
            isEnabled = Features.Current.IsEnabled("feature-01", principal);
            Assert.That(isEnabled, Is.True);

            // feature-02 -> simple condition as disabled.
            isEnabled = Features.Current.IsEnabled("feature-02");
            Assert.That(isEnabled, Is.False);

            // feature-02 -> simple condition as disabled. Principal should affect only regex condition.
            isEnabled = Features.Current.IsEnabled("feature-02", principal);
            Assert.That(isEnabled, Is.False);

            var principal2 = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("email", "ninja@gbk.com")
            }));

            isEnabled = Features.Current.IsEnabled("feature-02", principal2);
            Assert.That(isEnabled, Is.True);

            isEnabled = Features.Current.IsEnabled("feature-03");
            Assert.That(isEnabled, Is.False);
        }
    }
}