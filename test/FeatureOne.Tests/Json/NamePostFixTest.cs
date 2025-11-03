namespace FeatureOne.Tests.Json
{
    [TestFixture]
    public class NamePostFixTest
    {
        [TestCase("Regex", "RegexCondition")]
        [TestCase("Simple", "SimpleCondition")]
        [TestCase("RegexCondition", "RegexCondition")]
        [TestCase("SimpleCondition", "SimpleCondition")]
        public void TestNameForSupportedInputs(string input, string output)
        {
            Assert.That(new NamePostFix(input, "Condition").Name, Is.EqualTo(output));
        }

        [Test]
        public void TestNameForNullInputs()
        {
            Assert.Throws<ArgumentNullException>(() => new NamePostFix(null, null));
        }
    }
}