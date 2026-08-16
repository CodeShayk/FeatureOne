using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace FeatureOne.OpenFeature
{
    /// <summary>
    /// Hosted service that registers <see cref="FeatureOneProvider"/> as the OpenFeature global provider
    /// on application start.
    /// </summary>
    /// <remarks>
    /// Registration is a startup concern, not a resolution concern. Doing it here rather than inside the
    /// DI factory means the global provider is set even when nothing in the application ever resolves
    /// <see cref="FeatureOneProvider"/>, and it lets <c>SetProviderAsync</c> — which awaits provider
    /// initialization — be awaited properly instead of blocked on.
    /// </remarks>
    internal sealed class FeatureOneProviderInitializer : IHostedService
    {
        private readonly FeatureOneProvider provider;

        public FeatureOneProviderInitializer(FeatureOneProvider provider) => this.provider = provider;

        public Task StartAsync(CancellationToken cancellationToken)
            => global::OpenFeature.Api.Instance.SetProviderAsync(provider);

        public Task StopAsync(CancellationToken cancellationToken)
            => global::OpenFeature.Api.Instance.ShutdownAsync();
    }
}
