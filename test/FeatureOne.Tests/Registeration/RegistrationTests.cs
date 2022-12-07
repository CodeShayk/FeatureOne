using Castle.DynamicProxy.Generators;
using FeatureOne.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FeatureOne.Tests.Registeration
{
    [TestFixture]
    internal class RegistrationTests
    {
        private Mock<IStoreProvider> storeProvider;
        private Mock<IFeatureLogger> logger;
        private ServiceProvider serviceProvider;

        [SetUp]
        public void Setup() {

            logger = new Mock<IFeatureLogger>();
            storeProvider = new Mock<IStoreProvider>();
            storeProvider.Setup(x => x.Get())
                .Returns(new[]
                {
                    new KeyValuePair<string,string>("feature-01", "{\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": true}]}"),
                    new KeyValuePair<string,string>("feature-02", "{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}")
                });

            var services = new ServiceCollection();
            services.UseFeatureOne(new Configuration
            {
                StoreProvider = storeProvider.Object,
                Logger= logger.Object
            });

            serviceProvider = services.BuildServiceProvider();


        }

        [Test]
        public void TestRegistration()
        {
            var features = serviceProvider.GetService<Features>();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim("user", "ninja")
            }));

            var isEnabled =  features.IsEnabled("feature-01", principal);

            Assert.That(isEnabled, Is.True);
        }
    }
}
