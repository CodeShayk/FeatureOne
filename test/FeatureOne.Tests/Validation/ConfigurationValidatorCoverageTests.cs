namespace FeatureOne.Tests.Validation;

[TestFixture]
public class ConfigurationValidatorCoverageTests
{
    private ConfigurationValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ConfigurationValidator();
    }

    [Test]
    public void ValidateCondition_WithPatternHavingDoubleQuantifiers_ShouldFail()
    {
        // Pattern with double quantifiers like (abc)+* triggers the second check
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"(abc)+*" // double quantifier: + followed by *
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithAlternationQuantifier_ShouldFail()
    {
        // Pattern with alternation like (a|b)+ triggers HasPotentiallyDangerousAlternation
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"(foo|bar)+"
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithDeeplyNestedGroups_ShouldFail()
    {
        // Pattern with more than 10 nesting levels triggers HasComplexNestedStructure
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"(((((((((((a)))))))))))" // 11 levels deep
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithTripleQuantifiers_ShouldFail()
    {
        // Pattern with three consecutive quantifiers triggers HasComplexNestedStructure
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"a+?*" // three consecutive quantifiers
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithNestedQuantifierGroup_ShouldFail()
    {
        // Pattern like (a*b)+ triggers HasSpecificDangerousPatterns nested quantifier check
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"(a*b)+"
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithConcatenatedSpecialChars_ShouldFail()
    {
        // Pattern with consecutive special regex chars (.*) triggers HasSpecificDangerousPatterns
        var condition = new RegexCondition
        {
            Claim = "test",
            Expression = @"a.*b" // .* is two consecutive special chars
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ValidateCondition_WithSimpleSafePattern_ShouldPass()
    {
        // A simple safe pattern should pass all checks
        var condition = new RegexCondition
        {
            Claim = "role",
            Expression = @"^admin$"
        };

        var result = _validator.ValidateCondition(condition);

        Assert.That(result.IsValid, Is.True);
    }
}
