namespace FeatureOne.Test.Toggles
{
    [TestFixture]
    public sealed class SimpleConditionTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Evaluate_returns_IsEnabled(bool isEnabled)
        {
            var toggle = new SimpleCondition { IsEnabled = isEnabled };
            Assert.That(toggle.Evaluate(new Dictionary<string, string>()), Is.EqualTo(isEnabled));
        }
    }
}