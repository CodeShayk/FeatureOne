using System.Security.Claims;
using FeatureOne.Core.Stores;
using FeatureOne.SQL.StorageProvider;
using NUnit.Framework.Internal;

namespace FeatureOne.SQL.Tests.E2e
{
    public class End2EndTests
    {
        [SetUp]
        public void Setup()
        {
            var connectionString = $"DataSource={Environment.CurrentDirectory}//Features.db;mode=readonly;cache=shared";

            Console.WriteLine(connectionString);

            var configuration = new SQLConfiguration { ConnectionSettings = new ConnectionSettings { ConnectionString = connectionString, ProviderName = DbProviderName.SQLite } };

            var logger = new E2eLogger();

            var provider = new SQLStorageProvider(configuration);

            Features.Initialize(() => new Features(new FeatureStore(provider, logger), logger));
        }

        [Test]
        public void TestForDashboardWidgetToBeEnabled()
        {
            var enabled = Features.Current.IsEnabled("dashboard_widget");
            Assert.IsTrue(enabled);
        }

        [Test]
        public void TestForGBKDashboardToBeEnabledForUsersWithGBKEmails()
        {
            var enabled = Features.Current.IsEnabled("gbk_dashboard");
            Assert.False(enabled);

            var user1_claims = new[] { new Claim("email", "ninja@udt.com") };
            enabled = Features.Current.IsEnabled("gbk_dashboard", user1_claims);
            Assert.False(enabled);

            var user2_claims = new[] { new Claim("email", "ninja@gbk.com") };
            enabled = Features.Current.IsEnabled("gbk_dashboard", user2_claims);
            Assert.True(enabled);
        }
    }
}