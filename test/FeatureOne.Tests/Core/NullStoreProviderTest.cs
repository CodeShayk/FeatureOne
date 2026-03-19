namespace FeatureOne.Tests.Core;

[TestFixture]
public class NullStoreProviderTest
{
    [Test]
    public void NullStoreProvider_GetByName_ShouldReturnEmpty()
    {
        var provider = new NullStoreProvider();

        var result = provider.GetByName("AnyFeature");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void NullStoreProvider_GetByName_WithNullName_ShouldReturnEmpty()
    {
        var provider = new NullStoreProvider();

        var result = provider.GetByName(null);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
