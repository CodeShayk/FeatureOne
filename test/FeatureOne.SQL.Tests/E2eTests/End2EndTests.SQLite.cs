using System.Data.Common;
using System.Data.SQLite;
using System.Security.Claims;
using FeatureOne.Core.Stores;
using FeatureOne.SQL.StorageProvider;
using NUnit.Framework.Internal;

namespace FeatureOne.SQL.Tests.E2eTests
{
    public class End2EndTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var names = DbProviderFactories.GetProviderInvariantNames();
            if (!names.Any(x => x.Equals("System.Data.SQLite")) && SQLiteFactory.Instance != null)
                DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);

            var connectionString = $"DataSource={Environment.CurrentDirectory}//Features.db;mode=readonly;cache=shared";

            Console.WriteLine(connectionString);

            var configuration = new SQLConfiguration { ConnectionSettings = new ConnectionSettings { ConnectionString = connectionString, ProviderName = DbProviderName.SQLite } };

            var logger = new E2eLogger();

            var provider = new SQLStorageProvider(configuration);

            Features.Initialize(() => new Features(new FeatureStore(provider, logger), logger));
        }

        [SetUp]
        public void Setup()
        {
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