namespace FeatureOne.Tests.Toggles.Conditions
{
    [TestFixture]
    public class RegexConditionTests
    {
        [Test]
        public void RegexCondition_WithMaliciousPattern_ShouldReturnFalse()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"^([a-zA-Z0-9]+)+$", // Known ReDoS pattern
                Timeout = TimeSpan.FromMilliseconds(100)
            };
            // Use a string that causes catastrophic backtracking: many valid chars followed by an invalid one
            var maliciousString = new string('a', 500) + "!"; // 500 'a's followed by '!' which doesn't match
            var claims = new Dictionary<string, string> { {"test", maliciousString} };
            
            // Act
            var result = condition.Evaluate(claims);
            
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RegexCondition_WithValidPattern_ShouldWorkCorrectly()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "email",
                Expression = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            };
            var claims = new Dictionary<string, string> { {"email", "test@example.com"} };
            
            // Act
            var result = condition.Evaluate(claims);
            
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RegexCondition_WithTimeout_ShouldNotHang()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"^([a-zA-Z0-9]+)+$", // Known ReDoS pattern
                Timeout = TimeSpan.FromMilliseconds(50) // Small timeout
            };
            // Use string that causes backtracking
            var maliciousString = new string('a', 250) + "!"; // 250 'a's followed by '!' which doesn't match
            var claims = new Dictionary<string, string> { {"test", maliciousString} };
            
            // Act & Assert
            // Should not hang and return false instead
            var startTime = DateTime.Now;
            var result = condition.Evaluate(claims);
            var endTime = DateTime.Now;
            
            // Should complete in less than 1 second (much less than potential backtracking time)
            Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(1000));
            Assert.That(result, Is.False);
        }

        [Test]
        public void RegexCondition_NormalPatternsNotAffected()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "name",
                Expression = @"^[A-Za-z]+$",
                Timeout = TimeSpan.FromMilliseconds(100)
            };
            var claims = new Dictionary<string, string> { {"name", "John"} };
            
            // Act
            var result = condition.Evaluate(claims);
            
            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RegexCondition_WithNullClaims_ShouldReturnFalse()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"^.*$"
            };
            
            // Act
            var result = condition.Evaluate(null);
            
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RegexCondition_WithNonMatchingClaim_ShouldReturnFalse()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"^.*$"
            };
            var claims = new Dictionary<string, string> { {"other", "value"} };
            
            // Act
            var result = condition.Evaluate(claims);
            
            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RegexCondition_WithInvalidExpression_ShouldReturnFalse()
        {
            // Arrange
            var condition = new RegexCondition
            {
                Claim = "test",
                Expression = @"[invalid" // Invalid regex expression
            };
            var claims = new Dictionary<string, string> { {"test", "value"} };
            
            // Act
            var result = condition.Evaluate(claims);
            
            // Assert
            Assert.That(result, Is.False);
        }
    }
}