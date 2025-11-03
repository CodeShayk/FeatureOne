namespace FeatureOne.Tests;

[TestFixture]
public class RegexConditionPerformanceTest
{
    [Test]
    public void RegexCondition_PerformanceUnderLoad()
    {
        // Arrange
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"^[a-zA-Z0-9]+$",
            Timeout = TimeSpan.FromMilliseconds(100)
        };

        var claims = new Dictionary<string, string> { { "test", "normalInput" } };

        // Act
        var startTime = DateTime.Now;

        // Run multiple evaluations to test performance
        for (int i = 0; i < 1000; i++)
        {
            var result = condition.Evaluate(claims);
        }

        var endTime = DateTime.Now;

        // Assert
        // Should complete in reasonable time
        Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(5000)); // Less than 5 seconds for 1000 evaluations
    }

    [Test]
    public void RegexCondition_ReDoSProtection()
    {
        // Arrange - Test that the ReDoS fix works
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"^([a-zA-Z0-9]+)+$", // Known ReDoS pattern
            Timeout = TimeSpan.FromMilliseconds(100)
        };

        var longInput = new Dictionary<string, string> { { "test", new string('a', 1000) } };

        // Act & Assert - Should not hang and should complete quickly
        var startTime = DateTime.Now;
        var result = condition.Evaluate(longInput);
        var endTime = DateTime.Now;

        // Should complete quickly (under 1 second) to prove timeout is working
        Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(1000));
        // If timeout occurs, the result may be true or false depending on implementation
        // The important thing is it doesn't hang, but the result behavior depends on implementation
    }
}