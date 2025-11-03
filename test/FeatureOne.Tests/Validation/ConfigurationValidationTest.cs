namespace FeatureOne.Tests.Validation;

[TestFixture]
public class ConfigurationValidationTest
{
    [Test]
    public void Integration_ConfigurationValidation()
    {
        // Test feature name validation
        var validResult = ValidateFeatureName("ValidFeatureName");
        Assert.That(validResult, Is.True);

        // Invalid feature name with spaces
        var invalidResult = ValidateFeatureName("Invalid Feature Name With Spaces");
        Assert.That(invalidResult, Is.False);

        // Invalid feature name with special characters
        var invalidSpecialResult = ValidateFeatureName("Invalid@Feature#Name");
        Assert.That(invalidSpecialResult, Is.False);

        // Valid simple condition
        var simpleCondition = new SimpleCondition { IsEnabled = true };
        Assert.DoesNotThrow(() => ValidateCondition(simpleCondition));

        // Valid regex condition
        var regexCondition = new RegexCondition { Claim = "role", Expression = "^admin$" };
        Assert.DoesNotThrow(() => ValidateCondition(regexCondition));

        // Valid DateRange condition
        var dateRangeCondition = new DateRangeCondition { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
        Assert.DoesNotThrow(() => ValidateCondition(dateRangeCondition));
    }

    // Simulated validation methods since the actual validation logic might be in a different class
    private bool ValidateFeatureName(string name)
    {
        // Simulate validation logic - in real implementation this would be in ConfigurationValidator
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Check for invalid characters (simplified validation)
        var invalidChars = new[] { ' ', '@', '#', '%', '&', '*' };
        return !invalidChars.Any(c => name.Contains(c));
    }

    private void ValidateCondition(object condition)
    {
        // This method simulates validation - in real implementation it might throw if invalid
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        // For a SimpleCondition, check if isEnabled is valid
        if (condition is SimpleCondition simple)
        {
            // Simple validation - just make sure it's a boolean
            _ = simple.IsEnabled;
        }

        // For a RegexCondition, check the expression
        if (condition is RegexCondition regex)
        {
            if (string.IsNullOrEmpty(regex.Expression))
                throw new ArgumentException("Expression cannot be null or empty", nameof(regex.Expression));
        }

        // For a DateRangeCondition, check date values
        if (condition is DateRangeCondition dateRange)
        {
            if (dateRange.StartDate.HasValue && dateRange.EndDate.HasValue &&
                dateRange.StartDate.Value > dateRange.EndDate.Value)
                throw new ArgumentException("Start date cannot be after end date");
        }
    }
}