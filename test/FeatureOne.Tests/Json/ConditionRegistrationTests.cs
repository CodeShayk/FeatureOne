using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using FeatureOne.Core;

namespace FeatureOne.Tests.Json
{
    /// <summary>
    /// Custom condition used to prove that user supplied condition types can be registered and
    /// deserialized without reimplementing <see cref="IConditionDeserializer"/>.
    /// </summary>
    public class PercentageCondition : ICondition
    {
        public int Threshold { get; set; }
        public string Claim { get; set; }

        public bool Evaluate(IDictionary<string, string> claims)
        {
            if (claims == null || Claim == null || !claims.TryGetValue(Claim, out var raw))
                return false;

            return int.TryParse(raw, out var value) && value <= Threshold;
        }
    }

    public class NoParameterlessConstructorCondition : ICondition
    {
        public NoParameterlessConstructorCondition(string required) => Required = required;

        public string Required { get; set; }

        public bool Evaluate(IDictionary<string, string> claims) => false;
    }

    public class NotACondition
    {
    }

    [TestFixture]
    public class ConditionRegistrationTests
    {
        [Test]
        public void Register_CustomCondition_ShouldDeserializeAndHydrateProperties()
        {
            var deserializer = new ConditionDeserializer().Register<PercentageCondition>("Percentage");

            var json = new JsonObject
            {
                ["type"] = "Percentage",
                ["threshold"] = 50,
                ["claim"] = "bucket"
            };

            var condition = deserializer.Deserialize(json);

            Assert.That(condition, Is.InstanceOf<PercentageCondition>());
            var percentage = (PercentageCondition)condition;
            Assert.That(percentage.Threshold, Is.EqualTo(50));
            Assert.That(percentage.Claim, Is.EqualTo("bucket"));
            Assert.That(percentage.Evaluate(new Dictionary<string, string> { ["bucket"] = "20" }), Is.True);
            Assert.That(percentage.Evaluate(new Dictionary<string, string> { ["bucket"] = "80" }), Is.False);
        }

        [Test]
        public void Register_ShouldNormalisePostfixedAndBareNames()
        {
            var deserializer = new ConditionDeserializer().Register<PercentageCondition>("PercentageCondition");

            var bare = deserializer.Deserialize(new JsonObject { ["type"] = "Percentage" });
            var postfixed = deserializer.Deserialize(new JsonObject { ["type"] = "PercentageCondition" });

            Assert.That(bare, Is.InstanceOf<PercentageCondition>());
            Assert.That(postfixed, Is.InstanceOf<PercentageCondition>());
        }

        [Test]
        public void Register_ShouldReturnSameInstanceForChaining()
        {
            var deserializer = new ConditionDeserializer();

            var returned = deserializer.Register<PercentageCondition>("Percentage");

            Assert.That(returned, Is.SameAs(deserializer));
        }

        [Test]
        public void Constructor_WithCustomTypes_ShouldRegisterThem()
        {
            var deserializer = new ConditionDeserializer(new[]
            {
                new KeyValuePair<string, Type>("Percentage", typeof(PercentageCondition))
            });

            Assert.That(deserializer.Deserialize(new JsonObject { ["type"] = "Percentage" }),
                Is.InstanceOf<PercentageCondition>());
        }

        [Test]
        public void Constructor_WithCustomTypes_ShouldStillSupportBuiltInTypes()
        {
            var deserializer = new ConditionDeserializer(new[]
            {
                new KeyValuePair<string, Type>("Percentage", typeof(PercentageCondition))
            });

            Assert.That(deserializer.Deserialize(new JsonObject { ["type"] = "Simple", ["isEnabled"] = true }),
                Is.InstanceOf<FeatureOne.Core.Toggles.Conditions.SimpleCondition>());
        }

        [Test]
        public void Register_TypeNotImplementingICondition_ShouldThrow()
        {
            var deserializer = new ConditionDeserializer();

            var ex = Assert.Throws<FeatureOneConfigurationException>(
                () => deserializer.Register("Bad", typeof(NotACondition)));

            Assert.That(ex.Message, Does.Contain(nameof(ICondition)));
        }

        [Test]
        public void Register_AbstractType_ShouldThrow()
        {
            var deserializer = new ConditionDeserializer();

            Assert.Throws<FeatureOneConfigurationException>(
                () => deserializer.Register("Bad", typeof(ICondition)));
        }

        [Test]
        public void Register_TypeWithoutParameterlessConstructor_ShouldThrow()
        {
            var deserializer = new ConditionDeserializer();

            var ex = Assert.Throws<FeatureOneConfigurationException>(
                () => deserializer.Register("Bad", typeof(NoParameterlessConstructorCondition)));

            Assert.That(ex.Message, Does.Contain("parameterless constructor"));
        }

        [Test]
        public void Register_WithNullName_ShouldThrow()
        {
            var deserializer = new ConditionDeserializer();

            Assert.Throws<ArgumentNullException>(() => deserializer.Register(null, typeof(PercentageCondition)));
        }

        [Test]
        public void Register_WithNullType_ShouldThrow()
        {
            var deserializer = new ConditionDeserializer();

            Assert.Throws<ArgumentNullException>(() => deserializer.Register("Percentage", null));
        }

        [Test]
        public void Register_ShouldNotLeakIntoOtherInstances()
        {
            new ConditionDeserializer().Register<PercentageCondition>("Percentage");

            var untouched = new ConditionDeserializer();

            Assert.Throws<FeatureOneConfigurationException>(
                () => untouched.Deserialize(new JsonObject { ["type"] = "Percentage" }));
        }

        [Test]
        public void Deserialize_UnregisteredType_ShouldListRegisteredTypesInMessage()
        {
            var deserializer = new ConditionDeserializer();

            var ex = Assert.Throws<FeatureOneConfigurationException>(
                () => deserializer.Deserialize(new JsonObject { ["type"] = "Percentage" }));

            Assert.That(ex.Message, Does.Contain("SimpleCondition"));
            Assert.That(ex.Message, Does.Contain("Register"));
        }

        [Test]
        public void Deserialize_MissingTypeDiscriminator_ShouldThrowConfigurationException()
        {
            var deserializer = new ConditionDeserializer();

            var ex = Assert.Throws<FeatureOneConfigurationException>(
                () => deserializer.Deserialize(new JsonObject { ["isEnabled"] = true }));

            Assert.That(ex.Message, Does.Contain("type"));
        }
    }
}
