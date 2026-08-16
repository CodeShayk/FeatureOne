using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;
using FeatureOne.OpenFeature;
using FeatureOne.OpenFeature.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature.Tests
{
    [TestFixture]
    public class FeatureOneProviderTests
    {
        private Mock<IStorageProvider> mockStorage;
        private FeatureStore featureStore;
        private FeatureOneProvider provider;

        [SetUp]
        public void Setup()
        {
            mockStorage = new Mock<IStorageProvider>();
            featureStore = new FeatureStore(mockStorage.Object);

            var enabledFeature = new Feature("enabled_flag", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));
            var disabledFeature = new Feature("disabled_flag", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = false }));
            var regexFeature = new Feature("user_flag", new Toggle(Operator.Any, new RegexCondition { Claim = "email", Expression = ".*@gbk\\.com" }));

            mockStorage.Setup(x => x.GetByName("enabled_flag")).Returns(new IFeature[] { enabledFeature });
            mockStorage.Setup(x => x.GetByName("disabled_flag")).Returns(new IFeature[] { disabledFeature });
            mockStorage.Setup(x => x.GetByName("user_flag")).Returns(new IFeature[] { regexFeature });
            mockStorage.Setup(x => x.GetByName("missing_flag")).Returns(Array.Empty<IFeature>());

            provider = new FeatureOneProvider(featureStore);
        }

        [Test]
        public void GetMetadata_ShouldReturnCorrectProviderName()
        {
            var metadata = provider.GetMetadata();
            Assert.That(metadata, Is.Not.Null);
            Assert.That(metadata.Name, Is.EqualTo("FeatureOne Provider"));
        }

        [Test]
        public async Task ResolveBooleanValueAsync_WhenFlagIsEnabled_ShouldReturnTrueWithTargetingMatch()
        {
            var result = await provider.ResolveBooleanValueAsync("enabled_flag", false);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.True);
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
            Assert.That(result.Variant, Is.EqualTo("on"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.None));
        }

        [Test]
        public async Task ResolveBooleanValueAsync_WhenFlagIsDisabled_ShouldReturnFalseWithDisabledReason()
        {
            var result = await provider.ResolveBooleanValueAsync("disabled_flag", true);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.False);
            Assert.That(result.Reason, Is.EqualTo(Reason.Disabled));
            Assert.That(result.Variant, Is.EqualTo("off"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.None));
        }

        [Test]
        public async Task ResolveBooleanValueAsync_WhenFlagNotFound_ShouldReturnDefaultValueWithFlagNotFoundErrorCode()
        {
            var result = await provider.ResolveBooleanValueAsync("missing_flag", false);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.False);
            Assert.That(result.Reason, Is.EqualTo(Reason.Error));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
        }

        [Test]
        public async Task ResolveBooleanValueAsync_WithContextMatchingCondition_ShouldReturnTrue()
        {
            var context = EvaluationContext.Builder()
                .Set("email", "john@gbk.com")
                .Build();

            var result = await provider.ResolveBooleanValueAsync("user_flag", false, context);

            Assert.That(result.Value, Is.True);
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
        }

        [Test]
        public async Task ResolveBooleanValueAsync_WithContextNotMatchingCondition_ShouldReturnFalse()
        {
            var context = EvaluationContext.Builder()
                .Set("email", "john@other.com")
                .Build();

            var result = await provider.ResolveBooleanValueAsync("user_flag", true, context);

            Assert.That(result.Value, Is.False);
            Assert.That(result.Reason, Is.EqualTo(Reason.Disabled));
        }

        [Test]
        public async Task ResolveStringValueAsync_WhenFlagIsEnabled_ShouldReturnTrueString()
        {
            var result = await provider.ResolveStringValueAsync("enabled_flag", "default");

            Assert.That(result.Value, Is.EqualTo("true"));
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
        }

        [Test]
        public async Task ResolveStringValueAsync_WhenFlagNotFound_ShouldReturnDefaultValue()
        {
            var result = await provider.ResolveStringValueAsync("missing_flag", "default");

            Assert.That(result.Value, Is.EqualTo("default"));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
        }

        [Test]
        public async Task ResolveIntegerValueAsync_WhenFlagIsEnabled_ShouldReturnOne()
        {
            var result = await provider.ResolveIntegerValueAsync("enabled_flag", 0);

            Assert.That(result.Value, Is.EqualTo(1));
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
        }

        [Test]
        public async Task ResolveIntegerValueAsync_WhenFlagNotFound_ShouldReturnDefaultValue()
        {
            var result = await provider.ResolveIntegerValueAsync("missing_flag", 42);

            Assert.That(result.Value, Is.EqualTo(42));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
        }

        [Test]
        public async Task ResolveDoubleValueAsync_WhenFlagIsEnabled_ShouldReturnOnePointZero()
        {
            var result = await provider.ResolveDoubleValueAsync("enabled_flag", 0.0);

            Assert.That(result.Value, Is.EqualTo(1.0));
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
        }

        [Test]
        public async Task ResolveDoubleValueAsync_WhenFlagNotFound_ShouldReturnDefaultValue()
        {
            var result = await provider.ResolveDoubleValueAsync("missing_flag", 3.14);

            Assert.That(result.Value, Is.EqualTo(3.14));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
        }

        [Test]
        public async Task ResolveStructureValueAsync_WhenFlagIsEnabled_ShouldReturnStructureWithValue()
        {
            var defaultStructure = new Value("default");
            var result = await provider.ResolveStructureValueAsync("enabled_flag", defaultStructure);

            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.AsBoolean, Is.True);
            Assert.That(result.Reason, Is.EqualTo(Reason.TargetingMatch));
        }

        [Test]
        public async Task ResolveStructureValueAsync_WhenFlagNotFound_ShouldReturnDefaultValue()
        {
            var defaultStructure = new Value("default");
            var result = await provider.ResolveStructureValueAsync("missing_flag", defaultStructure);

            Assert.That(result.Value, Is.EqualTo(defaultStructure));
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
        }

        [Test]
        public void ToClaims_WithNullContext_ShouldReturnEmptyDictionary()
        {
            EvaluationContext context = null;
            var claims = context.ToClaims();

            Assert.That(claims, Is.Not.Null);
            Assert.That(claims.Count, Is.EqualTo(0));
        }

        [Test]
        public void ToClaims_WithTargetingKeyAndAttributes_ShouldMapToClaimsCorrectly()
        {
            var context = EvaluationContext.Builder()
                .SetTargetingKey("usr_123")
                .Set("tier", "gold")
                .Set("isAdmin", true)
                .Set("score", 95)
                .Build();

            var claims = context.ToClaims();

            Assert.That(claims["targetingKey"], Is.EqualTo("usr_123"));
            Assert.That(claims["sub"], Is.EqualTo("usr_123"));
            Assert.That(claims["user_id"], Is.EqualTo("usr_123"));
            Assert.That(claims["tier"], Is.EqualTo("gold"));
            Assert.That(claims["isAdmin"], Is.EqualTo("true"));
            Assert.That(claims["score"], Is.EqualTo("95"));
        }

        [Test]
        public async Task OpenFeatureApiIntegration_EndToEndEvaluation_ShouldWorkWithFeatureOneProvider()
        {
            await global::OpenFeature.Api.Instance.SetProviderAsync(provider);
            var client = global::OpenFeature.Api.Instance.GetClient();

            bool isEnabled = await client.GetBooleanValueAsync("enabled_flag", false);
            Assert.That(isEnabled, Is.True);

            bool missingFlag = await client.GetBooleanValueAsync("missing_flag", false);
            Assert.That(missingFlag, Is.False);
        }

        [Test]
        public void ServiceCollectionExtensions_AddFeatureOneOpenFeature_RegistersProviderInDI()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(featureStore);
            services.AddFeatureOneOpenFeature(setAsGlobalProvider: false);

            var serviceProvider = services.BuildServiceProvider();
            var registeredProvider = serviceProvider.GetService<FeatureOneProvider>();

            Assert.That(registeredProvider, Is.Not.Null);
        }

        [Test]
        public async Task Lifecycle_InitializeAsyncAndShutdownAsync_UpdatesStatus()
        {
            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.NotReady));

            await provider.InitializeAsync(EvaluationContext.Empty);
            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.Ready));

            await provider.ShutdownAsync();
            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.NotReady));
        }

        [Test]
        public void ProviderHooks_AddHook_ShouldBeExposedInGetProviderHooks()
        {
            var mockHook = new Mock<Hook>();
            provider.AddHook(mockHook.Object);

            var hooks = provider.GetProviderHooks();
            Assert.That(hooks, Is.Not.Null);
            Assert.That(hooks.Count, Is.EqualTo(1));
            Assert.That(hooks[0], Is.EqualTo(mockHook.Object));
        }
    }
}
