namespace FeatureOne.Tests.Toggles;

[TestFixture]
public class ToggleOperatorTest
{
    [Test]
    public void Toggle_DifferentOperators_ShouldWorkCorrectly()
    {
        // Test ANY operator with one true condition
        var toggleAny = new Toggle(Operator.Any,
            new SimpleCondition { IsEnabled = false },
            new SimpleCondition { IsEnabled = true });

        Assert.That(toggleAny.Run(new Dictionary<string, string>()), Is.True);

        // Test ANY operator with all false conditions
        var toggleAnyAllFalse = new Toggle(Operator.Any,
            new SimpleCondition { IsEnabled = false },
            new SimpleCondition { IsEnabled = false });

        Assert.That(toggleAnyAllFalse.Run(new Dictionary<string, string>()), Is.False);

        // Test ALL operator with all true conditions
        var toggleAll = new Toggle(Operator.All,
            new SimpleCondition { IsEnabled = true },
            new SimpleCondition { IsEnabled = true });

        Assert.That(toggleAll.Run(new Dictionary<string, string>()), Is.True);

        // Test ALL operator with one false condition
        var toggleAllOneFalse = new Toggle(Operator.All,
            new SimpleCondition { IsEnabled = true },
            new SimpleCondition { IsEnabled = false });

        Assert.That(toggleAllOneFalse.Run(new Dictionary<string, string>()), Is.False);
    }

    [Test]
    public void Toggle_WithNullClaims_ShouldHandleGracefully()
    {
        // Arrange
        var toggle = new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true });

        // Act & Assert
        // Should not throw with null claims
        var result = toggle.Run(null);
        // Simple condition with null claims should return the condition result (true in this case)
        Assert.That(result, Is.True); // Simple condition is always true regardless of claims
    }
}