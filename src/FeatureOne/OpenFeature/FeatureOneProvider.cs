using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FeatureOne.Core.Stores;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature
{
    /// <summary>
    /// OpenFeature specification compliant provider implementation for FeatureOne.
    /// </summary>
    /// <remarks>
    /// FeatureOne is a boolean toggle engine: a feature is either on or off for a given set of claims.
    /// The provider therefore projects that single boolean onto every OpenFeature flag type:
    /// <list type="bullet">
    /// <item><description><c>Boolean</c> resolves to the toggle result directly.</description></item>
    /// <item><description><c>String</c> resolves to <c>"true"</c> / <c>"false"</c>.</description></item>
    /// <item><description><c>Integer</c> resolves to <c>1</c> / <c>0</c>.</description></item>
    /// <item><description><c>Double</c> resolves to <c>1.0</c> / <c>0.0</c>.</description></item>
    /// <item><description><c>Structure</c> resolves to a <see cref="Value"/> wrapping the boolean.</description></item>
    /// </list>
    /// The non-boolean forms carry no information the boolean form does not. Treat them as a convenience
    /// for callers whose call sites are already typed, not as multivariate flag support — FeatureOne has
    /// no notion of a string or numeric flag value, so a call such as
    /// <c>GetStringValue("theme", "dark")</c> returns <c>"true"</c> or <c>"false"</c>, never a theme name.
    /// Prefer <c>GetBooleanValue</c> unless you specifically need one of the projections above.
    /// </remarks>
    public class FeatureOneProvider : FeatureProvider
    {
        private readonly IFeatureStore featureStore;
        private readonly IFeatureLogger logger;
        private readonly Metadata metadata = new Metadata("FeatureOne Provider");
        private ImmutableList<Hook> hooks = ImmutableList<Hook>.Empty;
        private ProviderStatus status = ProviderStatus.NotReady;

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneProvider"/> using global <see cref="Features.Current"/>.
        /// </summary>
        public FeatureOneProvider() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneProvider"/> with explicit <see cref="IFeatureStore"/>.
        /// </summary>
        /// <param name="featureStore">The FeatureOne feature store instance. When null, the store is taken from <see cref="Features.Current"/> at evaluation time.</param>
        /// <param name="logger">Optional logger instance.</param>
        public FeatureOneProvider(IFeatureStore featureStore, IFeatureLogger logger = null)
        {
            this.featureStore = featureStore;
            this.logger = logger;
        }

        /// <inheritdoc />
        public override Metadata GetMetadata() => metadata;

        /// <summary>
        /// Returns the current provider status.
        /// </summary>
        /// <returns>The provider status.</returns>
        public ProviderStatus GetStatus() => status;

        /// <inheritdoc />
        public override IImmutableList<Hook> GetProviderHooks() => Volatile.Read(ref hooks);

        /// <summary>
        /// Adds a provider-level hook to the FeatureOne provider.
        /// </summary>
        /// <param name="hook">The hook instance. Null is ignored.</param>
        public void AddHook(Hook hook)
        {
            if (hook == null)
            {
                return;
            }

            // Hooks are usually added at configuration time, but the provider is registered as a
            // singleton, so swap atomically rather than mutating a list another thread may be reading.
            ImmutableList<Hook> current, updated;
            do
            {
                current = Volatile.Read(ref hooks);
                updated = current.Add(hook);
            }
            while (Interlocked.CompareExchange(ref hooks, updated, current) != current);
        }

        /// <inheritdoc />
        public override Task InitializeAsync(EvaluationContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var store = ResolveStore();
                if (store == null)
                {
                    status = ProviderStatus.Error;
                    logger?.Error("FeatureOneProvider, Action='InitializeAsync', Message='No IFeatureStore available. Pass one to the constructor or call Features.Initialize before setting the provider.'");
                    return Task.CompletedTask;
                }

                status = ProviderStatus.Ready;
                logger?.Info("FeatureOneProvider, Action='InitializeAsync', Message='Provider initialized successfully'");
            }
            catch (Exception ex)
            {
                status = ProviderStatus.Error;
                logger?.Error("FeatureOneProvider, Action='InitializeAsync', Message='Initialization failed'", ex);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            status = ProviderStatus.NotReady;
            logger?.Info("FeatureOneProvider, Action='ShutdownAsync', Message='Provider shut down'");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>Resolves to the FeatureOne toggle result for <paramref name="flagKey"/>.</remarks>
        public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(string flagKey, bool defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Resolve(flagKey, defaultValue, context, nameof(ResolveBooleanValueAsync),
                isEnabled => (isEnabled, isEnabled ? "on" : "off")));

        /// <inheritdoc />
        /// <remarks>
        /// Resolves to <c>"true"</c> or <c>"false"</c>. FeatureOne has no string flag values;
        /// see the remarks on <see cref="FeatureOneProvider"/>.
        /// </remarks>
        public override Task<ResolutionDetails<string>> ResolveStringValueAsync(string flagKey, string defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Resolve(flagKey, defaultValue, context, nameof(ResolveStringValueAsync),
                isEnabled => (isEnabled ? "true" : "false", isEnabled ? "on" : "off")));

        /// <inheritdoc />
        /// <remarks>
        /// Resolves to <c>1</c> or <c>0</c>. FeatureOne has no numeric flag values;
        /// see the remarks on <see cref="FeatureOneProvider"/>.
        /// </remarks>
        public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(string flagKey, int defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Resolve(flagKey, defaultValue, context, nameof(ResolveIntegerValueAsync),
                isEnabled => (isEnabled ? 1 : 0, isEnabled ? "1" : "0")));

        /// <inheritdoc />
        /// <remarks>
        /// Resolves to <c>1.0</c> or <c>0.0</c>. FeatureOne has no numeric flag values;
        /// see the remarks on <see cref="FeatureOneProvider"/>.
        /// </remarks>
        public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(string flagKey, double defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Resolve(flagKey, defaultValue, context, nameof(ResolveDoubleValueAsync),
                isEnabled => (isEnabled ? 1.0 : 0.0, isEnabled ? "1.0" : "0.0")));

        /// <inheritdoc />
        /// <remarks>
        /// Resolves to a <see cref="Value"/> wrapping the boolean toggle result. FeatureOne has no
        /// structured flag values; see the remarks on <see cref="FeatureOneProvider"/>.
        /// </remarks>
        public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(string flagKey, Value defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
            => Task.FromResult(Resolve(flagKey, defaultValue, context, nameof(ResolveStructureValueAsync),
                isEnabled => (new Value(isEnabled), isEnabled ? "on" : "off")));

        /// <summary>
        /// Shared evaluation pipeline for every flag type. Applies the provider status guard, resolves the
        /// feature, evaluates it against the mapped claims, and projects the boolean result onto
        /// <typeparamref name="T"/> via <paramref name="project"/>.
        /// </summary>
        private ResolutionDetails<T> Resolve<T>(
            string flagKey,
            T defaultValue,
            EvaluationContext context,
            string action,
            Func<bool, (T Value, string Variant)> project)
        {
            try
            {
                if (status == ProviderStatus.Error)
                {
                    return new ResolutionDetails<T>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.ProviderNotReady,
                        reason: Reason.Error,
                        errorMessage: "FeatureOneProvider is in an error state.");
                }

                var store = ResolveStore();
                if (store == null)
                {
                    // Distinct from FlagNotFound: the flag may well exist, we have nowhere to look it up.
                    logger?.Warn($"FeatureOneProvider, Action='{action}', Flag='{flagKey}', Message='No feature store available'");
                    return new ResolutionDetails<T>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.ProviderNotReady,
                        reason: Reason.Error,
                        errorMessage: "FeatureOneProvider has no IFeatureStore available.");
                }

                var feature = GetFeature(store, flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='{action}', Flag='{flagKey}', Message='Flag not found'");
                    return new ResolutionDetails<T>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store.");
                }

                var claims = context.ToClaims();
                var isEnabled = feature.IsEnabled(claims);
                var (value, variant) = project(isEnabled);

                logger?.Info($"FeatureOneProvider, Action='{action}', Flag='{flagKey}', Result={isEnabled}");
                return new ResolutionDetails<T>(
                    flagKey,
                    value,
                    reason: isEnabled ? Reason.TargetingMatch : Reason.Disabled,
                    variant: variant);
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='{action}', Flag='{flagKey}'", ex);
                return new ResolutionDetails<T>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message);
            }
        }

        private IFeatureStore ResolveStore() => featureStore ?? Features.Current?.FeatureStore;

        private static IFeature GetFeature(IFeatureStore store, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var features = store.FindStartsWith(name);
            return features?.FirstOrDefault(x => x.Name.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
