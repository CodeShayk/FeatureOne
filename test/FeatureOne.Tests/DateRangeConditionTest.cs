namespace FeatureOne.Tests;

[TestFixture]
public class DateRangeConditionTest
{
    [Test]
    public void DateRangeCondition_WithinRange_ShouldReturnTrue()
    {
        // Arrange - Create a date range that includes today
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
    public void DateRangeCondition_BeforeStartDate_ShouldReturnFalse()
    {
        // Arrange - Create a date range in the future
        var condition = new DateRangeCondition
        {
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(2)
        };
        
        // Act
        var result = condition.Evaluate(new Dictionary<string, string>());
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void DateRangeCondition_AfterEndDate_ShouldReturnFalse()
    {
        // Arrange - Create a date range in the past
        var condition = new DateRangeCondition
        {
            StartDate = DateTime.Now.AddDays(-2),
            EndDate = DateTime.Now.AddDays(-1)
        };
        
        // Act
        var result = condition.Evaluate(new Dictionary<string, string>());
        
        // Assert
        Assert.That(result, Is.False);
    }
    
    [Test]
    public void DateRangeCondition_NullStartDate_OnlyEndDate()
    {
        // Arrange - No start date, only end date
        var condition = new DateRangeCondition
        {
            StartDate = null,
            EndDate = DateTime.Now.AddDays(1)
        };
        
        // Act
        var result = condition.Evaluate(new Dictionary<string, string>());
        
        // Assert - Should be within range since there's no start date
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void DateRangeCondition_NullEndDate_OnlyStartDate()
    {
        // Arrange - No end date, only start date
        var condition = new DateRangeCondition
        {
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = null
        };
        
        // Act
        var result = condition.Evaluate(new Dictionary<string, string>());
        
        // Assert - Should be within range since there's no end date
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void DateRangeCondition_BothDatesNull_ShouldReturnTrue()
    {
        // Arrange - Both dates null means always enabled
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
}