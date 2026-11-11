using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;
using Moq;
using NUnit.Framework;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature.Tests
{
    [TestFixture]
    public class EvaluationContextExtensionsTests
    {
        [Test]
        public void ToClaims_WithIntegerAttribute_ShouldMapToInvariantString()
        {
            var context = EvaluationContext.Builder().Set("score", 95).Build();

            var claims = context.ToClaims();

            Assert.That(claims["score"], Is.EqualTo("95"));
        }

        [Test]
        public void ToClaims_WithDoubleAttribute_ShouldMapToInvariantString()
        {
            var context = EvaluationContext.Builder().Set("ratio", 1.5).Build();

            var claims = context.ToClaims();

            Assert.That(claims["ratio"], Is.EqualTo("1.5"));
        }

        [Test]
        public void ToClaims_WithDoubleAttribute_ShouldIgnoreAmbientCulture()
        {
            // A comma decimal separator would break RelationalCondition's numeric parsing.
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var claims = EvaluationContext.Builder().Set("ratio", 1.5).Build().ToClaims();

                Assert.That(claims["ratio"], Is.EqualTo("1.5"));
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Test]
        public void ToClaims_WithStringAttribute_ShouldMapUnchanged()
        {
            var claims = EvaluationContext.Builder().Set("tier", "gold").Build().ToClaims();

            Assert.That(claims["tier"], Is.EqualTo("gold"));
        }

        [Test]
        public void ToClaims_WithBooleanAttribute_ShouldMapToLowerCaseLiteral()
        {
            var claims = EvaluationContext.Builder().Set("isAdmin", true).Set("isGuest", false).Build().ToClaims();

            Assert.That(claims["isAdmin"], Is.EqualTo("true"));
            Assert.That(claims["isGuest"], Is.EqualTo("false"));
        }

        [Test]
        public void ToClaims_WithDateTimeAttribute_ShouldMapToRoundTripFormat()
        {
            var when = new DateTime(2026, 8, 16, 10, 30, 0, DateTimeKind.Utc);

            var claims = EvaluationContext.Builder().Set("since", when).Build().ToClaims();

            Assert.That(claims["since"], Is.EqualTo(when.ToString("o", CultureInfo.InvariantCulture)));
        }

        [Test]
        public void ToClaims_WithListAttribute_ShouldSerializeToJson()
        {
            var context = EvaluationContext.Builder()
                .Set("roles", new Value(new List<Value> { new Value("admin"), new Value("editor") }))
                .Build();

            var claims = context.ToClaims();

            Assert.That(claims["roles"], Is.EqualTo("[\"admin\",\"editor\"]"));
        }

        [Test]
        public void ToClaims_WithStructureAttribute_ShouldSerializeToJson()
        {
            var structure = Structure.Builder()
                .Set("plan", "pro")
                .Set("seats", 5)
                .Build();
            var context = EvaluationContext.Builder().Set("account", new Value(structure)).Build();

            var claims = context.ToClaims();

            // Structure backing storage does not guarantee key order, so assert on the members.
            Assert.That(claims["account"], Does.StartWith("{").And.EndWith("}"));
            Assert.That(claims["account"], Does.Contain("\"plan\":\"pro\""));
            Assert.That(claims["account"], Does.Contain("\"seats\":5"));
        }

        [Test]
        public void ToClaims_WithNullAttribute_ShouldOmitTheClaim()
        {
            var context = EvaluationContext.Builder().Set("maybe", new Value()).Build();

            var claims = context.ToClaims();

            Assert.That(claims.ContainsKey("maybe"), Is.False);
        }

        [Test]
        public void ToClaims_ShouldBeCaseInsensitive()
        {
            var claims = EvaluationContext.Builder().Set("Tier", "gold").Build().ToClaims();

            Assert.That(claims["tier"], Is.EqualTo("gold"));
        }

        [Test]
        public async Task Provider_WithNumericContextAttribute_ShouldEvaluateRelationalCondition()
        {
            // The end-to-end reason the numeric mapping matters: without it the claim never reaches
            // the condition at all and the flag silently evaluates as disabled. Uses Equals so the
            // assertion turns on the mapping rather than on RelationalCondition's comparison semantics.
            var mockStorage = new Mock<IStorageProvider>();
            var feature = new Feature("tier_gate", new Toggle(Operator.Any,
                new RelationalCondition { Claim = "seats", Value = "5", Operator = RelationalOperator.Equals }));
            mockStorage.Setup(x => x.GetByName("tier_gate")).Returns(new IFeature[] { feature });

            var provider = new FeatureOneProvider(new FeatureStore(mockStorage.Object));

            var match = await provider.ResolveBooleanValueAsync("tier_gate", false,
                EvaluationContext.Builder().Set("seats", 5).Build());
            var noMatch = await provider.ResolveBooleanValueAsync("tier_gate", false,
                EvaluationContext.Builder().Set("seats", 9).Build());

            Assert.That(match.Value, Is.True);
            Assert.That(noMatch.Value, Is.False);
        }
    }
}
