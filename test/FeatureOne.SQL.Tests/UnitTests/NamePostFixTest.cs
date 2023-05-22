namespace FeatureOne.SQL.Tests.UnitTests
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

        [TestCase(null, null)]
        public void TestNameForNullInputs(string input, string postfix)
        {
            Assert.Throws<ArgumentNullException>(() => new NamePostFix(input, postfix));
        }
    }
}