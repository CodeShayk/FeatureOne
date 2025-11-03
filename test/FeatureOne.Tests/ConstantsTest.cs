namespace FeatureOne.Tests;

[TestFixture]
public class ConstantsTest
{
    [Test]
    public void Constants_DefaultRegExTimeout_ShouldBeReasonable()
    {
        // Arrange
        var timeout = Constants.DefaultRegExTimeout;
        
        // Act & Assert
        Assert.That(timeout, Is.EqualTo(TimeSpan.FromSeconds(3)));
        Assert.That(timeout.TotalMilliseconds, Is.GreaterThan(0));
    }
}