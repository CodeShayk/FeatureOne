using System.Runtime.Caching;
using FeatureOne.Cache;

namespace FeatureOne.Tests.Cache;

[TestFixture]
public class CacheTests
{
    [Test]
    public void CacheSettings_DefaultValues_ShouldBeCorrect()
    {
        var settings = new CacheSettings();

        Assert.That(settings.EnableCache, Is.False);
        Assert.That(settings.Expiry, Is.Not.Null);
        Assert.That(settings.Expiry.InMinutes, Is.EqualTo(60));
        Assert.That(settings.Expiry.Type, Is.EqualTo(CacheExpiryType.Absolute));
    }

    [Test]
    public void CacheSettings_SetProperties_ShouldWork()
    {
        var expiry = new CacheExpiry { InMinutes = 30, Type = CacheExpiryType.Sliding };
        var settings = new CacheSettings { EnableCache = true, Expiry = expiry };

        Assert.That(settings.EnableCache, Is.True);
        Assert.That(settings.Expiry.InMinutes, Is.EqualTo(30));
        Assert.That(settings.Expiry.Type, Is.EqualTo(CacheExpiryType.Sliding));
    }

    [Test]
    public void ExpiryPolicyExtension_AbsoluteExpiry_ShouldReturnAbsolutePolicy()
    {
        var expiry = new CacheExpiry { InMinutes = 10, Type = CacheExpiryType.Absolute };

        var policy = expiry.GetPolicy();

        Assert.That(policy, Is.Not.Null);
        Assert.That(policy.AbsoluteExpiration, Is.Not.EqualTo(DateTimeOffset.MinValue));
        Assert.That(policy.SlidingExpiration, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void ExpiryPolicyExtension_SlidingExpiry_ShouldReturnSlidingPolicy()
    {
        var expiry = new CacheExpiry { InMinutes = 15, Type = CacheExpiryType.Sliding };

        var policy = expiry.GetPolicy();

        Assert.That(policy, Is.Not.Null);
        Assert.That(policy.SlidingExpiration, Is.EqualTo(TimeSpan.FromMinutes(15)));
    }

    [Test]
    public void FeatureCache_AddAndGet_ShouldWork()
    {
        var cache = new FeatureCache();
        var key = $"test-key-{Guid.NewGuid()}";
        var value = new object();
        var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10) };

        cache.Add(key, value, policy);
        var result = cache.Get(key);

        Assert.That(result, Is.EqualTo(value));
    }

    [Test]
    public void FeatureCache_GetNonExistentKey_ShouldReturnNull()
    {
        var cache = new FeatureCache();

        var result = cache.Get($"non-existent-key-{Guid.NewGuid()}");

        Assert.That(result, Is.Null);
    }
}
