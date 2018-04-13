using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Supernova.Dapper.Core.Factories;

namespace Supernova.Dapper.Concrete.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        public string ConnectionString { get; }

        /// <summary>
        /// The public Dapper connection factory.
        /// </summary>
        /// <param name="connectionStringKey">The name of the default connection string entry in the 
        /// runtime configuration file. Must be inside the <connectionString/> configuration section.
        /// </param>
        /// <exception cref="ArgumentException">Thrown if supplied connectionStringKey is non instantiated, found in the configuration file or the resulting connection string is empty
        /// </exception>
        public ConnectionFactory(string connectionStringKey)
        {
            if (string.IsNullOrWhiteSpace(connectionStringKey))
            {
                throw new ArgumentException("A valid connection string key must be supplied.", nameof(connectionStringKey));
            }

            string configurationConnectionString;
            try
            {
                configurationConnectionString = ConfigurationManager
                    .ConnectionStrings[connectionStringKey]
                    .ConnectionString;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Non-existent entry in the configuration.", 
                    nameof(connectionStringKey), e);
            }

            if (string.IsNullOrWhiteSpace(configurationConnectionString))
            {
                throw new ArgumentException("Connection string entry in the configuration must have a value");
            }

            ConnectionString = configurationConnectionString;
        }

        // TODO: Implement a private method, which checks the configuration for the type of DB connection (SQL Server, Oracle DB, MySql, etc.)
        /// <summary>
        /// Opens a connection to the connection string supplied in the constructor of the connection 
        /// factory. 
        /// </summary>
        /// <returns>The opened connection to the DB.</returns>
        /// <exception cref="T:System.InvalidOperationException">Cannot open a connection without specifying a data source or server.orThe connection is already open.</exception>
        /// <exception cref="T:System.Data.SqlClient.SqlException">A connection-level error occurred while opening the connection. If the <see cref="P:System.Data.SqlClient.SqlException.Number" /> property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the <see cref="M:System.Data.SqlClient.SqlConnection.ChangePassword(System.String,System.String)" /> method for more information.The &lt;system.data.localdb&gt; tag in the app.config file has invalid or unknown elements.</exception>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">There are two entries with the same name in the &lt;localdbinstances&gt; section.</exception>
        public virtual IDbConnection GetConnection()
        {
            SqlConnection currentConnection = new SqlConnection(ConnectionString);
            currentConnection.Open();

            return currentConnection;
        }


        // TODO: Implement a private method, which checks the configuration for the type of DB connection (SQL Server, Oracle DB, MySql, etc.)
        /// <summary>
        /// Opens a connection to the supplied connection string
        /// </summary>
        /// <returns>The opened connection to the DB.</returns>
        /// <exception cref="T:System.InvalidOperationException">Cannot open a connection without specifying a data source or server.orThe connection is already open.</exception>
        /// <exception cref="T:System.Data.SqlClient.SqlException">A connection-level error occurred while opening the connection. If the <see cref="P:System.Data.SqlClient.SqlException.Number" /> property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the <see cref="M:System.Data.SqlClient.SqlConnection.ChangePassword(System.String,System.String)" /> method for more information.The &lt;system.data.localdb&gt; tag in the app.config file has invalid or unknown elements.</exception>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">There are two entries with the same name in the &lt;localdbinstances&gt; section.</exception>
        public virtual IDbConnection GetConnection(string connectionString)
        {
            SqlConnection currentConnection = new SqlConnection(connectionString);
            currentConnection.Open();

            return currentConnection;
        }
    }
}