namespace FeatureOne.Tests;

[TestFixture]
public class FeatureTestWithNullClaims
{
    [Test]
    public void Feature_EvaluateWithNullClaims_ShouldHandle()
    {
        // Arrange
        var feature = new Feature(new FeatureName("TestFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        // Act
        var result = feature.IsEnabled(null);

        // Assert
        // Behavior depends on implementation, but shouldn't crash
        Assert.That(result, Is.EqualTo(true)); // Simple condition is always true
    }
}