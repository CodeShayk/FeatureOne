using System.Text.Json.Nodes;
using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.SQL.Tests.UnitTests
{
    [TestFixture]
    public sealed class ConditionDeserializerTest
    {
        [Test]
        public void TestToggleConditionForNUllInput()
        {
            JsonObject jObj = null;
            Assert.Throws<ArgumentNullException>(() => new ConditionDeserializer().Deserialize(jObj));
        }

        [Test]
        public void TestToggleConditionForCorrectSimpleInstanceType()
        {
            var json = "{\r\n\t\t\t  \"type\":\"Simple\",\r\n\t\t\t  \"IsEnabled\":\"true\"\r\n\t\t}";

            var jobject = JsonNode.Parse(json)?.AsObject();
            var toggleCondition = new ConditionDeserializer().Deserialize(jobject);

            Assert.IsInstanceOf(typeof(SimpleCondition), toggleCondition);
        }

        [Test]
        public void TestToggleConditionForCorrectRegexInstanceType()
        {
            var json = "{\r\n\t\t\t  \"type\":\"RegexCondition\",\r\n\t\t\t  \"claim\":\"email\",\r\n\t\t\t  \"expression\":\"*@gbk.com\"\r\n\t\t  }";

            var jobject = JsonNode.Parse(json)?.AsObject();
            var toggleCondition = new ConditionDeserializer().Deserialize(jobject);

            Assert.IsInstanceOf(typeof(RegexCondition), toggleCondition);
        }
    }
}