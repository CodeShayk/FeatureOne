﻿using System.Security.Claims;
using FeatureOn.Configuration;
using FeatureOn.Logger;
using FeatureOn.Stores;

namespace Ninja.FeatureIt
{
    public class Features
    {

        private readonly IFeatureStore featureStore;
        private readonly FeatureConfiguration Configuration;
        public static Features Current { get; private set; }

        public Features(IFeatureStore featureStore) : this(featureStore, new FeatureConfiguration
        {
            UseCache = true,
            Logger = new NullLogger()
        }){ }

        public Features(IFeatureStore featureStore, FeatureConfiguration configuration)
        {
            this.featureStore = featureStore;
            this.Configuration = configuration;           
        }
                

        public static void Initialise(Func<Features> factory) => Current = factory();


        /// <summary>
        /// Determines whether the feature is enabled for given claims principal.
        /// </summary>
        /// <param name="name"> feature name</param>
        /// <param name="principal">Claims Principal cliams</param>
        /// <returns></returns>
        public bool IsEnabled(string name, ClaimsPrincipal principal)
        {
            return IsEnabled(name, principal.Claims.ToDictionary(k => k.Type, v => v.Value));
        }
        /// <summary>
        /// Determines whether the feature is enabled for given set of user claims
        /// </summary>
        /// <param name="name"> feature name</param>
        /// <param name="claims">user claims</param>
        /// <returns></returns>
        public bool IsEnabled(string name, IEnumerable<Claim> claims)
        {
            return IsEnabled(name, claims.ToDictionary(k => k.Type, v => v.Value));
        }
        /// <summary>
        /// Determines whether the feature is enabled for given set of user claims
        /// </summary>
        /// <param name="name"> feature name</param>
        /// <param name="claims">user claims</param>
        /// <returns></returns>
        public bool IsEnabled(string name, IDictionary<string, string> claims)
        {
            try
            {
                if (claims == null)
                    return false;                             

                var featureName = new FeatureName(name);

                var features = featureStore.FindStartsWith(featureName.Value).ToList();
                if (!features.Any())
                    return false;

                var feature = features.FirstOrDefault(x => x.Name.Value.Equals(featureName.Value, StringComparison.OrdinalIgnoreCase));

                if (feature == null)
                    return false;

                return feature.IsEnabled(claims);
            }
            catch (Exception ex)
            {
                Configuration.Logger?.Warn($"Action='IsEnabled', Feature='{name}', Exception='{ex}'.");

                return false;
            }
        }
       
    }
}