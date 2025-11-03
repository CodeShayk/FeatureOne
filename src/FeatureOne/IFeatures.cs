using System.Collections.Generic;
using System.Security.Claims;

namespace FeatureOne
{
    public interface IFeatures
    {
        bool IsEnabled(string name);
        bool IsEnabled(string name, ClaimsPrincipal principal);
        bool IsEnabled(string name, IDictionary<string, string> claims);
        bool IsEnabled(string name, IEnumerable<Claim> claims);
    }
}