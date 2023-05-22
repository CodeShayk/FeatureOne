using System.Runtime.Caching;
using FeatureOne.SQL.StorageProvider;
using Moq;

namespace FeatureOne.SQL.Tests.UnitTests
{
    internal class SQLStorageProviderTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestStorageProviderInit()
        {
            var cacheSettings = new CacheSettings();
            var configuration = new SQLConfiguration { CacheSettings = cacheSettings };

            var provider = new SQLStorageProvider(configuration);

            Assert.IsNotNull(provider.cacheSettings);
            Assert.That(provider.cacheSettings, Is.EqualTo(cacheSettings));
            Assert.IsNotNull(provider.repository);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);

            provider = new SQLStorageProvider(configuration, new Mock<IFeatureLogger>().Object);

            Assert.IsNotNull(provider.cacheSettings);
            Assert.That(provider.cacheSettings, Is.EqualTo(cacheSettings));
            Assert.IsNotNull(provider.repository);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);

            provider = new SQLStorageProvider(new SQLConfiguration(), new Mock<IFeatureLogger>().Object, new Mock<ICache>().Object, new Mock<IConditionDeserializer>().Object);

            Assert.IsNotNull(provider.cacheSettings);
            Assert.That(provider.cacheSettings, Is.Not.EqualTo(cacheSettings));
            Assert.IsNotNull(provider.repository);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);

            provider = new SQLStorageProvider(new Mock<IDbRepository>().Object, new Mock<IToggleDeserializer>().Object, new Mock<ICache>().Object, null);

            Assert.IsNotNull(provider.cacheSettings);
            Assert.That(provider.cacheSettings, Is.Not.EqualTo(cacheSettings));
            Assert.IsNotNull(provider.repository);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);
        }

        [Test]
        public void TestGetMethodToUseNoCache()
        {
            var cache = new Mock<ICache>();

            var repository = new Mock<IDbRepository>();
            repository.Setup(x => x.GetByName("Foo")).Returns(new[] { new DbRecord { Name = "Foo", Toggle = "Toggle" } });

            var deserializer = new Mock<IToggleDeserializer>();
            var provider = new SQLStorageProvider(repository.Object, deserializer.Object, cache.Object, new CacheSettings());

            var result = provider.GetByName("Foo");

            repository.Verify(x => x.GetByName("Foo"), Times.Once());
            deserializer.Verify(x => x.Deserialize(It.IsAny<string>()), Times.Once());

            cache.Verify(x => x.Get(It.IsAny<string>()), Times.Never());
            cache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void TestGetMethodToUseCache()
        {
            var dbRecords = new[] { new DbRecord { Name = "Foo", Toggle = "Toggle" } };
            var repository = new Mock<IDbRepository>();
            var cache = new Mock<ICache>();

            repository.Setup(x => x.GetByName("Foo")).Returns(dbRecords);
            var deserializer = new Mock<IToggleDeserializer>();
            var cacheSettings = new CacheSettings { EnableCache = true, ExpiryInMinutes = 4 };

            var provider = new SQLStorageProvider(repository.Object, deserializer.Object, cache.Object, cacheSettings);

            var result = provider.GetByName("Foo");

            repository.Verify(x => x.GetByName("Foo"), Times.Once());
            deserializer.Verify(x => x.Deserialize(It.IsAny<string>()), Times.Once());

            cache.Verify(x => x.Get("Foo"), Times.Once());
            cache.Verify(x => x.Add("Foo", dbRecords, 4), Times.Once());
        }
    }
}