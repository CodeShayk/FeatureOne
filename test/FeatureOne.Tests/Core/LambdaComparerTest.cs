namespace FeatureOne.Tests.Core;

[TestFixture]
public class LambdaComparerTest
{
    [Test]
    public void LambdaComparer_Equals_ShouldUseProvidedFunction()
    {
        var comparer = new LambdaComparer<string>((x, y) => x.Equals(y, StringComparison.OrdinalIgnoreCase));

        Assert.That(comparer.Equals("Hello", "hello"), Is.True);
        Assert.That(comparer.Equals("Hello", "World"), Is.False);
    }

    [Test]
    public void LambdaComparer_GetHashCode_ShouldReturnObjectHashCode()
    {
        var comparer = new LambdaComparer<string>((x, y) => x == y);
        var value = "test";

        Assert.That(comparer.GetHashCode(value), Is.EqualTo(value.GetHashCode()));
    }

    [Test]
    public void LambdaComparer_NullEqualityFunction_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new LambdaComparer<string>(null));
    }
}
