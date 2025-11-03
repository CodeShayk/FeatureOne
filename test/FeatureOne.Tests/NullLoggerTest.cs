namespace FeatureOne.Tests;

[TestFixture]
public class DefaultLoggerTest
{
    [Test]
    public void DefaultLogger_ShouldNotThrow()
    {
        // Arrange
        var logger = new DefaultLogger(null); // Pass null as the ILogger service
        var testMessage = "Test message";
        var testException = new Exception("Test exception");

        // Act & Assert
        Assert.DoesNotThrow(() => logger.Info(testMessage));
        Assert.DoesNotThrow(() => logger.Debug(testMessage));
        Assert.DoesNotThrow(() => logger.Warn(testMessage));
        Assert.DoesNotThrow(() => logger.Error(testMessage, null)); // DefaultLogger.Error requires exception parameter
        Assert.DoesNotThrow(() => logger.Error(testMessage, testException));
    }
}