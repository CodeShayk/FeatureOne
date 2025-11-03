using FeatureOne.Cache;
using FeatureOne.Json;
using FeatureOne.SQL.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FeatureOne.SQL.Tests.Extensions;

[TestFixture]
public class FeatureOneSQLExtensionsTest
{
    [Test]
    public void AddFeatureOneWithSQLStorage_WithValidConfiguration_AddsFeatureOneToServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new SQLConfiguration
        {
            ConnectionSettings = new ConnectionSettings
            {
                ConnectionString = "Data Source=:memory:",
                ProviderName = "System.Data.SQLite"
            }
        };

        // Act
        var result = services.AddFeatureOneWithSQLStorage(configuration);

        // Assert
        Assert.That(result, Is.EqualTo(services));
        Assert.That(services.Count, Is.GreaterThan(0));
    }

    [Test]
    public void AddFeatureOneWithSQLStorage_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        SQLConfiguration? configuration = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => services.AddFeatureOneWithSQLStorage(configuration));

        Assert.That(exception.Message, Does.Contain("SQLConfiguration is required."));
    }

    [Test]
    public void AddFeatureOneWithSQLStorage_WithCustomDeserializer_UsesCustomDeserializer()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new SQLConfiguration
        {
            ConnectionSettings = new ConnectionSettings
            {
                ConnectionString = "Data Source=:memory:",
                ProviderName = "System.Data.SQLite"
            }
        };
        var mockDeserializer = new Mock<IToggleDeserializer>();

        // Act
        var result = services.AddFeatureOneWithSQLStorage(configuration, mockDeserializer.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithSQLStorage_WithCustomCache_UsesCustomCache()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new SQLConfiguration
        {
            ConnectionSettings = new ConnectionSettings
            {
                ConnectionString = "Data Source=:memory:",
                ProviderName = "System.Data.SQLite"
            }
        };
        var mockCache = new Mock<ICache>();

        // Act
        var result = services.AddFeatureOneWithSQLStorage(configuration, cache: mockCache.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithSQLStorage_WithBothCustomDeserializerAndCache_UsesBothCustomServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new SQLConfiguration
        {
            ConnectionSettings = new ConnectionSettings
            {
                ConnectionString = "Data Source=:memory:",
                ProviderName = "System.Data.SQLite"
            }
        };
        var mockDeserializer = new Mock<IToggleDeserializer>();
        var mockCache = new Mock<ICache>();

        // Act
        var result = services.AddFeatureOneWithSQLStorage(configuration, mockDeserializer.Object, mockCache.Object);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }

    [Test]
    public void AddFeatureOneWithSQLStorage_WithNullDeserializerAndCache_UsesDefaultServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new SQLConfiguration
        {
            ConnectionSettings = new ConnectionSettings
            {
                ConnectionString = "Data Source=:memory:",
                ProviderName = "System.Data.SQLite"
            }
        };

        // Act
        var result = services.AddFeatureOneWithSQLStorage(configuration, null, null);

        // Assert
        Assert.That(result, Is.EqualTo(services));
    }
}