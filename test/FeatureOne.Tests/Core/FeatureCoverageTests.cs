namespace FeatureOne.Tests.Core;

[TestFixture]
public class FeatureCoverageTests
{
    // Derived class to exercise protected constructor
    private class TestableFeature : Feature
    {
        public TestableFeature() : base()
        {
        }

        public void SetNameAndToggle(FeatureName name, IToggle toggle)
        {
            Name = name;
            Toggle = toggle;
        }
    }

    [Test]
    public void Feature_ProtectedConstructor_ShouldCreateInstance()
    {
        var feature = new TestableFeature();
        feature.SetNameAndToggle(new FeatureName("TestFeature"),
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));

        Assert.That(feature.Name.Value, Is.EqualTo("TestFeature"));
        Assert.That(feature.IsEnabled(new Dictionary<string, string>()), Is.True);
    }
}
