namespace FeatureOne.Tests.Core;

[TestFixture]
public class ToggleCoverageTests
{
    [Test]
    public void Toggle_DefaultConstructor_ShouldCreateWithAnyOperatorAndEmptyConditions()
    {
        var toggle = new Toggle();

        Assert.That(toggle.Operator, Is.EqualTo(Operator.Any));
        Assert.That(toggle.Conditions, Is.Not.Null);
        Assert.That(toggle.Conditions, Is.Empty);
    }

    [Test]
    public void Toggle_WithNullConditionsArray_ShouldUseEmptyArray()
    {
        var toggle = new Toggle(Operator.All, (ICondition[])null);

        Assert.That(toggle.Conditions, Is.Not.Null);
        Assert.That(toggle.Conditions, Is.Empty);
    }

    [Test]
    public void Toggle_Run_WithNullConditions_ShouldReturnFalse()
    {
        // Set Conditions to null via the property setter after construction
        var toggle = new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true });
        toggle.Conditions = null;

        var result = toggle.Run(new Dictionary<string, string>());

        Assert.That(result, Is.False);
    }

    [Test]
    public void Toggle_Run_WithEmptyConditions_AndAnyOperator_ShouldReturnFalse()
    {
        var toggle = new Toggle(); // default - empty conditions

        var result = toggle.Run(new Dictionary<string, string>());

        Assert.That(result, Is.False);
    }
}
