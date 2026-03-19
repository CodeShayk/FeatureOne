using Moq;

namespace FeatureOne.Tests.Stores;

[TestFixture]
public class FeatureStoreEdgeCaseTests
{
    [Test]
    public void FindStartsWith_WhenProviderReturnsNull_ShouldReturnEmpty()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();
        mockProvider.Setup(p => p.GetByName(It.IsAny<string>())).Returns((IFeature[])null);

        var store = new FeatureStore(mockProvider.Object);

        // Act
        var result = store.FindStartsWith("Feature").ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FindStartsWith_WhenProviderReturnsEmpty_ShouldReturnEmpty()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();
        mockProvider.Setup(p => p.GetByName(It.IsAny<string>())).Returns(Array.Empty<IFeature>());

        var store = new FeatureStore(mockProvider.Object);

        // Act
        var result = store.FindStartsWith("Feature").ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FindStartsWith_WhenProviderThrows_ShouldReturnEmpty()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();
        mockProvider.Setup(p => p.GetByName(It.IsAny<string>())).Throws<Exception>();

        var mockLogger = new Mock<IFeatureLogger>();
        var store = new FeatureStore(mockProvider.Object, mockLogger.Object);

        // Act
        var result = store.FindStartsWith("Feature").ToList();

        // Assert
        Assert.That(result, Is.Empty);
        mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    [Test]
    public void FeatureStore_ConstructorWithNullLogger_ShouldThrow()
    {
        var mockProvider = new Mock<IStorageProvider>();

        Assert.Throws<ArgumentNullException>(() => new FeatureStore(mockProvider.Object, null));
    }
}
