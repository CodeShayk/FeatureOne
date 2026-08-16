using FeatureOne.Core.Toggles.Conditions;

namespace FeatureOne.Tests.Toggles.Conditions;

[TestFixture]
public class RelationalConditionTests
{
    // ──────────────────────────────────────────────
    // Null / missing-claim guard tests
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_WithNullClaims_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = "admin" };

        Assert.That(condition.Evaluate(null), Is.False);
    }

    [Test]
    public void Evaluate_WhenClaimNotPresent_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = "admin" };
        var claims = new Dictionary<string, string> { { "email", "user@example.com" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    // ──────────────────────────────────────────────
    // Equals
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_Equals_WhenValuesMatch_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = "admin" };
        var claims = new Dictionary<string, string> { { "role", "admin" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_Equals_WhenValuesDiffer_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = "admin" };
        var claims = new Dictionary<string, string> { { "role", "user" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    [Test]
    public void Evaluate_Equals_TrimsWhitespace()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = " admin " };
        var claims = new Dictionary<string, string> { { "role", " admin " } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    // ──────────────────────────────────────────────
    // NotEquals
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_NotEquals_WhenValuesDiffer_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.NotEquals, Value = "admin" };
        var claims = new Dictionary<string, string> { { "role", "user" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_NotEquals_WhenValuesMatch_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.NotEquals, Value = "admin" };
        var claims = new Dictionary<string, string> { { "role", "admin" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    // ──────────────────────────────────────────────
    // GreaterThan
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_GreaterThan_WhenClaimIsGreater_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThan, Value = "bronze" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_GreaterThan_WhenClaimIsEqual_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThan, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    [Test]
    public void Evaluate_GreaterThan_WhenClaimIsLess_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThan, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "bronze" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    // ──────────────────────────────────────────────
    // GreaterThanOrEqual
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_GreaterThanOrEqual_WhenClaimIsGreater_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThanOrEqual, Value = "bronze" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_GreaterThanOrEqual_WhenClaimIsEqual_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThanOrEqual, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_GreaterThanOrEqual_WhenClaimIsLess_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThanOrEqual, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "bronze" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    // ──────────────────────────────────────────────
    // LessThanOrEqual
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_LessThanOrEqual_WhenClaimIsLess_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThanOrEqual, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "bronze" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_LessThanOrEqual_WhenClaimIsEqual_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThanOrEqual, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_LessThanOrEqual_WhenClaimIsGreater_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThanOrEqual, Value = "bronze" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    // ──────────────────────────────────────────────
    // LessThan
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_LessThan_WhenClaimIsLess_ShouldReturnTrue()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThan, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "bronze" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_LessThan_WhenClaimIsEqual_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThan, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    [Test]
    public void Evaluate_LessThan_WhenClaimIsGreater_ShouldReturnFalse()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThan, Value = "bronze" };
        var claims = new Dictionary<string, string> { { "tier", "gold" } };

        Assert.That(condition.Evaluate(claims), Is.False);
    }

    [Test]
    public void Evaluate_LessThan_WithNumericClaim_ShouldCompareNumerically()
    {
        var condition = new RelationalCondition { Claim = "age", Operator = RelationalOperator.LessThan, Value = "18" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "age", "9" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "age", "21" } }), Is.False);
    }

    // ──────────────────────────────────────────────
    // Numeric comparison
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_GreaterThan_WithNumericClaim_ShouldNotCompareLexicographically()
    {
        // Digit-by-digit ordering would rank "9" above "18"; arithmetic ordering must not.
        var condition = new RelationalCondition { Claim = "age", Operator = RelationalOperator.GreaterThan, Value = "18" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "age", "9" } }), Is.False);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "age", "21" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "age", "100" } }), Is.True);
    }

    [Test]
    public void Evaluate_GreaterThanOrEqual_WithNumericClaim_ShouldCompareNumerically()
    {
        var condition = new RelationalCondition { Claim = "seats", Operator = RelationalOperator.GreaterThanOrEqual, Value = "5" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "seats", "5" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "seats", "40" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "seats", "4" } }), Is.False);
    }

    [Test]
    public void Evaluate_LessThanOrEqual_WithNumericClaim_ShouldCompareNumerically()
    {
        var condition = new RelationalCondition { Claim = "score", Operator = RelationalOperator.LessThanOrEqual, Value = "90" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "score", "9" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "score", "90" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "score", "91" } }), Is.False);
    }

    [Test]
    public void Evaluate_WithDecimalClaim_ShouldCompareNumerically()
    {
        var condition = new RelationalCondition { Claim = "ratio", Operator = RelationalOperator.GreaterThan, Value = "1.5" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "ratio", "1.75" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "ratio", "1.25" } }), Is.False);
    }

    [Test]
    public void Evaluate_WithNegativeNumbers_ShouldCompareNumerically()
    {
        var condition = new RelationalCondition { Claim = "balance", Operator = RelationalOperator.GreaterThan, Value = "-10" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "balance", "-5" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "balance", "-50" } }), Is.False);
    }

    [Test]
    public void Evaluate_Equals_WithEquivalentNumericForms_ShouldMatch()
    {
        var condition = new RelationalCondition { Claim = "seats", Operator = RelationalOperator.Equals, Value = "5" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "seats", "5.0" } }), Is.True);
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "seats", "05" } }), Is.True);
    }

    [Test]
    public void Evaluate_WithLargeIntegerClaim_ShouldNotLosePrecision()
    {
        // Beyond double's exact integer range; decimal comparison keeps these distinct.
        var condition = new RelationalCondition { Claim = "id", Operator = RelationalOperator.GreaterThan, Value = "9007199254740992" };

        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "id", "9007199254740993" } }), Is.True);
    }

    [Test]
    public void Evaluate_WithNumericClaimAndNonNumericTarget_ShouldFallBackToStringComparison()
    {
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThan, Value = "gold" };

        // "9" vs "gold": digits sort before letters ordinally.
        Assert.That(condition.Evaluate(new Dictionary<string, string> { { "tier", "9" } }), Is.False);
    }

    [Test]
    public void Evaluate_StringComparison_ShouldBeOrdinalNotCultureSensitive()
    {
        var original = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.GreaterThan, Value = "Gold" };
            var claims = new Dictionary<string, string> { { "tier", "gold" } };

            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var underEnglish = condition.Evaluate(claims);

            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            var underSwedish = condition.Evaluate(claims);

            Assert.That(underEnglish, Is.EqualTo(underSwedish));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = original;
        }
    }

    // ──────────────────────────────────────────────
    // Null value edge cases
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_Equals_WhenClaimValueIsNull_TreatsAsEmptyString()
    {
        // null claim value is normalised to "" by the ?. Trim() ?? "" guard
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = "" };
        var claims = new Dictionary<string, string> { { "role", null } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }

    [Test]
    public void Evaluate_Equals_WhenConditionValueIsNull_TreatsAsEmptyString()
    {
        var condition = new RelationalCondition { Claim = "role", Operator = RelationalOperator.Equals, Value = null };
        var claims = new Dictionary<string, string> { { "role", "" } };

        Assert.That(condition.Evaluate(claims), Is.True);
    }
}
