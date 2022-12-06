using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.Tests.Stores
{
    [TestFixture]
    internal class ToggleDeserializerTests
    {       
        [Test]
        public void TestDeSerialize()
        {
            var deserializer = new ToggleDeserializer();

            var toggle = deserializer
                .Deserializer("{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}");


            Assert.That(toggle.Operator, Is.EqualTo(Operator.All));
            Assert.That(toggle.Conditions.Length, Is.EqualTo(2));

            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<SimpleCondition>(toggle.Conditions[0]);
                Assert.That(((SimpleCondition)toggle.Conditions[0]).IsEnabled, Is.EqualTo(false));
            });
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<RegexCondition>(toggle.Conditions[1]);
                Assert.That(((RegexCondition)toggle.Conditions[1]).Claim, Is.EqualTo("email"));
                Assert.That(((RegexCondition)toggle.Conditions[1]).Expression, Is.EqualTo("*@gbk.com"));
            });

        }
    }
}
