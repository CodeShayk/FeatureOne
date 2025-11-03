using Moq;

namespace FeatureOne.Tests.Stores;

[TestFixture]
public class FeatureStoreTest
{
    [Test]
    public void FeatureStore_ConstructorWithNullProvider_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FeatureStore(null));
        // Test the constructor with storage provider only (uses default logger)
        Assert.Throws<ArgumentNullException>(() => new FeatureStore(null));
    }

    [Test]
    public void FeatureStore_FindStartsWith_PrefixMatching()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();

        // Setup features that start with "Feature" prefix
        var features = new IFeature[]
        {
            new Feature(new FeatureName("FeatureA"), new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })),
            new Feature(new FeatureName("FeatureB"), new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })),
            new Feature(new FeatureName("OtherFeature"), new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }))
        };

        mockProvider.Setup(p => p.GetByName("Feature")).Returns(features);

        var featureStore = new FeatureStore(mockProvider.Object);

        // Act
        var result = featureStore.FindStartsWith("Feature").ToList();

        // Assert - Should find FeatureA and FeatureB but not OtherFeature
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(f => f.Name.Value == "FeatureA"), Is.True);
        Assert.That(result.Any(f => f.Name.Value == "FeatureB"), Is.True);
    }
}