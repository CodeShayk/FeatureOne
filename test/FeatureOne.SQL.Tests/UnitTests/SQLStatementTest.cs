using FeatureOne.SQL.StorageProvider;

namespace FeatureOne.SQL.Tests.UnitTests
{
    [TestFixture]
    internal class SQLStatementTest
    {
        [Test]
        public void TestCreateSQL()
        {
            var table = new FeatureTable();
            var sql = table.CreateSQL("Foo");
            Assert.AreEqual("Select Name, Toggle From TFeatures Where Archived = 0 and Name = 'Foo'", sql);
        }
    }
}