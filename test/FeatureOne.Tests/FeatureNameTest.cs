namespace FeatureOne.Test
{
    [TestFixture]
    public class FeatureNameTest
    {
        [TestCase("x")]
        [TestCase("x1")]
        [TestCase("x-1")]
        [TestCase("x_1")]
        [TestCase("xY")]
        public void TestNameForSupportedInputs(string input)
        {
            Assert.That(new FeatureName(input).Value, Is.EqualTo(input));
        }

        [TestCase(null)]
        [TestCase("x~")]
        [TestCase("x/")]
        [TestCase(@"x\")]
        [TestCase("x/99")]
        [TestCase("x(")]
        public void TestNameForUnsupportedInputs(string input)
        {
            Assert.Throws<ArgumentException>(() => new FeatureName(input));
        }
    }
}