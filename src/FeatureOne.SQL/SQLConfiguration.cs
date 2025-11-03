using System;
using FeatureOne.Cache;

namespace FeatureOne.SQL
{
    public class SQLConfiguration
    {
        public SQLConfiguration()
        {
            FeatureTable = new FeatureTable();
            CacheSettings = new CacheSettings();
            ConnectionSettings = new ConnectionSettings();
        }

        /// <summary>
        /// Database settings - Connection string and provider name.
        /// </summary>
        public ConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        /// Feature table name and column aliases
        /// </summary>
        public FeatureTable FeatureTable { get; set; }

        /// <summary>
        /// Feature cache settings.
        /// </summary>
        public CacheSettings CacheSettings { get; set; }
    }

    public class ConnectionSettings
    {
        /// <summary>
        /// Connection string to feature database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Provider name for connection factory. Please see DbProviderName class for supported constants.
        /// </summary>
        /// <remarks>
        /// Supported providers:
        /// System.Data.SqlClient
        /// System.Data.Odbc
        /// System.Data.OleDb
        /// System.Data.SQLite
        /// MySql.Data.MySqlClient
        /// Npgsql
        /// </remarks>
        public string ProviderName { get; set; } = DbProviderName.MSSql;
    }

    /// <summary>
    /// Provider name for connection factories.
    /// </summary>
    public class DbProviderName
    {
        /// <summary>
        /// Provider - System.Data.SqlClient
        /// </summary>
        public const string MSSql = "System.Data.SqlClient";

        /// <summary>
        /// Provider - System.Data.Odbc
        /// </summary>
        public const string Odbc = "System.Data.Odbc";

        /// <summary>
        /// Provider - System.Data.OleDb
        /// </summary>
        public const string OleDb = "System.Data.OleDb";

        /// <summary>
        /// Provider - System.Data.SqlClient
        /// </summary>
        public const string SQLite = "System.Data.SQLite";

        /// <summary>
        /// Provider - MySql.Data.MySqlClient
        /// </summary>
        public const string MySql = "MySql.Data.MySqlClient";

        /// <summary>
        /// Provider - Npgsql
        /// </summary>
        public const string PostgresSql = "Npgsql";
    }

    /// <summary>
    /// Feature table settings to override table aliases.
    /// </summary>
    public class FeatureTable
    {
        /// <summary>
        /// Table Name for the feature table.
        /// </summary>
        public string TableName { get; set; } = "TFeatures";

        /// <summary>
        /// Feature name column name.
        /// </summary>
        public string NameColumn { get; set; } = "Name";

        /// <summary>
        /// Feature toggle column name.
        /// </summary>
        public string ToggleColumn { get; set; } = "Toggle";

        /// <summary>
        /// Feature archived column name.
        /// </summary>
        public string ArchivedColumn { get; set; } = "Archived";
    }
}