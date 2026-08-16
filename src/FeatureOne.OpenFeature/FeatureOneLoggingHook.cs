using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FeatureOne.Core.Stores;
using OpenFeature;
using OpenFeature.Model;

namespace FeatureOne.OpenFeature
{
    /// <summary>
    /// OpenFeature hook implementation that adapts OpenFeature evaluation lifecycle events
    /// to FeatureOne's native <see cref="IFeatureLogger"/>.
    /// </summary>
    public class FeatureOneLoggingHook : Hook
    {
        private readonly IFeatureLogger logger;

        /// <summary>
        /// Initializes a new instance of <see cref="FeatureOneLoggingHook"/> using FeatureOne logger.
        /// </summary>
        /// <param name="logger">The FeatureOne logger instance.</param>
        public FeatureOneLoggingHook(IFeatureLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public override ValueTask<EvaluationContext> BeforeAsync<T>(HookContext<T> context, IReadOnlyDictionary<string, object> hints = null, CancellationToken cancellationToken = default)
        {
            logger.Info($"FeatureOneLoggingHook, Action='Before', Flag='{context.FlagKey}', Type='{typeof(T).Name}'");
            return new ValueTask<EvaluationContext>(context.EvaluationContext);
        }

        /// <inheritdoc />
        public override ValueTask AfterAsync<T>(HookContext<T> context, FlagEvaluationDetails<T> details, IReadOnlyDictionary<string, object> hints = null, CancellationToken cancellationToken = default)
        {
            logger.Info($"FeatureOneLoggingHook, Action='After', Flag='{context.FlagKey}', Value='{details.Value}', Reason='{details.Reason}'");
            return default;
        }

        /// <inheritdoc />
        public override ValueTask ErrorAsync<T>(HookContext<T> context, Exception error, IReadOnlyDictionary<string, object> hints = null, CancellationToken cancellationToken = default)
        {
            logger.Error($"FeatureOneLoggingHook, Action='Error', Flag='{context.FlagKey}'", error);
            return default;
        }

        /// <inheritdoc />
        public override ValueTask FinallyAsync<T>(HookContext<T> context, FlagEvaluationDetails<T> evaluationDetails, IReadOnlyDictionary<string, object> hints = null, CancellationToken cancellationToken = default)
        {
            logger.Info($"FeatureOneLoggingHook, Action='Finally', Flag='{context.FlagKey}'");
            return default;
        }
    }
}
