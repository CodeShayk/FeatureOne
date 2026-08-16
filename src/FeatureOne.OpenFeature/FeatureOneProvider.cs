using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// OpenFeature provider implementation for FeatureOne.
    /// </summary>
    public class FeatureOneProvider : FeatureProvider
    {
        private readonly IFeatureStore featureStore;
        private readonly IFeatureLogger logger;
        private readonly Metadata metadata = new Metadata("FeatureOne Provider");

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneProvider"/> using global <see cref="Features.Current"/>.
        /// </summary>
        public FeatureOneProvider() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneProvider"/> with explicit <see cref="IFeatureStore"/>.
        /// </summary>
        /// <param name="featureStore">The FeatureOne feature store instance.</param>
        /// <param name="logger">Optional logger instance.</param>
        public FeatureOneProvider(IFeatureStore featureStore, IFeatureLogger logger = null)
        {
            this.featureStore = featureStore;
            this.logger = logger;
        }

        /// <inheritdoc />
        public override Metadata GetMetadata() => metadata;

        /// <inheritdoc />
        public override Task<ResolutionDetails<bool>> ResolveBooleanValueAsync(string flagKey, bool defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var feature = GetFeature(flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='ResolveBooleanValueAsync', Flag='{flagKey}', Message='Flag not found'");
                    return Task.FromResult(new ResolutionDetails<bool>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store."));
                }

                var claims = context.ToClaims();
                bool isEnabled = feature.IsEnabled(claims);
                string variant = isEnabled ? "on" : "off";
                string reason = isEnabled ? Reason.TargetingMatch : Reason.Disabled;

                logger?.Info($"FeatureOneProvider, Action='ResolveBooleanValueAsync', Flag='{flagKey}', Result={isEnabled}");
                return Task.FromResult(new ResolutionDetails<bool>(flagKey, isEnabled, reason: reason, variant: variant));
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='ResolveBooleanValueAsync', Flag='{flagKey}'", ex);
                return Task.FromResult(new ResolutionDetails<bool>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<string>> ResolveStringValueAsync(string flagKey, string defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var feature = GetFeature(flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='ResolveStringValueAsync', Flag='{flagKey}', Message='Flag not found'");
                    return Task.FromResult(new ResolutionDetails<string>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store."));
                }

                var claims = context.ToClaims();
                bool isEnabled = feature.IsEnabled(claims);
                string variant = isEnabled ? "on" : "off";
                string resultValue = isEnabled ? "true" : "false";

                return Task.FromResult(new ResolutionDetails<string>(flagKey, resultValue, reason: isEnabled ? Reason.TargetingMatch : Reason.Disabled, variant: variant));
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='ResolveStringValueAsync', Flag='{flagKey}'", ex);
                return Task.FromResult(new ResolutionDetails<string>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<int>> ResolveIntegerValueAsync(string flagKey, int defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var feature = GetFeature(flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='ResolveIntegerValueAsync', Flag='{flagKey}', Message='Flag not found'");
                    return Task.FromResult(new ResolutionDetails<int>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store."));
                }

                var claims = context.ToClaims();
                bool isEnabled = feature.IsEnabled(claims);
                int resultValue = isEnabled ? 1 : 0;
                string variant = isEnabled ? "1" : "0";

                return Task.FromResult(new ResolutionDetails<int>(flagKey, resultValue, reason: isEnabled ? Reason.TargetingMatch : Reason.Disabled, variant: variant));
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='ResolveIntegerValueAsync', Flag='{flagKey}'", ex);
                return Task.FromResult(new ResolutionDetails<int>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<double>> ResolveDoubleValueAsync(string flagKey, double defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var feature = GetFeature(flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='ResolveDoubleValueAsync', Flag='{flagKey}', Message='Flag not found'");
                    return Task.FromResult(new ResolutionDetails<double>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store."));
                }

                var claims = context.ToClaims();
                bool isEnabled = feature.IsEnabled(claims);
                double resultValue = isEnabled ? 1.0 : 0.0;
                string variant = isEnabled ? "1.0" : "0.0";

                return Task.FromResult(new ResolutionDetails<double>(flagKey, resultValue, reason: isEnabled ? Reason.TargetingMatch : Reason.Disabled, variant: variant));
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='ResolveDoubleValueAsync', Flag='{flagKey}'", ex);
                return Task.FromResult(new ResolutionDetails<double>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message));
            }
        }

        /// <inheritdoc />
        public override Task<ResolutionDetails<Value>> ResolveStructureValueAsync(string flagKey, Value defaultValue, EvaluationContext context = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var feature = GetFeature(flagKey);
                if (feature == null)
                {
                    logger?.Warn($"FeatureOneProvider, Action='ResolveStructureValueAsync', Flag='{flagKey}', Message='Flag not found'");
                    return Task.FromResult(new ResolutionDetails<Value>(
                        flagKey,
                        defaultValue,
                        errorType: ErrorType.FlagNotFound,
                        reason: Reason.Error,
                        errorMessage: $"Flag '{flagKey}' was not found in FeatureOne store."));
                }

                var claims = context.ToClaims();
                bool isEnabled = feature.IsEnabled(claims);
                var structureValue = new Value(isEnabled);
                string variant = isEnabled ? "on" : "off";

                return Task.FromResult(new ResolutionDetails<Value>(flagKey, structureValue, reason: isEnabled ? Reason.TargetingMatch : Reason.Disabled, variant: variant));
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOneProvider, Action='ResolveStructureValueAsync', Flag='{flagKey}'", ex);
                return Task.FromResult(new ResolutionDetails<Value>(
                    flagKey,
                    defaultValue,
                    errorType: ErrorType.General,
                    reason: Reason.Error,
                    errorMessage: ex.Message));
            }
        }

        private IFeature GetFeature(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var store = featureStore ?? Features.Current?.FeatureStore;
            if (store != null)
            {
                var features = store.FindStartsWith(name);
                return features?.FirstOrDefault(x => x.Name.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }
    }
}
