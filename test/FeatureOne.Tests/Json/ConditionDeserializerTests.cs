using System;
using System.Text.Json.Nodes;
using FeatureOne.Json;
using FeatureOne.Core.Toggles.Conditions;
using NUnit.Framework;

namespace FeatureOne.Tests.Json
{
    [TestFixture]
    public class ConditionDeserializerTests
    {
        private ConditionDeserializer _deserializer;

        [SetUp]
        public void Setup()
        {
            _deserializer = new ConditionDeserializer();
        }

        [Test]
        public void ConditionDeserializer_WithValidConditionType_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "Simple",
                ["isEnabled"] = "true"
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<SimpleCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithValidConditionTypeWithSuffix_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "SimpleCondition",
                ["isEnabled"] = "true"
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<SimpleCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithValidRegexCondition_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "Regex",
                ["claim"] = "role",
                ["expression"] = "admin"
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<RegexCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithValidRegexConditionWithSuffix_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "RegexCondition",
                ["claim"] = "role",
                ["expression"] = "admin"
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<RegexCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithValidDateRangeCondition_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "DateRange",
                ["startDate"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ["endDate"] = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<DateRangeCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithValidDateRangeConditionWithSuffix_ShouldLoadSuccessfully()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "DateRangeCondition",
                ["startDate"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ["endDate"] = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
            };
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<DateRangeCondition>());
        }

        [Test]
        public void ConditionDeserializer_WithInvalidTypeName_ShouldThrowException()
        {
            // Arrange
            var json = new JsonObject
            {
                ["type"] = "NonExistentCondition"
            };
            
            // Act & Assert
            Assert.Throws<Exception>(() => _deserializer.Deserialize(json));
        }

        [Test]
        public void ConditionDeserializer_WithKnownConditions_ShouldLoadAll()
        {
            // Arrange
            var simpleJson = new JsonObject { ["type"] = "Simple", ["isEnabled"] = "true" };
            var regexJson = new JsonObject { ["type"] = "Regex", ["claim"] = "role", ["expression"] = "admin" };
            var dateRangeJson = new JsonObject { 
                ["type"] = "DateRange", 
                ["startDate"] = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
                ["endDate"] = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
            };
            
            // Act
            var simpleCondition = _deserializer.Deserialize(simpleJson);
            var regexCondition = _deserializer.Deserialize(regexJson);
            var dateRangeCondition = _deserializer.Deserialize(dateRangeJson);
            
            // Assert
            Assert.That(simpleCondition, Is.InstanceOf<SimpleCondition>());
            Assert.That(regexCondition, Is.InstanceOf<RegexCondition>());
            Assert.That(dateRangeCondition, Is.InstanceOf<DateRangeCondition>());
        }

        [Test]
        public void ConditionDeserializer_CaseInsensitiveTypeMatching_ShouldWork()
        {
            // Arrange
            var json = new JsonObject { ["type"] = "simple", ["isEnabled"] = "true" }; // lowercase
            
            // Act
            var condition = _deserializer.Deserialize(json);
            
            // Assert
            Assert.That(condition, Is.InstanceOf<SimpleCondition>());
        }

        [Test]
        public void ConditionDeserializer_UnknownSimilarType_ShouldThrow()
        {
            // Arrange
            var json = new JsonObject { ["type"] = "SimpleAttacker" }; // Similar to "Simple" but not valid
            
            // Act & Assert
            Assert.Throws<Exception>(() => _deserializer.Deserialize(json));
        }

        [Test]
        public void ConditionDeserializer_WithNullCondition_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _deserializer.Deserialize(null));
        }
    }
}