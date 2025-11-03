using Moq;

namespace FeatureOne.Tests.E2ETests;

[TestFixture]
public class EndToEndCoreTest
{
    [Test]
    public void Integration_EndToEnd_CoreFunctionality()
    {
        // Arrange - Test core end-to-end functionality using mocks for storage
        var mockProvider = new Mock<IStorageProvider>();
        var testFeature = new Feature(new FeatureName("TestFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        mockProvider.Setup(p => p.GetByName("TestFeature")).Returns(new[] { testFeature });

        var featureStore = new FeatureStore(mockProvider.Object); // Uses default logger
        var features = new Features(featureStore); // Uses default logger

        // Act
        var result = features.IsEnabled("TestFeature");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Integration_EndToEnd_WithClaims()
    {
        // Arrange - Test with claims-based evaluation
        var mockProvider = new Mock<IStorageProvider>();
        var testFeature = new Feature(new FeatureName("ClaimBasedFeature"),
            new Toggle(Operator.Any, new RegexCondition
            {
                Claim = "role",
                Expression = "^admin$"
            }));

        mockProvider.Setup(p => p.GetByName("ClaimBasedFeature")).Returns(new[] { testFeature });

        var featureStore = new FeatureStore(mockProvider.Object); // Uses default logger
        var features = new Features(featureStore); // Uses default logger

        // Act - Should return true for admin role
        var adminClaims = new Dictionary<string, string> { ["role"] = "admin" };
        var adminResult = features.IsEnabled("ClaimBasedFeature", adminClaims);

        // Act - Should return false for user role
        var userClaims = new Dictionary<string, string> { ["role"] = "user" };
        var userResult = features.IsEnabled("ClaimBasedFeature", userClaims);

        // Assert
        Assert.That(adminResult, Is.True);
        Assert.That(userResult, Is.False);
    }
}