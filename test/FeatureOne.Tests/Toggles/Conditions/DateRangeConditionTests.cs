namespace FeatureOne.Tests.Toggles.Conditions
{
    [TestFixture]
    public class DateRangeConditionTests
    {
        [Test]
        public void DateRangeCondition_WithinRange_ShouldReturnTrue()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_OutsideRange_ShouldReturnFalse()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-5)
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void DateRangeCondition_WithStartDateOnly_ShouldWork()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = null // No end limit
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_WithEndDateOnly_ShouldWork()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = null, // No start limit
                EndDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_WithBothDatesNull_ShouldReturnTrue()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = null,
                EndDate = null
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_ExactStartDate_ShouldReturnTrue()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var condition = new DateRangeCondition
            {
                StartDate = today,
                EndDate = today.AddDays(2)
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_ExactEndDate_ShouldReturnTrue()
        {
            // Arrange
            var today = DateTime.Now.Date;
            var condition = new DateRangeCondition
            {
                StartDate = today.AddDays(-2),
                EndDate = today
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void DateRangeCondition_FutureStartDatePastDate_ShouldReturnFalse()
        {
            // Arrange
            var condition = new DateRangeCondition
            {
                StartDate = DateTime.Now.AddDays(5), // Future start
                EndDate = DateTime.Now.AddDays(10)   // Future end
            };

            // Act
            var result = condition.Evaluate(new Dictionary<string, string>());

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void DateRangeCondition_SerializationProperties_AreCorrect()
        {
            // Arrange
            var expectedStart = DateTime.Now.AddDays(-5);
            var expectedEnd = DateTime.Now.AddDays(5);

            // Act
            var condition = new DateRangeCondition
            {
                StartDate = expectedStart,
                EndDate = expectedEnd
            };

            // Assert
            Assert.That(condition.StartDate.Value.Date, Is.EqualTo(expectedStart.Date));
            Assert.That(condition.EndDate.Value.Date, Is.EqualTo(expectedEnd.Date));
        }
    }
}