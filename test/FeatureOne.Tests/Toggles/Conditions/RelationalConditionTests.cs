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
    // LessThan — defined in enum but not in switch;
    // falls through to default and returns false.
    // ──────────────────────────────────────────────

    [Test]
    public void Evaluate_LessThan_ReturnsDefaultFalse()
    {
        // LessThan is not handled in the switch statement; default branch returns false.
        var condition = new RelationalCondition { Claim = "tier", Operator = RelationalOperator.LessThan, Value = "gold" };
        var claims = new Dictionary<string, string> { { "tier", "bronze" } };

        Assert.That(condition.Evaluate(claims), Is.False);
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
