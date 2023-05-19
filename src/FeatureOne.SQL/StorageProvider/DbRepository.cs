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
        private readonly IFeatureLogger logger;

        public DbRepository(SQLConfiguration sqlConfiguration, IFeatureLogger logger)
        {
            this.sqlConfiguration = sqlConfiguration ?? throw new ArgumentNullException(nameof(SQLConfiguration));
            this.logger = logger;

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
            try
            {
                var factory = DbProviderFactories.GetFactory(sqlConfiguration.ConnectionSettings.ProviderName);

                if (factory == null)
                    throw new InvalidOperationException($"Provider: {sqlConfiguration.ConnectionSettings.ProviderName} is not supported. Please register entry in DbProviderFactories ");

                using (var connection = factory.CreateConnection())
                {
                    connection.ConnectionString = sqlConfiguration.ConnectionSettings.ConnectionString;

                    connection.Open();
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = sqlConfiguration.FeatureTable.CreateSQL(name);

                            var reader = command.ExecuteReader();
                            while (reader.Read())
                                dbRecords.Add(new DbRecord { Name = reader.GetString(0), Toggle = reader.GetString(1) });

                            reader.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        logger?.Error($"FeatureOne.SQL, Action='Repository.GetByName()', Exception='{e}'.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                logger?.Error($"FeatureOne.SQL, Action='Repository.GetByName()', Success=Feature Count'{dbRecords.Count}'.");

                return dbRecords.ToArray();
            }
            catch (Exception ex)
            {
                logger?.Error($"FeatureOne.SQL, Action='Repository.GetByName()', Exception='{ex}'.");
            }

            return Array.Empty<DbRecord>();
        }
    }
}