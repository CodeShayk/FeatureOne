using Moq;

namespace FeatureOne.Tests;

[TestFixture]
public class BackwardCompatibilityTest
{
    [Test]
    public void Integration_BackwardCompatibility_ExistingFeatures()
    {
        // Arrange - Test that existing feature configurations still work
        var mockProvider = new Mock<IStorageProvider>();
        var oldStyleFeature = new Feature(new FeatureName("LegacyFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        mockProvider.Setup(p => p.GetByName("LegacyFeature")).Returns(new[] { oldStyleFeature });

        var featureStore = new FeatureStore(mockProvider.Object);
        var features = new Features(featureStore);

        // Act
        var result = features.IsEnabled("LegacyFeature");

        // Assert
        Assert.That(result, Is.True);

        // Also test with claims
        var claimsResult = features.IsEnabled("LegacyFeature", new Dictionary<string, string> { { "role", "user" } });
        Assert.That(claimsResult, Is.True);
    }

    [Test]
    public void Integration_NewFeature_DateRangeCondition()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();

        // Create a feature with DateRangeCondition
        var dateRangeFeature = new Feature(new FeatureName("TimeBasedFeature"),
            new Toggle(Operator.Any, new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            }));

        mockProvider.Setup(p => p.GetByName("TimeBasedFeature")).Returns(new[] { dateRangeFeature });

        var featureStore = new FeatureStore(mockProvider.Object);
        var features = new Features(featureStore);

        // Act
        var result = features.IsEnabled("TimeBasedFeature");

        // Assert - Should be within date range
        Assert.That(result, Is.True);
    }

    [Test]
    public void Integration_SecurityFix_ReDoSProtection()
    {
        // Arrange - Test that the ReDoS fix works in integration
        var mockProvider = new Mock<IStorageProvider>();

        // Create a feature with a regex that would cause ReDoS in old version
        var regexFeature = new Feature(new FeatureName("ReDosProtectedFeature"),
            new Toggle(Operator.Any, new RegexCondition
            {
                Claim = "test",
                Expression = @"^([a-zA-Z0-9]+)+$", // Known ReDoS pattern
                Timeout = TimeSpan.FromMilliseconds(100)
            }));

        mockProvider.Setup(p => p.GetByName("ReDosProtectedFeature")).Returns(new[] { regexFeature });

        var featureStore = new FeatureStore(mockProvider.Object);
        var features = new Features(featureStore);

        // Act & Assert - Should not hang and should complete quickly
        var startTime = DateTime.Now;
        var result = features.IsEnabled("ReDosProtectedFeature", new Dictionary<string, string> { { "test", new string('a', 1000) } });
        var endTime = DateTime.Now;

        // Should complete quickly (under 1 second) to prove timeout is working
        Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(1000));
        // The result may vary depending on implementation, but the important thing is no hang
    }

    [Test]
    public void Integration_Performance_ConcurrentAccess()
    {
        // Arrange
        var mockProvider = new Mock<IStorageProvider>();
        var testFeature = new Feature(new FeatureName("ConcurrentTestFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        mockProvider.Setup(p => p.GetByName("ConcurrentTestFeature")).Returns(new[] { testFeature });

        var featureStore = new FeatureStore(mockProvider.Object);
        var features = new Features(featureStore);

        // Act - Run multiple concurrent evaluations
        var tasks = new List<Task<bool>>();
        var startTime = DateTime.Now;

        for (int i = 0; i < 50; i++)
        {
            var task = Task.Run(() => features.IsEnabled("ConcurrentTestFeature"));
            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
        var endTime = DateTime.Now;

        // Assert - All should return true, and should complete in reasonable time
        Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(5000)); // Should complete in under 5 seconds
        Assert.That(tasks.All(t => t.Result), Is.True);
    }
}