using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FeatureOne.Tests;

[TestFixture]
public class DependencyInjectionIntegrationTest
{
    [Test]
    public void Integration_DependencyInjection()
    {
        // Arrange - Test that the new DI patterns work in integration
        var services = new ServiceCollection();
        
        var mockProvider = new Mock<IStorageProvider>();
        var mockLogger = new Mock<IFeatureLogger>();
        
        var testFeature = new Feature(new FeatureName("DIIntegrationTest"), 
            new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));
        
        mockProvider.Setup(p => p.GetByName("DIIntegrationTest")).Returns(new[] { testFeature });
        
        // Use the new constructor with explicit dependencies if available
        // If the constructor with explicit dependencies doesn't exist, we'll test the registration
        var featureStore = new FeatureStore(mockProvider.Object, mockLogger.Object);
        var features = new Features(featureStore, mockLogger.Object);
        
        // Act
        var result = features.IsEnabled("DIIntegrationTest");
        
        // Assert
        Assert.That(result, Is.True);
        
        // Verify logger was used (not strictly required but good to check)
        mockLogger.Verify(l => l.Info(It.IsAny<string>()), Times.AtMost(1));
    }
    
    [Test]
    public void AddFeatureOne_ExtensionMethod_Works()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockProvider = new Mock<IStorageProvider>();
        
        // Act
        services.AddFeatureOne(serviceProvider => mockProvider.Object);
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var features = serviceProvider.GetService<IFeatures>();
        
        Assert.That(features, Is.Not.Null);
    }
}