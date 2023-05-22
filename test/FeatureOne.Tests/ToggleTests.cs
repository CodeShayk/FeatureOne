using FeatureOne.Core;
using Moq;

namespace FeatureOne.Test
{
    [TestFixture]
    public class ToggleTests
    {
        [Test]
        public void TestToggle()
        {
            var condition = new Mock<ICondition>();
            var toggle = new Toggle(Operator.All, new[] { condition.Object });

            var claims = new Dictionary<string, string>();
            claims.Add("user", "ninja");

            toggle.Run(claims);

            Assert.That(toggle.Operator, Is.EqualTo(Operator.All));
            Assert.That(toggle.Conditions[0], Is.EqualTo(condition.Object));

            condition.Verify(x => x.Evaluate(claims));
        }
    }
}