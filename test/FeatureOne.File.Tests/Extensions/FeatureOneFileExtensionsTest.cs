using Microsoft.Extensions.DependencyInjection;
using FeatureOne.File.Extensions;
using FeatureOne.Cache;
using FeatureOne.Json;
using Moq;

namespace FeatureOne.File.Tests.Extensions;

[TestFixture]
public class FeatureOneFileExtensionsTest
{
    [Test]
    public void AddFeatureOneWithFileStorage_WithValidConfiguration_AddsFeatureOneToServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new FileConfiguration { FilePath = "features.json" };

        // Act
        var result = services.AddFeatureOneWithFileStorage(configuration);

        // Assert
        Assert.That(result, Is.EqualTo(services));
        Assert.That(services.Count, Is.GreaterThan(0));
    }

    [Test]
    public void AddFeatureOneWithFileStorage_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        FileConfiguration? configuration = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => services.AddFeatureOneWithFileStorage(configuration));
        
        Assert.That(exception.Message, Does.Contain("FileConfiguration is required."));
    }

    [Test]
    public void AddFeatureOneWithFileStorage_WithCustomDeserializer_UsesCustomDeserializer()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new FileConfiguration { FilePath = "features.json" };
        var mockDeserializer = new Mock<IToggleDeserializer>();

        // Act
        var result = services.AddFeatureOneWithFileStorage(configuration, mockDeserializer.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithFileStorage_WithCustomCache_UsesCustomCache()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new FileConfiguration { FilePath = "features.json" };
        var mockCache = new Mock<ICache>();

        // Act
        var result = services.AddFeatureOneWithFileStorage(configuration, cache: mockCache.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithFileStorage_WithBothCustomDeserializerAndCache_UsesBothCustomServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new FileConfiguration { FilePath = "features.json" };
        var mockDeserializer = new Mock<IToggleDeserializer>();
        var mockCache = new Mock<ICache>();

        // Act
        var result = services.AddFeatureOneWithFileStorage(configuration, mockDeserializer.Object, mockCache.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithFileStorage_WithNullDeserializerAndCache_UsesDefaultServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new FileConfiguration { FilePath = "features.json" };

        // Act
        var result = services.AddFeatureOneWithFileStorage(configuration, null, null);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }
}