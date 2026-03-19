using Microsoft.Extensions.DependencyInjection;

namespace FeatureOne.Tests.Extensions;

[TestFixture]
public class FeatureOneServiceExtensionsTests
{
    [Test]
    public void AddFeatureOne_WithNullFactory_ShouldThrow()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => services.AddFeatureOne(null));
    }
}
