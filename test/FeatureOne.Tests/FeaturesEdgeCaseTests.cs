using Moq;

namespace FeatureOne.Tests;

[TestFixture]
public class FeaturesEdgeCaseTests
{
    [Test]
    public void IsEnabled_WithNullClaimsDictionary_ShouldStillEvaluateFeature()
    {
        // Arrange
        var mockStore = new Mock<IFeatureStore>();
        var testFeature = new Feature(new FeatureName("TestFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        mockStore.Setup(s => s.FindStartsWith("TestFeature")).Returns(new[] { testFeature });

        var features = new Features(mockStore.Object);

        // Act - pass null claims dictionary
        var result = features.IsEnabled("TestFeature", (IDictionary<string, string>)null);

        // Assert - should still evaluate (SimpleCondition doesn't care about claims)
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEnabled_WithInvalidFeatureName_ShouldReturnFalse()
    {
        // Arrange
        var mockStore = new Mock<IFeatureStore>();
        var mockLogger = new Mock<IFeatureLogger>();
        var features = new Features(mockStore.Object, mockLogger.Object);

        // Act - pass a name with invalid characters
        var result = features.IsEnabled("Invalid Feature Name With Spaces");

        // Assert
        Assert.That(result, Is.False);
        mockStore.Verify(s => s.FindStartsWith(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void IsEnabled_WhenStoreReturnsEmptyList_ShouldReturnFalse()
    {
        // Arrange
        var mockStore = new Mock<IFeatureStore>();
        var mockLogger = new Mock<IFeatureLogger>();

        mockStore.Setup(s => s.FindStartsWith(It.IsAny<string>())).Returns(Array.Empty<IFeature>());

        var features = new Features(mockStore.Object, mockLogger.Object);

        // Act
        var result = features.IsEnabled("ValidFeatureName");

        // Assert
        Assert.That(result, Is.False);
    }
}
