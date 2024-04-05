using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace FeatureOne.SQL.StorageProvider
{
    internal class DbRepository : IDbRepository
    {
        private readonly SQLConfiguration sqlConfiguration;

        public DbRepository(SQLConfiguration sqlConfiguration)
        {
            this.sqlConfiguration = sqlConfiguration ??
                throw new ArgumentNullException(nameof(SQLConfiguration));
        }

        public DbRecord[] GetByName(string name)
        {
            var dbRecords = new List<DbRecord>();

            var factory = DbProviderFactories.GetFactory(sqlConfiguration.ConnectionSettings.ProviderName);

            if (factory == null)
                throw new InvalidOperationException($"Provider: {sqlConfiguration.ConnectionSettings.ProviderName} is not supported. Please register entry in DbProviderFactories ");

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = sqlConfiguration.ConnectionSettings.ConnectionString;

                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = sqlConfiguration.FeatureTable.CreateSQL(name);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                        dbRecords.Add(new DbRecord { Name = reader.GetString(0), Toggle = reader.GetString(1) });

                    reader.Close();
                }

                connection.Close();
            }

            return dbRecords.ToArray();
        }
    }
}