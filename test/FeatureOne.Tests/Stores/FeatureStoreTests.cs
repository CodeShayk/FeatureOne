using Moq;

namespace FeatureOne.Tests.Stores
{
    [TestFixture]
    public class FeatureStoreTests
    {
        private Mock<IStorageProvider> _mockProvider;
        private FeatureStore _featureStore;

        [SetUp]
        public void Setup()
        {
            _mockProvider = new Mock<IStorageProvider>();
            _featureStore = new FeatureStore(_mockProvider.Object);
        }

        [Test]
        public void FeatureStore_ConstructorWithNullStorageProvider_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FeatureStore(null));
        }

        [Test]
        public void FeatureStore_ConstructorWithNullLogger_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FeatureStore(_mockProvider.Object, null));
        }

        [Test]
        public void FeatureStore_FindStartsWith_ExactMatch_ShouldWork()
        {
            // Arrange - Setup mock storage provider with feature named "FeatureA"
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName("FeatureA"))
                        .Returns(new IFeature[] { 
                            new Feature("FeatureA", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })) 
                        });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("FeatureA").ToList();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name.Value, Is.EqualTo("FeatureA"));
        }

        [Test]
        public void FeatureStore_FindStartsWith_PrefixMatch_ShouldWork()
        {
            // Arrange - Setup mock with features: "FeatureA", "FeatureASubFeature", "FeatureB"
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName(It.IsAny<string>()))
                        .Returns(new IFeature[] { 
                            new Feature("FeatureA", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })),
                            new Feature("FeatureASubFeature", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })),
                            new Feature("FeatureB", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }))
                        });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("FeatureA").ToList();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(2)); // Should return both FeatureA and FeatureA.SubFeature
            var names = result.Select(f => f.Name.Value).OrderBy(n => n).ToList();
            Assert.That(names, Contains.Item("FeatureA"));
            Assert.That(names, Contains.Item("FeatureASubFeature"));
        }

        [Test]
        public void FeatureStore_FindStartsWith_EmptyPrefix_ShouldReturnEmpty()
        {
            // Arrange
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName(It.IsAny<string>()))
                        .Returns(new IFeature[] { 
                            new Feature("TestFeature", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }))
                        });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("").ToList();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void FeatureStore_FindStartsWith_CaseInsensitive_ShouldWork()
        {
            // Arrange
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName(It.IsAny<string>()))
                        .Returns(new IFeature[] { 
                            new Feature("FeatureA", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }))
                        });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("featurea").ToList(); // lowercase prefix
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name.Value, Is.EqualTo("FeatureA"));
        }

        [Test]
        public void FeatureStore_FindStartsWith_NonMatchingPrefix_ShouldReturnEmpty()
        {
            // Arrange
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName("NonMatch"))
                        .Returns(new IFeature[] { 
                            new Feature("FeatureA", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }))
                        });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("NonMatch").ToList();
            
            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void FeatureStore_FindStartsWith_Performance_WithManyFeatures()
        {
            // Arrange
            var features = new List<IFeature>();
            for (int i = 0; i < 1000; i++)
            {
                features.Add(new Feature($"Feature{i}", new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true })));
            }
            
            var mockProvider = new Mock<IStorageProvider>();
            mockProvider.Setup(p => p.GetByName("Feature"))
                        .Returns(features.ToArray());
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var startTime = DateTime.Now;
            var result = store.FindStartsWith("Feature").ToList();
            var endTime = DateTime.Now;
            
            // Assert - Should complete in reasonable time
            Assert.That((endTime - startTime).TotalMilliseconds, Is.LessThan(1000)); // Should complete in under 1 second
            // Count how many start with "Feature"
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(100)); // Should have features like "Feature0", "Feature1", etc.
        }

        [Test]
        public void FeatureStore_FindStartsWith_NoValidToggleConditions_ShouldNotInclude()
        {
            // Arrange
            var mockProvider = new Mock<IStorageProvider>();
            var featureWithNoConditions = new Feature("FeatureA", new Toggle(Operator.Any)); // No conditions
            var featureWithValidConditions = new Feature("FeatureB", 
                new Toggle(Operator.Any, new SimpleCondition { IsEnabled = true }));
            
            mockProvider.Setup(p => p.GetByName("Feature"))
                        .Returns(new[] { featureWithNoConditions, featureWithValidConditions });
            
            var store = new FeatureStore(mockProvider.Object);
            
            // Act
            var result = store.FindStartsWith("Feature").ToList();
            
            // Assert
            // Should only include features with valid toggle conditions
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name.Value, Is.EqualTo("FeatureB"));
        }
    }
}