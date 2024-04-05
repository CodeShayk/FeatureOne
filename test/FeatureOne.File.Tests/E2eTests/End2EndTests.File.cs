using System.Security.Claims;
using FeatureOne.Core.Stores;
using FeatureOne.File.StorageProvider;
using NUnit.Framework.Internal;

namespace FeatureOne.File.Tests.E2eTests
{
    public class End2EndTests
    {
        [SetUp]
        public void Setup()
        {
            var filePath = $"{Environment.CurrentDirectory}//Features.json";

            Console.WriteLine(filePath);

            var configuration = new FileConfiguration { FilePath = filePath };

            var logger = new E2eLogger();

            var provider = new FileStorageProvider(configuration);

            Features.Initialize(() => new Features(new FeatureStore(provider, logger), logger));
        }

        [Test]
        public void TestForDashboardWidgetToBeEnabled()
        {
            var enabled = Features.Current.IsEnabled("dashboard_widget");
            Assert.That(enabled == true);
        }

        [Test]
        public void TestForGBKDashboardToBeEnabledForUsersWithGBKEmails()
        {
            var enabled = Features.Current.IsEnabled("gbk_dashboard");
            Assert.That(enabled == false);

            var user1_claims = new[] { new Claim("email", "ninja@udt.com") };
            enabled = Features.Current.IsEnabled("gbk_dashboard", user1_claims);
            Assert.That(enabled == false);

            var user2_claims = new[] { new Claim("email", "ninja@gbk.com") };
            enabled = Features.Current.IsEnabled("gbk_dashboard", user2_claims);
            Assert.That(enabled == true);
        }
    }
}