using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FeatureOne
{
    /// <summary>
    /// Class to enable checking if a feature is enabled
    /// </summary>
    public class Features
    {
        private readonly IFeatureStore featureStore;
        private readonly IFeatureLogger logger;
        public static Features Current { get; private set; }

        public Features(IFeatureStore featureStore) : this(featureStore, new NullLogger())
        { }

        public Features(IFeatureStore featureStore, IFeatureLogger logger)
        {
            this.featureStore = featureStore;
            this.logger = logger;
        }

        public static void Initialize(Func<Features> factory) => Current = factory();

        /// <summary>
        /// Determines whether the feature is enabled for given claims principal.
        /// </summary>
        /// <param name="name">feature name</param>
        /// <param name="principal">Claims Principal cliams</param>
        /// <returns></returns>
        public bool IsEnabled(string name, ClaimsPrincipal principal)
        {
            return IsEnabled(name, principal.Claims);
        }

        /// <summary>
        /// Determines whether the feature is enabled for given set of user claims
        /// </summary>
        /// <param name="name">feature name</param>
        /// <param name="claims">user claims</param>
        /// <returns></returns>
        public bool IsEnabled(string name, IEnumerable<Claim> claims)
        {
            return IsEnabled(name, claims.ToDictionary(k => k.Type, v => v.Value));
        }

        /// <summary>
        /// Determines whether the feature is enabled for given set of user claims
        /// </summary>
        /// <param name="name">feature name</param>
        /// <param name="claims">user claims</param>
        /// <returns></returns>
        public bool IsEnabled(string name, IDictionary<string, string> claims)
        {
            try
            {
                if (claims == null)
                {
                    logger?.Warn($"FeatureOne, Action='Features.IsEnabled', Feature= {name}, Message='Empty claims'");
                }

                var featureName = new FeatureName(name);

                var features = featureStore.FindStartsWith(featureName.Value).ToList();
                if (!features.Any())
                {
                    logger?.Warn($"FeatureOne, Action='Features.IsEnabled', Feature= {name}, Message='No features in store'");
                    return false;
                }

                var feature = features.FirstOrDefault(x => x.Name.Value.Equals(featureName.Value, StringComparison.OrdinalIgnoreCase));

                if (feature == null)
                {
                    logger?.Warn($"FeatureOne, Action='Features.IsEnabled', Feature= {name}, Message='Featrue not found'");
                    return false;
                }

                var result = feature.IsEnabled(claims);
                logger?.Info($"FeatureOne, Action='Features.IsEnabled', Feature= {name}, result={result}");

                return result;
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOne, Action='Features.IsEnabled', Feature='{name}', Exception='{ex}'.");
                return false;
            }
        }
    }
}