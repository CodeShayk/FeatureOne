using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using MySql.Data.MySqlClient;
using Npgsql;

namespace FeatureOne.SQL.StorageProvider
{
    internal class DbRepository : IDbRepository
    {
        private readonly SQLConfiguration sqlConfiguration;

        public DbRepository(SQLConfiguration configuration)
        {
            this.sqlConfiguration = configuration ?? throw new ArgumentNullException(nameof(SQLConfiguration));

            var names = DbProviderFactories.GetProviderInvariantNames();

            if (!names.Any(x => x.Equals("System.Data.SqlClient") && SqlClientFactory.Instance != null))
                DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            if (!names.Any(x => x.Equals("System.Data.Odbc")) && OdbcFactory.Instance != null)
                DbProviderFactories.RegisterFactory("System.Data.Odbc", OdbcFactory.Instance);

            if (!names.Any(x => x.Equals("System.Data.OleDb")) && OleDbFactory.Instance != null)
                DbProviderFactories.RegisterFactory("System.Data.OleDb", OleDbFactory.Instance);

            if (!names.Any(x => x.Equals("System.Data.SQLite")) && SQLiteFactory.Instance != null)
                DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);

            if (!names.Any(x => x.Equals("MySql.Data.MySqlClient")) && MySqlClientFactory.Instance != null)
                DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);

            if (!names.Any(x => x.Equals("Npgsql")) && NpgsqlFactory.Instance != null)
                DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
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