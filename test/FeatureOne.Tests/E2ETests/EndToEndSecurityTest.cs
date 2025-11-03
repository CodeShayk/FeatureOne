using Moq;

namespace FeatureOne.Tests.E2ETests;

[TestFixture]
public class EndToEndSecurityTest
{
    [Test]
    public void Integration_SecurityReDoSProtection()
    {
        // Arrange - Test ReDoS protection in a full end-to-end scenario
        var mockProvider = new Mock<IStorageProvider>();

        // Create a feature with a regex that could cause ReDoS in older versions
        var vulnerableFeature = new Feature(new FeatureName("VulnerableFeature"),
            new Toggle(Operator.Any, new RegexCondition
            {
                Claim = "test",
                Expression = @"^([a-zA-Z0-9]+)+$", // Known ReDoS vulnerable pattern
                Timeout = TimeSpan.FromMilliseconds(100) // Set a short timeout
            }));

        mockProvider.Setup(p => p.GetByName("VulnerableFeature")).Returns(new[] { vulnerableFeature });

        var featureStore = new FeatureStore(mockProvider.Object); // Uses default logger
        var features = new Features(featureStore); // Uses default logger

        // Act - Test with a long string that could cause hang
        var longClaims = new Dictionary<string, string> { ["test"] = new string('a', 1000) };
        var startTime = DateTime.Now;
        var result = features.IsEnabled("VulnerableFeature", longClaims);
        var endTime = DateTime.Now;

        // Assert - Should complete within timeout (prove no hang)
        var elapsedMs = (endTime - startTime).TotalMilliseconds;
        Assert.That(elapsedMs, Is.LessThan(500)); // Should be well under timeout
        // Result depends on implementation, but the key is no hang
    }

    [Test]
    public void Integration_DateRangeCondition_EndToEnd()
    {
        // Arrange - Test DateRangeCondition in an end-to-end scenario
        var mockProvider = new Mock<IStorageProvider>();

        // Create a feature with a date range that should be active now
        var dateRangeFeature = new Feature(new FeatureName("TimeBasedFeature"),
            new Toggle(Operator.Any, new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            }));

        mockProvider.Setup(p => p.GetByName("TimeBasedFeature")).Returns(new[] { dateRangeFeature });

        var featureStore = new FeatureStore(mockProvider.Object); // Uses default logger
        var features = new Features(featureStore); // Uses default logger

        // Act
        var result = features.IsEnabled("TimeBasedFeature");

        // Assert - Should be enabled since we're within the date range
        Assert.That(result, Is.True);
    }
}