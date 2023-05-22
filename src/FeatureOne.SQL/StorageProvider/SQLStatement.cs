namespace FeatureOne.SQL.StorageProvider
{
    public static class SQLStatement
    {
        public static string CreateSQL(this FeatureTable table, string featureName)
        {
            return $"Select {table.NameColumn}, {table.ToggleColumn} From {table.TableName} Where {table.ArchivedColumn} = 0 and {table.NameColumn} = '{featureName}'";
        }
    }
}