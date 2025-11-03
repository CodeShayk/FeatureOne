namespace FeatureOne.Tests.Validation
{
    [TestFixture]
    public class ConfigurationValidatorTests
    {
        private ConfigurationValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ConfigurationValidator();
        }

        [Test]
        public void ConfigurationValidator_ValidFeatureName_ShouldPass()
        {
            // Act
            var result = _validator.ValidateFeatureName("ValidFeatureName123");

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_InvalidFeatureNameWithSpaces_ShouldFail()
        {
            // Act
            var result = _validator.ValidateFeatureName("Invalid Feature Name");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_InvalidFeatureNameWithSpecialChars_ShouldFail()
        {
            // Act
            var result = _validator.ValidateFeatureName("Invalid@Name!");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_EmptyFeatureName_ShouldFail()
        {
            // Act
            var result = _validator.ValidateFeatureName("");

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_NullFeatureName_ShouldFail()
        {
            // Act
            var result = _validator.ValidateFeatureName(null);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_ValidSimpleCondition_ShouldPass()
        {
            // Arrange
            var condition = new SimpleCondition { IsEnabled = true };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_ValidRegexCondition_ShouldPass()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "role",
                Expression = "admin"
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_RegexConditionWithNullClaim_ShouldFail()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = null,
                Expression = "admin"
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_RegexConditionWithNullExpression_ShouldFail()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "role",
                Expression = null
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_DangerousRegexPattern_ShouldBeDetected()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"^([a-zA-Z0-9]+)+$" // Known dangerous ReDoS pattern from test case
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_DateRangeCondition_ShouldPass()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_InvalidDateRangeCondition_ShouldFail()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(10), // Future start
                EndDate = DateTime.Now.AddDays(5)    // Past end (invalid range)
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void ConfigurationValidator_ValidDateRangeCondition_ShouldPass()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-5), // Past start
                EndDate = DateTime.Now.AddDays(5)    // Future end (valid range)
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_DateRangeWithNullDates_ShouldPass()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = null, // No start limit
                EndDate = null    // No end limit
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_DateRangeWithOnlyStartDate_ShouldPass()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-5), // Valid start date
                EndDate = null                        // No end limit
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public void ConfigurationValidator_DateRangeWithOnlyEndDate_ShouldPass()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = null,                      // No start limit
                EndDate = DateTime.Now.AddDays(5)     // Valid end date
            };

            // Act
            var result = _validator.ValidateCondition(condition);

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }
    }
}