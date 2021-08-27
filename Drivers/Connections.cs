using System.Data.SqlClient;

namespace mFFramework.Drivers
{

    /// <summary>
    /// 
    /// </summary>
    public class Connections
    {

        private static SqlConnectionStringBuilder sqlBuilder;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="connectionTimeout"></param>
        /// <returns></returns>
        public static string GetConncetionString(string server, string database, string user, string password, int connectionTimeout = 30)
        {

            sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = server.Trim();
            sqlBuilder.InitialCatalog = database.Trim();
            sqlBuilder.UserID = user.Trim();
            sqlBuilder.Password = password.Trim();
            sqlBuilder.ConnectTimeout = connectionTimeout;
            
            
            return sqlBuilder.ToString();
        
        }
    }
}
