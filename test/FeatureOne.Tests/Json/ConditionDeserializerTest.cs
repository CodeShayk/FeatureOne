using System.Text.Json.Nodes;

namespace FeatureOne.Tests.Json;

[TestFixture]
public class ConditionDeserializerTest
{
    [Test]
    public void ConditionDeserializer_EdgeCases()
    {
        // Test with minimal valid JSON
        var deserializer = new ConditionDeserializer();
        
        // Valid simple condition
        var simpleJson = new JsonObject();
        simpleJson["type"] = "Simple";
        simpleJson["isEnabled"] = true;
        var simpleCondition = deserializer.Deserialize(simpleJson);
        Assert.That(simpleCondition, Is.InstanceOf<SimpleCondition>());
        
        // Valid regex condition
        var regexJson = new JsonObject();
        regexJson["type"] = "Regex";
        regexJson["claim"] = "role";
        regexJson["expression"] = "admin";
        var regexCondition = deserializer.Deserialize(regexJson);
        Assert.That(regexCondition, Is.InstanceOf<RegexCondition>());
        
        // Valid DateRange condition
        var dateRangeJson = new JsonObject();
        dateRangeJson["type"] = "DateRange";
        dateRangeJson["startDate"] = "2025-01-01";
        dateRangeJson["endDate"] = "2025-12-31";
        var dateRangeCondition = deserializer.Deserialize(dateRangeJson);
        Assert.That(dateRangeCondition, Is.InstanceOf<DateRangeCondition>());
        
        // Invalid type
        var invalidJson = new JsonObject();
        invalidJson["type"] = "NonExistent";
        Assert.Throws<Exception>(() => deserializer.Deserialize(invalidJson));
        
        // Null condition
        Assert.Throws<ArgumentNullException>(() => deserializer.Deserialize(null));
    }
    
    [Test]
    public void ConditionDeserializer_SecureTypeLoading()
    {
        // Arrange - Test that only safe types are loaded
        var deserializer = new ConditionDeserializer();
        
        // Valid type should work
        var validJson = new JsonObject();
        validJson["type"] = "Simple";
        validJson["isEnabled"] = true;
        var validCondition = deserializer.Deserialize(validJson);
        Assert.That(validCondition, Is.InstanceOf<SimpleCondition>());
        
        // Another valid type
        var validJson2 = new JsonObject();
        validJson2["type"] = "Regex";
        validJson2["claim"] = "role";
        validJson2["expression"] = "^admin$";
        var validCondition2 = deserializer.Deserialize(validJson2);
        Assert.That(validCondition2, Is.InstanceOf<RegexCondition>());
        
        // Try to load a potentially dangerous type - should fail
        var dangerousJson = new JsonObject();
        dangerousJson["type"] = "System.IO.FileInfo"; // This should not be allowed
        Assert.Throws<Exception>(() => deserializer.Deserialize(dangerousJson));
    }
}