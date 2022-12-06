using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;
using System.Text.Json.Nodes;

namespace FeatureOne.Test.Toggles
{
    [TestFixture]
    public sealed class ToggleConditionFactoryTest
    {
        [Test]
        public void TestToggleConditionForNUllInput()
        {
            JsonObject jObj = null;
            Assert.Throws<ArgumentNullException>(() => ConditionFactory.Create(jObj));
        }

        [Test]
        public void TestToggleConditionForCorrectSimpleInstanceType()
        {
            var json = "{\r\n\t\t\t  \"type\":\"Simple\",\r\n\t\t\t  \"IsEnabled\":\"true\"\r\n\t\t}";

            var jobject = JsonObject.Parse(json)?.AsObject();
            var toggleCondition = ConditionFactory.Create(jobject);

            Assert.IsInstanceOf(typeof(SimpleCondition), toggleCondition);
        }

        [Test]
        public void TestToggleConditionForCorrectRegexInstanceType()
        {
            var json = "{\r\n\t\t\t  \"type\":\"RegexCondition\",\r\n\t\t\t  \"claim\":\"email\",\r\n\t\t\t  \"expression\":\"*@gbk.com\"\r\n\t\t  }";

            var jobject = JsonObject.Parse(json)?.AsObject();
            var toggleCondition = ConditionFactory.Create(jobject);

            Assert.IsInstanceOf(typeof(RegexCondition), toggleCondition);
        }
    }
}