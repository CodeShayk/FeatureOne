using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeatureOne.Core;
using FeatureOne.Core.Stores;
using FeatureOne.Core.Toggles.Conditions;
using FeatureOne.OpenFeature.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature.Tests
{
    [TestFixture]
    public class FeatureOneProviderLifecycleTests
    {
        private FeatureStore CreateStore()
        {
            var mockStorage = new Mock<IStorageProvider>();
            var enabled = new Feature("enabled_flag", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));
            mockStorage.Setup(x => x.GetByName("enabled_flag")).Returns(new IFeature[] { enabled });
            return new FeatureStore(mockStorage.Object);
        }

        [Test]
        public async Task InitializeAsync_WithNoStoreAvailable_ShouldSetErrorStatus()
        {
            // No explicit store and no Features.Current means there is nothing to evaluate against.
            var provider = new FeatureOneProvider(featureStore: null);

            await provider.InitializeAsync(EvaluationContext.Empty);

            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.Error));
        }

        [Test]
        public async Task InitializeAsync_WithNoStore_ShouldLogTheFailure()
        {
            var logger = new Mock<IFeatureLogger>();
            var provider = new FeatureOneProvider(featureStore: null, logger: logger.Object);

            await provider.InitializeAsync(EvaluationContext.Empty);

            logger.Verify(l => l.Error(It.Is<string>(s => s.Contains("InitializeAsync")), It.IsAny<Exception>()), Times.Once);
        }

        [Test]
        public async Task InitializeAsync_WithStore_ShouldSetReadyStatus()
        {
            var provider = new FeatureOneProvider(CreateStore());

            await provider.InitializeAsync(EvaluationContext.Empty);

            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.Ready));
        }

        [Test]
        public async Task Resolve_WhenProviderInErrorState_ShouldReturnProviderNotReady()
        {
            var provider = new FeatureOneProvider(featureStore: null);
            await provider.InitializeAsync(EvaluationContext.Empty);
            Assert.That(provider.GetStatus(), Is.EqualTo(ProviderStatus.Error));

            var result = await provider.ResolveBooleanValueAsync("enabled_flag", false);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ProviderNotReady));
            Assert.That(result.Reason, Is.EqualTo(Reason.Error));
            Assert.That(result.Value, Is.False);
        }

        [Test]
        public async Task Resolve_WithNoStoreAvailable_ShouldReportProviderNotReadyNotFlagNotFound()
        {
            // A missing store is a configuration fault, not a missing flag - the two must not be conflated.
            var provider = new FeatureOneProvider(featureStore: null);

            var result = await provider.ResolveBooleanValueAsync("enabled_flag", false);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.ProviderNotReady));
        }

        [Test]
        public async Task Resolve_WithStoreButUnknownFlag_ShouldStillReportFlagNotFound()
        {
            var provider = new FeatureOneProvider(CreateStore());

            var result = await provider.ResolveBooleanValueAsync("nope", true);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.FlagNotFound));
            Assert.That(result.Value, Is.True);
        }

        [Test]
        public async Task Resolve_WhenStoreThrows_ShouldReturnGeneralError()
        {
            var store = new Mock<IFeatureStore>();
            store.Setup(x => x.FindStartsWith(It.IsAny<string>())).Throws(new InvalidOperationException("boom"));
            var provider = new FeatureOneProvider(store.Object);

            var result = await provider.ResolveBooleanValueAsync("enabled_flag", false);

            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.General));
            Assert.That(result.ErrorMessage, Is.EqualTo("boom"));
        }

        [Test]
        public void AddHook_ShouldBeSafeUnderConcurrentReads()
        {
            var provider = new FeatureOneProvider(CreateStore());
            var failures = 0;

            var reader = Task.Run(() =>
            {
                for (var i = 0; i < 2000; i++)
                {
                    try
                    {
                        foreach (var _ in provider.GetProviderHooks())
                        {
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        Interlocked.Increment(ref failures);
                    }
                }
            });

            var writer = Task.Run(() =>
            {
                for (var i = 0; i < 2000; i++)
                {
                    provider.AddHook(new Mock<Hook>().Object);
                }
            });

            Task.WaitAll(reader, writer);

            Assert.That(failures, Is.Zero);
            Assert.That(provider.GetProviderHooks().Count, Is.EqualTo(2000));
        }

        [Test]
        public void AddHook_WithNull_ShouldBeIgnored()
        {
            var provider = new FeatureOneProvider(CreateStore());

            provider.AddHook(null);

            Assert.That(provider.GetProviderHooks(), Is.Empty);
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithGlobalProvider_ShouldRegisterHostedServiceRatherThanSettingItInTheFactory()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(CreateStore());
            services.AddFeatureOneOpenFeature();

            // The registration must not depend on anything resolving FeatureOneProvider.
            var hostedServices = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();

            Assert.That(hostedServices, Has.Count.EqualTo(1));
            Assert.That(hostedServices[0].ImplementationType.Name, Is.EqualTo("FeatureOneProviderInitializer"));
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithoutGlobalProvider_ShouldNotRegisterHostedService()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(CreateStore());
            services.AddFeatureOneOpenFeature(setAsGlobalProvider: false);

            Assert.That(services.Any(d => d.ServiceType == typeof(IHostedService)), Is.False);
        }

        [Test]
        public async Task HostedService_StartAsync_ShouldSetTheGlobalProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(CreateStore());
            services.AddFeatureOneOpenFeature();

            var sp = services.BuildServiceProvider();
            var hostedService = sp.GetRequiredService<IHostedService>();

            await hostedService.StartAsync(CancellationToken.None);

            var client = global::OpenFeature.Api.Instance.GetClient();
            Assert.That(await client.GetBooleanValueAsync("enabled_flag", false), Is.True);
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithLoggingHookEnabled_ShouldAttachTheHook()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(CreateStore());
            services.AddSingleton<IFeatureLogger>(new Mock<IFeatureLogger>().Object);
            services.AddFeatureOneOpenFeature(options =>
            {
                options.SetAsGlobalProvider = false;
                options.EnableLoggingHook = true;
            });

            var provider = services.BuildServiceProvider().GetRequiredService<FeatureOneProvider>();

            Assert.That(provider.GetProviderHooks().Any(h => h is FeatureOneLoggingHook), Is.True);
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithConfiguredHooks_ShouldAttachThem()
        {
            var hook = new Mock<Hook>().Object;
            var services = new ServiceCollection();
            services.AddSingleton<IFeatureStore>(CreateStore());
            services.AddFeatureOneOpenFeature(options =>
            {
                options.SetAsGlobalProvider = false;
                options.AddHook(hook);
            });

            var provider = services.BuildServiceProvider().GetRequiredService<FeatureOneProvider>();

            Assert.That(provider.GetProviderHooks(), Does.Contain(hook));
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithExplicitStore_ShouldUseIt()
        {
            var services = new ServiceCollection();
            services.AddFeatureOneOpenFeature(CreateStore(), setAsGlobalProvider: false);

            var provider = services.BuildServiceProvider().GetRequiredService<FeatureOneProvider>();

            Assert.That(provider, Is.Not.Null);
        }

        [Test]
        public void AddFeatureOneOpenFeature_WithNullStore_ShouldThrow()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(
                () => services.AddFeatureOneOpenFeature((IFeatureStore)null, setAsGlobalProvider: false));
        }

        [Test]
        public void Options_AddHook_WithNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new FeatureOneOpenFeatureOptions().AddHook(null));
        }
    }
}
