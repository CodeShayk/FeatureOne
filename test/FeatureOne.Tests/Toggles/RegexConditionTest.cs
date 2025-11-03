using FeatureOne.Core.Toggles.Conditions;
using NUnit.Framework;
using System.Collections.Generic;

namespace FeatureOne.Test.Toggles
{
    [TestFixture]
    public sealed class RegexConditionTest
    {
        private Dictionary<string, string> claims;
        private const string GmailDotCom = @"[a-zA-Z0-9\.]*@(ninja.com|NINJA.COM)";

        [SetUp]
        public void Setup() => claims = new Dictionary<string, string>();

        [Test]
        public void EvaluateToggleToFalseWhenNoCliamFound()
        {
            var condition = new RegexCondition { Claim = "email", Expression = GmailDotCom };
            Assert.That(!condition.Evaluate(claims));
        }

        [Test]
        public void EvaluateToggleConditionToTrueOnMatchIsHit()
        {
            claims.Add("email", "kl12.sha123@ninja.com");
            var condition = new RegexCondition { Claim = "email", Expression = GmailDotCom };
            Assert.That(condition.Evaluate(claims), Is.EqualTo(true));
        }

        [Test]
        public void EvaluateToggleConditionToFalseOnMatchIsMiss()
        {
            claims.Add("email", "kl12.sha123@yahoo.com");
            var condition = new RegexCondition { Claim = "email", Expression = GmailDotCom };

            Assert.That(condition.Evaluate(claims), Is.EqualTo(false)); // Fixed: was Is.Not.EqualTo(false)
        }
    }
}