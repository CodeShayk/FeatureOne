namespace FeatureOne.Tests.Validation;

[TestFixture]
public class FeatureNameValidationTest
{
    [Test]
    public void FeatureName_ComprehensiveValidation()
    {
        // Test valid names
        Assert.DoesNotThrow(() => new FeatureName("ValidName123"));
        Assert.DoesNotThrow(() => new FeatureName("Valid_Name"));
        Assert.DoesNotThrow(() => new FeatureName("Valid-Name"));
        Assert.DoesNotThrow(() => new FeatureName("A")); // Single character
        Assert.DoesNotThrow(() => new FeatureName("ValidNameWith123Numbers"));
        
        // Test invalid names
        Assert.Throws<ArgumentException>(() => new FeatureName(null));
        Assert.Throws<ArgumentException>(() => new FeatureName(""));
        Assert.Throws<ArgumentException>(() => new FeatureName("   ")); // Whitespace only
        Assert.Throws<ArgumentException>(() => new FeatureName("Invalid Name")); // Space
        Assert.Throws<ArgumentException>(() => new FeatureName("Invalid@Name")); // Special char
    }
}