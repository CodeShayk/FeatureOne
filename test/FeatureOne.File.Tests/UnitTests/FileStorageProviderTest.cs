using System.Runtime.Caching;
using FeatureOne.Cache;
using FeatureOne.File;
using FeatureOne.Json;
using FeatureOne.File.StorageProvider;
using Moq;

namespace FeatureOne.SQL.Tests.UnitTests
{
    internal class FileStorageProviderTest
    {
        private string filePath = $"{Environment.CurrentDirectory}//Features.json";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestStorageProviderInit()
        {
            var cacheSettings = new CacheSettings();
            var configuration = new FileConfiguration { FilePath = filePath, CacheSettings = cacheSettings };

            var provider = new FileStorageProvider(configuration);

            Assert.That(provider.configuration, Is.EqualTo(configuration));
            Assert.IsNotNull(provider.reader);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);

            provider = new FileStorageProvider(configuration, new Mock<ICache>().Object, new Mock<IConditionDeserializer>().Object);

            Assert.That(provider.configuration, Is.EqualTo(configuration));
            Assert.IsNotNull(provider.reader);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);

            provider = new FileStorageProvider(configuration, new Mock<IFileReader>().Object, new Mock<IToggleDeserializer>().Object, new Mock<ICache>().Object);

            Assert.That(provider.configuration, Is.EqualTo(configuration));
            Assert.IsNotNull(provider.reader);
            Assert.IsNotNull(provider.deserializer);
            Assert.IsNotNull(provider.cache);
        }

        [Test]
        public void TestGetMethodToUseNoCache()
        {
            var cache = new Mock<ICache>();

            var reader = new Mock<IFileReader>();
            reader.Setup(x => x.Read()).Returns(new FileState { Records = new[] { new FileRecord("feature1", "toggle") } });

            var deserializer = new Mock<IToggleDeserializer>();
            var provider = new FileStorageProvider(new FileConfiguration { FilePath = filePath }, reader.Object, deserializer.Object, cache.Object);

            var result = provider.GetByName("Foo");

            reader.Verify(x => x.Read(), Times.Once());
            deserializer.Verify(x => x.Deserialize(It.IsAny<string>()), Times.Once());

            cache.Verify(x => x.Get(It.IsAny<string>()), Times.Never());
            cache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheItemPolicy>()), Times.Never());
        }

        [Test]
        public void TestGetMethodToUseCache()
        {
            var repository = new Mock<IFileReader>();
            var cache = new Mock<ICache>();
            var deserializer = new Mock<IToggleDeserializer>();

            var fileState = new FileState { Records = new[] { new FileRecord("feature1", "toggle") } };
            repository.Setup(x => x.Read()).Returns(fileState);

            var cacheSettings = new CacheSettings { EnableCache = true, Expiry = new CacheExpiry { InMinutes = 4 } };
            var configuration = new FileConfiguration { FilePath = filePath, CacheSettings = cacheSettings };

            var provider = new FileStorageProvider(configuration, repository.Object, deserializer.Object, cache.Object);

            var result = provider.GetByName("Foo");

            repository.Verify(x => x.Read(), Times.Once());
            deserializer.Verify(x => x.Deserialize(It.IsAny<string>()), Times.Once());

            cache.Verify(x => x.Get(FileStorageProvider.cacheKey), Times.Once());
            cache.Verify(x => x.Add(FileStorageProvider.cacheKey, fileState, It.Is<CacheItemPolicy>(x => x.ChangeMonitors.Any())), Times.Once());
        }
    }
}