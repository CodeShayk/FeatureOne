using FeatureOne.Core;
using FeatureOne.Core.Toggles.Conditions;
using FeatureOne.Json;

namespace FeatureOne.Tests.Json
{
    [TestFixture]
    internal class ToggleDeserializerTest
    {
        [Test]
        public void TestDeSerialize()
        {
            var deserializer = new ToggleDeserializer();

            var toggle = deserializer
                .Deserialize("{\"operator\":\"all\",\"conditions\":[{\"type\":\"Simple\",\"isEnabled\": false}, {\"type\":\"RegexCondition\",\"claim\":\"email\",\"expression\":\"*@gbk.com\"}]}");

            Assert.That(toggle.Operator, Is.EqualTo(Operator.All));
            Assert.That(toggle.Conditions.Length, Is.EqualTo(2));

            Assert.Multiple(() =>
            {
                Assert.That(toggle.Conditions[0] is SimpleCondition);
                Assert.That(((SimpleCondition)toggle.Conditions[0]).IsEnabled, Is.EqualTo(false));
            });

            Assert.Multiple(() =>
            {
                Assert.That(toggle.Conditions[1] is RegexCondition);
                Assert.That(((RegexCondition)toggle.Conditions[1]).Claim, Is.EqualTo("email"));
                Assert.That(((RegexCondition)toggle.Conditions[1]).Expression, Is.EqualTo("*@gbk.com"));
            });
        }
    }
}