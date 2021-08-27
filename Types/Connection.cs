using System.Data.EntityClient;
using System.Data.SqlClient;


namespace mFFramework.Types
{

    /// <summary>
    /// Definisce la connessione nel mFFramework, sia a SqlServer che a EF
    /// </summary>
    public class Connection
    {

        private string sqlConnectionString;
        private string entityConnectionString;

        private SqlConnectionStringBuilder sqlBuilder;
        private EntityConnectionStringBuilder entityBuilder;

        /// <summary>
        /// 
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }




        /// <summary>
        /// Costruttore della connessione 
        /// </summary>
        /// <param name="serverName">Nome del server SqlServer</param>
        /// <param name="databaseName">Nome del database</param>
        /// <param name="userID">Username</param>
        /// <param name="password">Password</param>
        /// <param name="providerName">Nome del provider (solo per connessioni a modelli EF)</param>
        /// <param name="metaData">Metadata (solo per connessioni a modelli EF)</param>
        public Connection(string serverName, string databaseName, string userID, string password, string providerName = null, string metaData = null)
        {

            ServerName = serverName;
            DatabaseName = databaseName;
            UserID = userID;
            Password = password;

            // Imposta la stringa di connessione al database
            sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.UserID = userID;
            sqlBuilder.Password = password;
            sqlConnectionString = sqlBuilder.ToString();

            if (!string.IsNullOrEmpty(providerName) && !string.IsNullOrEmpty(metaData))
            {
                entityBuilder = new EntityConnectionStringBuilder();
                entityBuilder.Provider = providerName;
                entityBuilder.ProviderConnectionString = sqlConnectionString;
                entityBuilder.Metadata = metaData;
                entityConnectionString = entityBuilder.ToString();
            }

        }


        /// <summary>
        /// Restituisce la stringa di connessione a un database SqlServer
        /// </summary>
        public string SqlConnectionString
        {

            get { return sqlConnectionString; }
        
        }

        /// <summary>
        /// Restituisce la stringa di connessione a un modello EF
        /// </summary>
        public string EntityConnectionString
        {

            get { return entityConnectionString; }

        }

    }
}
