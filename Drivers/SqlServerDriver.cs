using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using mFFramework.LogManager;
using mFFramework.Types;
using mFFramework.Utilities;

namespace mFFramework.Drivers
{
 
    /// <summary>
    /// Driver di connessione a un database SqlServer
    /// </summary>
    public class SqlServerDriver
    {

        private Connection connection;

        private SqlConnection sqlconnection;
        private SqlCommand sqlcommand;
        private SqlDataReader sdr;
        private SqlDataAdapter sda;
        private List<SqlParameter> sqlparameters;

        private DataTable schemaDataTable;
        private DataTable schemaStoredProcedure;
        private List<DBColumn> columns;
        private DataRow[] dataRows;

        private bool congruency;

        private string messageExceptionManaged;
        private string columnName;
        private string columnType;
        private string propertyType;
        private string[] vcolumnType;

        private PropertyInfo property;

        /// <summary>
        /// 
        /// </summary>
        public Exception __Exception { get {
                return !messageExceptionManaged.IsVoid()
                                               ? new Exception("[mFFramework]" + messageExceptionManaged)
                                               : null;
            } }


        /// <summary>
        /// Restituisce la connessione a SqlServer
        /// </summary>
        public Connection Connection
        {
            get
            {
                return connection;
            }

        }


      


         #region ----  PRIVATE  ----



        /// <summary>
        /// Verifica la congruenza tra un oggetto generico e un ben preciso schema DataTable fornito dalla storedprocedure
        /// </summary>
        /// <typeparam name="TypeSource"></typeparam>
        /// <param name="typesource"></param>
        /// <param name="datarows"></param>
        /// <returns></returns>
        private bool CheckCongruencySchema<TypeSource>(TypeSource typesource, DataRowCollection datarows)
        {

            messageExceptionManaged = null;
            congruency = true;
            columns = new List<DBColumn>();

            foreach (DataRow dataRow in datarows)
            {

                // recupera il name della colonna e il tipo
                columnName = dataRow["ColumnName"].ToString();

                vcolumnType = dataRow["ProviderSpecificDataType"].ToString().Split('.');
                columnType = vcolumnType[vcolumnType.Length - 1].ToLower().Replace("sql", string.Empty)
                                                                          .Replace("xml", "string"); // mappa manualmente xml(SQL) in string(CLR)

                // recupera il name della proprietà e il tipo
                property = typesource.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property.PropertyType.BaseType.Name.ToLower().Contains("enum"))
                    propertyType = "int32";
                else
                    propertyType = (property != null) 
                                        ? property.PropertyType.Name.ToLower().Replace("byte[]", "binary") // mappa manualmente byte[](CLR) in binary(SQL)  
                                        : string.Empty;



                // congruenza: nomi e tipo devono coincidere tra colonne e typeSource
                if (property == null || propertyType != columnType)
                {
                    messageExceptionManaged = "Congruenza non trovata tra (columnName = " + columnName + ", columnType = " + columnType + ") e (propertyName = " + ((property != null) ? property.Name : string.Empty) + ", propertyType = " + propertyType + ")";
                    congruency = false;
                    break;
                }
                else
                    columns.Add(new DBColumn() { Name = columnName, Type = columnType });


            }

            return congruency;

        }



        /// <summary>
        /// Verifica la congruenza di una storedprocedure, controllandone sia l'esistenza che la firma dei parametri (nome e tipo) 
        /// </summary>
        /// <param name="storedProcedureName">Nome della storedprocedure da controllare</param>
        /// <returns>True se la storedprocedure è congruente, false in caso contrario</returns>
        private bool CheckCongruencyStoredProcedure(string storedProcedureName)
        {

            // Inizializzo gli oggetti del metodo
            messageExceptionManaged = null;
            schemaStoredProcedure = new DataTable();
            sda = new SqlDataAdapter();
            sqlcommand.CommandType = CommandType.Text;
            dataRows = null;
            congruency = true;


            // VERIFICO se esiste una storedprocedure con lo stesso nome
            sqlcommand.CommandText = DriversRisorse.CheckStoredProcedureName.Replace("@StoredProcedureName", storedProcedureName);
            sda.SelectCommand = sqlcommand;
            sda.Fill(schemaStoredProcedure);

            if (Convert.ToInt32(schemaStoredProcedure.Rows[0]["CheckSP"]) == 0)
            {
                messageExceptionManaged = "La StoredProcedure " + storedProcedureName + " non esiste";
                return false;
            }

            // VERIFICO quali sono i parametri della storedprocedure
            schemaStoredProcedure.Rows.Clear();
            sqlcommand.CommandText = DriversRisorse.CheckStoredProcedureParameters.Replace("@StoredProcedureName", storedProcedureName);
            sda.SelectCommand = sqlcommand;
            sda.Fill(schemaStoredProcedure);


            // VERIFICO se il numero di parametri della storedprocedure è uguale al numero di parametri passati dal driver
            if (sqlcommand.Parameters.Count != schemaStoredProcedure.Rows.Count)
            {
                messageExceptionManaged = "Il numero di parametri passati non corrisponde a quelli della StoredProcedure " + storedProcedureName;
                return false;
            }


            // VERIFICO se ogni parametro della storedprocedure è congruente con quelli passati dal driver
            foreach (SqlParameter sqlparameter in sqlcommand.Parameters)
            {

                // ne verifico il nome, il tipo 
                dataRows = schemaStoredProcedure.Select("ParameterName = '" + sqlparameter.ParameterName + "' and ParameterType='" + sqlparameter.SqlDbType.ToString() + "'"); 
                
                // se non ci sono righe o sono  > 1 allora fallisce la verifica dei parametri
                if (dataRows.Length != 1)
                {
                    messageExceptionManaged = "Non esiste il parametro ParameterName = '" + sqlparameter.ParameterName + "' and ParameterType='" + sqlparameter.SqlDbType.ToString() + "'"; 
                    congruency = false;
                    break;
                }

            }

            return congruency;


        }


        /// <summary>
        /// Raccoglie i valori dei parametri di output, eventuali, alla fine di una StoredProcedure
        /// </summary>
        private void CollectOutputParametersValue()
        {

            sqlcommand.Parameters.OfType<SqlParameter>()
                                 .Where(p => p.Direction == ParameterDirection.Output)
                                 .ToList()
                                 .ForEach(op => sqlparameters.Find(p => p.ParameterName.Identify() == (op.ParameterName).Identify()).Value = op.Value);

        }


        /// <summary>
        /// Chiude una connessione 
        /// </summary>
        private void CloseConnection()
        {

            // raccolgo tutti i valori degli, eventuali, parametri di ouput
            CollectOutputParametersValue();

            // elimino i parametri alla chiusura della connessione: questo serve per evitare che, richiamando di nuovo il metodo, rimangano appesi gli stessi parametri
            sqlcommand.Parameters.Clear();

            if (sqlconnection.State != ConnectionState.Closed)
            {
                sqlcommand.Dispose();
                sqlconnection.Close();
            }

        }


        /// <summary>
        /// Riempie i singoli oggetti generici
        /// </summary>
        /// <typeparam name="TypeSource"></typeparam>
        /// <param name="typesource"></param>
        /// <returns></returns>
        private TypeSource Filling<TypeSource>(TypeSource typesource)
        where TypeSource : new()
        {

            typesource = new TypeSource();

            columns.ForEach(column =>
            {


                switch (column.Type)
                {

                    // tinyint
                    case "byte":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToByte(sdr[column.Name]) : (byte?)null, null);
                        break;

                    // smallint
                    case "int16":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToInt16(sdr[column.Name]) : (Int16?)null, null);
                        break;

                    // int
                    case "int32":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToInt32(sdr[column.Name]) : (Int32?)null, null);
                        break;

                    // bigint
                    case "int64":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToInt64(sdr[column.Name]) : (Int64?)null, null);
                        break;

                    // decimal
                    case "decimal":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToDecimal(sdr[column.Name]) : (decimal?)null, null);
                        break;

                    // real
                    case "single":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToSingle(sdr[column.Name]) : (float?)null, null);
                        break;

                    // datetime, date
                    case "datetime":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToDateTime(sdr[column.Name]) : (DateTime?)null, null);
                        break;

                    // bit
                    case "boolean":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToBoolean(sdr[column.Name]) : (bool?)null, null);
                        break;

                    // float
                    case "double":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? Convert.ToDouble(sdr[column.Name]) : (double?)null, null);
                        break;

                    // image, varbinary, binary
                    case "binary":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? (byte[])sdr[column.Name] : null, null);
                        break;

                    // varchar, char, nvarchar, nchar, text, ntext
                    default:
                    case "string":
                        typesource.GetType().GetProperty(column.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(typesource, !sdr.IsNull(column.Name) ? sdr[column.Name].ToString() : null, null);
                        break;



                }


            });

            return typesource;

        }



        #endregion ----  PRIVATE  ----




        #region ----  PUBLIC  ----



        ///// <summary>
        ///// Costruttore del driver SqlServer
        ///// </summary>
        ///// <param name="connectionString">Stringa di connessione al database</param>
        //public SqlServerDriver(string connectionString)
        //{

        //    sqlconnection = new SqlConnection(connectionString);
        //    sqlcommand = new SqlCommand();
        //    sqlparameters = new List<SqlParameter>();

        //}


        /// <summary>
        /// Costruttore del driver
        /// </summary>
        /// <param name="serverName">Nome del server SqlServer</param>
        /// <param name="databaseName">Nome del database</param>
        /// <param name="userID">Username</param>
        /// <param name="password">Password</param>
        /// <param name="connectionTimeout">Timeout connessione (default = 30 secondi)</param>
        public SqlServerDriver(string serverName, string databaseName, string userID, string password, int connectionTimeout = 30)
        {
            connection = new Connection(serverName, databaseName, userID, password);

            sqlconnection = new SqlConnection(connection.SqlConnectionString);
            sqlcommand = new SqlCommand();
            sqlcommand.CommandTimeout = connectionTimeout;
            sqlparameters = new List<SqlParameter>();

        }


        /// <summary>
        /// Costruttore del driver
        /// </summary>
        /// <param name="connectionString">String di connessione al database</param>
        public SqlServerDriver(string connectionString)
        {

            sqlconnection = new SqlConnection(connectionString);
            sqlcommand = new SqlCommand();
            sqlcommand.CommandTimeout = sqlconnection.ConnectionTimeout;
            sqlparameters = new List<SqlParameter>();

        }


        /// <summary>
        /// Recupera il valore di un parametro di Output
        /// </summary>
        /// <param name="parameterName">Nome del parametro di Output</param>
        /// <returns>Il valore del parametro di Output</returns>
        public object GetOutputParameterValue(string parameterName)
        {
            SqlParameter sqlparameter = sqlparameters.Find(p => p.ParameterName.Identify() == ("@" + parameterName).Identify());

            return sqlparameter != null ? sqlparameter.Value : null;
        }

        

        /// <summary>
        /// Aggiunge un parametro di Input di una storedprocedure
        /// </summary>
        /// <param name="parameterName">Il nome del parametro</param>
        /// <param name="parameterType">Il tipo del parametro</param>
        /// <param name="value">Il valore del parametro</param>
        public void AddInput(string parameterName, SqlDbType parameterType, object value)
        {
 
            sqlparameters.Add(new SqlParameter() { ParameterName = "@" + parameterName, SqlDbType = parameterType, Value = value, Direction = ParameterDirection.Input });
            sqlcommand.Parameters.Add(sqlparameters[sqlparameters.Count - 1]);

        }


        /// <summary>
        /// Aggiunge un parametro di Output di una storedprocedure
        /// </summary>
        /// <param name="parameterName">Il nome del parametro</param>
        /// <param name="parameterType">Il tipo del parametro</param>
        /// <param name="size">La dimensione del tipo di parametro</param>
        public void AddOutput(string parameterName, SqlDbType parameterType, int? size = null)
        {
            if (size != null && size.HasValue)
                sqlparameters.Add(new SqlParameter() { ParameterName = "@" + parameterName, SqlDbType = parameterType, Size = size.Value, Direction = ParameterDirection.Output });
            else
                sqlparameters.Add(new SqlParameter() { ParameterName = "@" + parameterName, SqlDbType = parameterType, Direction = ParameterDirection.Output });

            sqlcommand.Parameters.Add(sqlparameters[sqlparameters.Count - 1]);

        }




        /// <summary>
        /// Ottiene una lista di oggetti da una query/storedprocedure
        /// </summary>
        /// <typeparam name="TypeSource">la tipologia di oggetti contenuti nella lista</typeparam>
        /// <param name="query">La query o il nome della storedprocedure da eseguire</param>
        /// <param name="sqlCommandType">La tipologia di comando: query o storedprocedure</param>
        /// <returns>La lista di oggetti</returns>
        public List<TypeSource> GetList<TypeSource>(string query, SqlCommandType sqlCommandType)
        where TypeSource : new()
        {
            messageExceptionManaged = null;

            try
            {


                sqlconnection.Open();

                sqlcommand.Connection = sqlconnection;


                // se la query è una storedprocedure verifico la sua congruenza
                if (sqlCommandType == SqlCommandType.StoredProcedure && !CheckCongruencyStoredProcedure(query))
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception(messageExceptionManaged), TypeError.Managed);
                    CloseConnection();
                    return default(List<TypeSource>);
                }


                // imposto la query
                sqlcommand.CommandText = query;
                sqlcommand.CommandType = sqlCommandType == SqlCommandType.Text
                                                ? CommandType.Text
                                                : sqlCommandType == SqlCommandType.StoredProcedure
                                                    ? CommandType.StoredProcedure
                                                    : CommandType.TableDirect;


                // eseguo la query
                sdr = sqlcommand.ExecuteReader();


                // recupero lo schema del datatable
                schemaDataTable = new DataTable();
                schemaDataTable = sdr.GetSchemaTable();


                // verifico la congruenza dell'oggetto TypeSource con lo schemaDataTable della query
                if (!CheckCongruencySchema<TypeSource>(new TypeSource(), schemaDataTable.Rows))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception(messageExceptionManaged), TypeError.Managed);
                    CloseConnection();
                    return default(List<TypeSource>);
                }


                // costruisco la lista di oggetti a partire dallo schemaDataTable e dal lettore DataReader
                List<TypeSource> typesources = new List<TypeSource>();
                TypeSource typesource = default(TypeSource);
                while (sdr.Read())
                    typesources.Add(Filling<TypeSource>(typesource));


                return typesources;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<TypeSource>);

            }
            #endregion Manage Error
            finally
            {
                // svuota l'elenco dei parametri del command, per un successivo utilizzo
                CloseConnection();

            }


        }



        /// <summary>
        /// Prova ad eseguire una Query (non di Select)
        /// </summary>
        /// <example>L'esempio mostra come costruire una StoredProcedure (non di Select) che produca Outcome quale esito del metodo <see cref="SqlServerDriver.TryExecute"/>.
        /// <code> 
        /// CREATE PROCEDURE NomeStoredProcedure
        /// (
        ///     // qui : elenco dei parametri della storedprocedure
        /// )
        /// AS
        /// BEGIN
        ///	
        ///     DECLARE @NAME_TRAN CHAR(22) = 'EsempioStoredProcedure'
        ///	
        ///     BEGIN TRY
        ///
        ///         BEGIN TRAN @NAME_TRAN
        ///
        ///         // qui : corpo delle istruzioni della storedprocedure
        ///
        ///         // questa istruzione riempie Outcome indicando che non ci sono errori
        ///         SELECT 0 ERRORNUMBER, NULL ERRORMESSAGE, NULL ERRORLINE
        ///	
        ///         COMMIT
        ///         
        ///     END TRY
        ///     BEGIN CATCH
        ///
        ///	        IF @@TRANCOUNT > 0
        ///		        ROLLBACK TRAN @NAME_TRAN
        ///			
        ///         // questa istruzione riempie Outcome indicando che c'è un errore
        ///         // il numero di errore è impostato negativo
        ///	        SELECT -ERROR_NUMBER() ERRORNUMBER, ERROR_MESSAGE() ERRORMESSAGE, ERROR_LINE() ERRORLINE
        ///
        ///     END CATCH
        ///		
        ///END
        /// </code> 
        /// </example>
        /// <param name="query">Il nome della StoredProcedure, o il testo della query, da eseguire</param>
        /// <param name="sqlCommandType">La tipologia di query (se testo o StoredProcedure)</param>
        /// <returns>L'esito Outcome</returns>
        public OutcomeStoredProcedure TryExecute(string query, SqlCommandType sqlCommandType)
        {

            try
            {
                // Inizializzo gli oggetti del metodo
                schemaDataTable = new DataTable();
                sda = new SqlDataAdapter();


                sqlconnection.Open();

                sqlcommand.Connection = sqlconnection;


                // se la query è una storedprocedure, e questa non esiste
                if (sqlCommandType == SqlCommandType.StoredProcedure && !CheckCongruencyStoredProcedure(query))
                    return default(OutcomeStoredProcedure);

                sqlcommand.CommandText = query;
                sqlcommand.CommandType = sqlCommandType == SqlCommandType.Text
                                                ? CommandType.Text
                                                : sqlCommandType == SqlCommandType.StoredProcedure
                                                    ? CommandType.StoredProcedure
                                                    : CommandType.TableDirect;
                sda.SelectCommand = sqlcommand;
                sda.Fill(schemaDataTable);

                sqlconnection.Close();

                if (schemaDataTable.Rows.Count != 1)
                    return default(OutcomeStoredProcedure);


                return new OutcomeStoredProcedure
                {


                    NumberError = Convert.ToInt32((schemaDataTable.Rows[0][0] != DBNull.Value) ? schemaDataTable.Rows[0][0] : "-1")
                   ,
                    MessageError = schemaDataTable.Rows[0][1].ToString()
                   ,
                    LineError = Convert.ToInt32((schemaDataTable.Rows[0][2] != DBNull.Value) ? schemaDataTable.Rows[0][2] : "-1")
                    ,
                    PipelineOutParameters = schemaDataTable.Columns.Count == 4 && schemaDataTable.Rows[0][3] != DBNull.Value ? schemaDataTable.Rows[0][3].ToString() : null


                };



            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(OutcomeStoredProcedure);

            }
            #endregion Manage Error
            finally
            {

                // svuota l'elenco dei parametri del command, per un successivo utilizzo
                CloseConnection();

            }


        }


        #endregion ----  PUBLIC  ----

      



        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sqlCommandType"></param>
        public int Execute(string query, SqlCommandType sqlCommandType)
        {

            sqlconnection.Open();

            sqlcommand.Connection = sqlconnection;


            // se la query è una storedprocedure, e questa non esiste
            if (sqlCommandType == SqlCommandType.StoredProcedure && !CheckCongruencyStoredProcedure(query))
                return Int32.MinValue;

            sqlcommand.CommandText = query;
            sqlcommand.CommandType = sqlCommandType == SqlCommandType.Text
                                                ? CommandType.Text
                                                : sqlCommandType == SqlCommandType.StoredProcedure
                                                    ? CommandType.StoredProcedure
                                                    : CommandType.TableDirect;
            int rowsaffected = sqlcommand.ExecuteNonQuery();

            sqlconnection.Close();


            return rowsaffected;
        
        
        }


    

       
    }



    /// <summary>
    /// Oggetto Colonna, rappresentante un generico campo restituito dal recordset della query/storedprocedure
    /// </summary>
    public class DBColumn
    {

        /// <summary>
        /// Nome della colonna
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tipo di dati contenuti nella colonna
        /// </summary>
        public string Type { get; set; }

    
    }



    /// <summary>
    /// Metodi estesi per l'oggetto SqlDataReader
    /// </summary>
    public static class SqlDataReaderExtension
    { 
    
        /// <summary>
        /// Controlla se il campo della colonna del SqlDataReader è nullo
        /// </summary>
        /// <param name="sdr">SqlDataReader</param>
        /// <param name="columnName">Nome dela colonna</param>
        /// <returns>True, se il campo è nullo; False altrimenti</returns>
        public static bool IsNull(this SqlDataReader sdr, string columnName)
        {

            if (sdr[columnName] != DBNull.Value)
                return false;

            return true;
        
        }
    
    }



    /// <summary>
    /// Metodi statici per la verifica di SqlServer e delle sue connessioni
    /// </summary>
    public static class SqlServerChecker
    {

        /// <summary>
        /// 
        /// </summary>
        private static SqlConnection sqlConnectionTest;
        /// <summary>
        /// 
        /// </summary>
        private static SqlCommand sqlCommandTest;
        /// <summary>
        /// 
        /// </summary>
        private static string dbName;
        /// <summary>
        /// 
        /// </summary>
        private static int checkDB;
        /// <summary>
        /// 
        /// </summary>
        private static ServiceController serviceController;


        /// <summary>
        /// Estrae il nome del database dalla stringa di connessione
        /// </summary>
        /// <param name="connectionString">Stringa di connessione</param>
        /// <returns>Il nome del database</returns>
        private static string ExtractDBNameFromConnectionString(string connectionString)
        {

            return connectionString.Split(';').Where(pc => pc.ToLower().Contains("initial catalog")).FirstOrDefault();

        }


        /// <summary>
        /// Verifica la connessione al database
        /// </summary>
        /// <param name="connectionString">Stringa di connessione</param>
        /// <returns>True, se il database è raggiungibile; False, in caso contrario</returns>
        public static bool CheckConnection(string connectionString)
        {


            if (string.IsNullOrEmpty(connectionString))
                return false;

            sqlConnectionTest = new SqlConnection(connectionString);

            try
            {
                sqlConnectionTest.Open();
                sqlConnectionTest.Close();

                sqlConnectionTest.Dispose();

                return true;
            }
            catch
            {

                if (sqlConnectionTest.State != ConnectionState.Closed)
                    sqlConnectionTest.Close();

                sqlConnectionTest.Dispose();

                return false;
            }

        }


        /// <summary>
        /// Verifica se il database esiste sul server
        /// </summary>
        /// <param name="connectionString">Stringa di connessione</param>
        /// <returns>Un valore > 0, se il database esiste; -1, in caso contrario</returns>
        public static int CheckDB(string connectionString)
        {

            sqlConnectionTest = new SqlConnection(connectionString);

            try
            {

                dbName = ExtractDBNameFromConnectionString(connectionString);

                sqlConnectionTest.Open();
                sqlCommandTest = sqlConnectionTest.CreateCommand();

                sqlCommandTest.CommandText = "Use master; SELECT count(*) FROM MASTER.DBO.SYSDATABASES WHERE NAME = N'" + dbName + "'";
                checkDB = Convert.ToInt32(sqlCommandTest.ExecuteScalar());
                sqlConnectionTest.Close();

                return checkDB;
            }
            catch
            {

                if (sqlConnectionTest.State != ConnectionState.Closed)
                    sqlConnectionTest.Close();

                return -1;

            }


        }


        /// <summary>
        /// Controlla che SqlServer sia installato e il suo servizio avviato.
        /// </summary>
        /// <param name="sqlserver">Il nome del servizio SqlServer</param>
        /// <param name="sqlserverexpress">Il nome del servizio SqlServer Express</param>
        /// <returns>True: il servizio è attivo, False: il servizio non è attivo</returns>
        public static bool CheckSqlServerService(string sqlserver, string sqlserverexpress)
        {

            // imposta il nome del server sql di default
            string ServerSql = "*";
            serviceController = new ServiceController();

            serviceController.ServiceName = sqlserver;
            if (!Functions.CheckService(serviceController.ServiceName))
            {
                // se il servizio SqlServer non è attivo verifica se lo sia il servizio SqlServer Express
                serviceController.ServiceName = sqlserverexpress;
                if (!Functions.CheckService(serviceController.ServiceName))
                    return false;

                // se il servizio sqlserver express esiste, imposta il nome del server sqlexpress
                ServerSql = Environment.MachineName + @"\SQLEXPRESS";

            }
            else // se il servizio sqlserver esiste, imposta il nome del server sql
                ServerSql = Environment.MachineName;

            //// verifica se il servizio sqlserver (express o meno) è in esecuzione
            //if (serviceController.Status != ServiceControllerStatus.Running)
            //    return false;


            return true;

        }
    
    }



}
