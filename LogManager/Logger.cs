using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using mFFramework.Drivers;
using mFFramework.Types;
using mFFramework.Utilities;


namespace mFFramework.LogManager
{


    /// <summary>
    /// 
    /// </summary>
    public class Logger
    {

        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlDataReader sdr;

        private FileStream fs;
        private StreamWriter sw;

        private StringBuilder sb;
        private string connection;
        private string tableName;
        private string queryLog;
        private string signatureMethod;

        private bool verifiedConnection;

        private Object objlock;

        private ParameterInfo[] parameters;





        private bool? _isEnabled;

        /// <summary>
        /// Specifica se il Logger è abilitato per il mFFramework
        /// </summary>
        public bool IsEnabled
        {
            get { return !_isEnabled.HasValue ? false : _isEnabled.Value; }
            set { _isEnabled = value; }
        }


        private TypeLogger? _typeLog;
        /// <summary>
        /// Specifica il tipo di Log da utilizzare
        /// </summary>
        public TypeLogger TypeLog
        {
            get { return !_typeLog.HasValue ? TypeLogger.NoLog : _typeLog.Value; }
            set { _typeLog = value; }
        }



        /// <summary>
        /// I parametri del file di log: è sufficiente indicare il percorso fisico del file di log, assicurandosi che l'utente ASPNET abbia i privilegi di scrittura.
        /// </summary>
        public string ParametersLogFile { get; set; }



        /// <summary>
        /// I parametri della tabella di log
        /// </summary>
        /// <example>L'esempio mostra come costruire un parametro di log su database.
        /// <code> 
        /// Logger.Istance.ParametersLogDatabase = "Data Source={nome server};Initial Catalog={nome database};Persist Security Info=True;User ID={username};Password={password}|{nome tabella}";
        /// </code>
        /// I parametri sono divisi in due gruppi, separati dal carattere speciale |
        /// Il primo parametro è la stringa di connessione al database, mediante la quale verrà effettuato il log del mFFRamework.
        /// Il secodo parametro è il nome della tabella nella quale verranno tracciati i log del mFFramework. Questa tabella deve essere progettata rispettando due regole:
        ///   1.  tutti i campi devono essere nullabili, ad eccezione della chiave primaria
        ///   2.  deve contenere i campi seguenti:
        ///         2.1  MESSAGE                NVARCHAR(500)
        ///         2.2  STACKTRACE             NVARCHAR(1000)
        ///         2.3  SOURCE                 NVARCHAR(100)
        ///         2.4  ASSEMBLY               NAVRCHAR(100)
        ///         2.5  CLASS                  NAVRCHAR(100)
        ///         2.6  METHOD                 NAVRCHAR(500)
        ///         2.7  TYPEERROR              NAVRCHAR(10)
        ///         2.8  DATAERROR              DATETIME
        /// </example>
        public string ParametersLogDatabase { get; set; }







        // Istanzia la classe Logger come Singleton
        //_________________________________________
        private static Logger istanceLogger;



        // Costruttore interno
        private Logger() { }



        /// <summary>
        /// 
        /// </summary>
        public static Logger Istance
        {
            get
            {
                if (istanceLogger == null)
                    istanceLogger = new Logger();

                return istanceLogger;
            }
        }
        //_________________________________________









        /// <summary>
        /// Verifica se la connessione al database è effettuabile
        /// </summary>
        /// <returns>True, nel caso la connessione è stabilita; False in caso contrario</returns>
        private bool CheckConnection()
        {

            try
            {
                sqlConnection.Open();
                sqlConnection.Close();

                verifiedConnection = true;


            }
            catch
            {

                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();

                verifiedConnection = false;

            }

            return verifiedConnection;
        }



        /// <summary>
        /// Scrive le informazioni relative all'eccezione su file
        /// </summary>
        /// <param name="assembly">L'assembly che ha causato l'eccezione</param>
        /// <param name="method">Il metodo nel quale si è verificata l'eccezione</param>
        /// <param name="exception">I dettagli dell'eccezione</param>
        /// <param name="typeError">Il tipo di eccezione: Managed o Unmanaged</param>
        private void WriteOnDatabase(ref Assembly assembly, ref MethodBase method, ref Exception exception, ref TypeError typeError)
        {

            try
            {

                // verifica se la stringa di Logger su database non sia vuota
                if (string.IsNullOrEmpty(ParametersLogDatabase))
                    return;


                // ricava e verifica che i parametri di connessione siano 2: stringa di connessione e tabella di log
                string[] connections = ParametersLogDatabase.Split('|');
                if (connections == null || connections.Length != 2)
                    return;


                // ricava la stringa di connessione e il nome della tabella di log
                connection = connections[0];
                tableName = connections[1];


                // verifica se la stringa di connessione e il nome della tabella di log siano impostati 
                if (string.IsNullOrEmpty(connection) || string.IsNullOrEmpty(tableName))
                    return;


                // imposta la connessione come singleton
                if (sqlConnection == null)
                    sqlConnection = new SqlConnection(connection);


                // imposta il comando come singleton
                if (sqlCommand == null)
                {

                    sqlCommand = new SqlCommand();
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.CommandType = CommandType.Text;
                }


                // verifica che la connessione è stabilita
                if (!verifiedConnection && !CheckConnection())
                    return;

                // imposta la firma del metodo in cui si è generato l'errore (managed o unmanaged)
                SetSignatureMethod(method);


                // imposta la query di registrazione del Log
                queryLog = DriversRisorse.WriteLog
                                             .Replace("@TABELLALOG", tableName)
                                             .Replace("@MESSAGE", "'" + Functions.FilterSpecialChars(exception.Message) + "'")
                                             .Replace("@SOURCE", "'" + Functions.FilterSpecialChars(typeError == TypeError.Unmanaged ? exception.Source : "mFFramework") + "'")
                                             .Replace("@STACK_TRACE", "'" + Functions.FilterSpecialChars(typeError == TypeError.Unmanaged ? exception.StackTrace.Replace(" in ", Environment.NewLine + " in ") : string.Empty) + "'")
                                             .Replace("@ASSEMBLY", "'" + Functions.FilterSpecialChars(assembly.GetName().Name) + "'")
                                             .Replace("@CLASS", "'" + Functions.FilterSpecialChars(method.ReflectedType.Name) + "'")
                                             .Replace("@METHOD", "'" + Functions.FilterSpecialChars(signatureMethod) + "'")
                                             .Replace("@TYPE_ERROR", "'" + Functions.FilterSpecialChars(typeError.ToString()) + "'")
                                             .Replace("@DATA_ERROR", "'" + Functions.FilterSpecialChars(DateTime.Now.ToString()) + "'");




                sqlConnection.Open();

                sqlCommand.CommandText = queryLog;
                sdr = sqlCommand.ExecuteReader();

                sqlConnection.Close();

            }
            catch
            {


                if (sqlConnection.State != ConnectionState.Closed)
                    sqlConnection.Close();
            }

        }



        /// <summary>
        /// Scrive le informazioni relative all'eccezione su database
        /// </summary>
        /// <param name="assembly">L'assembly che ha causato l'eccezione</param>
        /// <param name="method">Il metodo nel quale si è verificata l'eccezione</param>
        /// <param name="exception">I dettagli dell'eccezione</param>
        /// <param name="typeError">Il tipo di eccezione: Managed o Unmanaged</param>
        private void WriteOnFile(ref Assembly assembly, ref MethodBase method, ref Exception exception, ref TypeError typeError)
        {
            try
            {

                if (string.IsNullOrEmpty(ParametersLogFile))
                    return;


                objlock = new object();
                lock (objlock)
                {

                    // Recupera il file di Log: se non esiste, lo crea prima.
                    CheckFileLog();


                    // imposta la firma del metodo
                    SetSignatureMethod(method);


                    // costruisce l'eccezione
                    sb = new StringBuilder();
                    sb.Append("Source: ");
                    sb.Append(assembly.GetName().FullName);
                    sb.Append(Environment.NewLine);
                    sb.Append("Assembly: ");
                    sb.Append(assembly.GetName().Name);
                    sb.Append(Environment.NewLine);
                    sb.Append("Class: ");
                    sb.Append(method.ReflectedType.Name);
                    sb.Append(Environment.NewLine);
                    sb.Append("Method: ");
                    sb.Append(signatureMethod);
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append(typeError.GetDescription());
                    sb.Append(" Message: ");
                    sb.Append(exception.Message);
                    sb.Append(Environment.NewLine);
                    sb.Append("Stack Trace");
                    sb.Append(": ");
                    sb.Append(exception.StackTrace.Replace(" in ", Environment.NewLine + " in "));
                    if (exception.InnerException != null)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append("Inner Exception");
                        sb.Append(": ");
                        sb.Append(exception.InnerException.Message);
                    }
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("Data : ");
                    sb.Append(DateTime.Now.ToString());
                    sb.Append(Environment.NewLine);
                    foreach (char c in assembly.GetName().FullName.ToCharArray())
                        sb.Append("██");
                    sw.WriteLine(sb.ToString());
                    sw.Flush();
                    sw.Close();
                    fs.Close();

                    sb.Clear();

                }


            }
            catch { }


        }



        /// <summary>
        /// Traccia l'eccezione sollevata dal mFFramework nel log
        /// </summary>
        /// <param name="assembly">L'assembly che ha causato l'eccezione</param>
        /// <param name="method">Il metodo nel quale si è verificata l'eccezione</param>
        /// <param name="exception">I dettagli dell'eccezione</param>
        /// <param name="typeError">Il tipo di eccezione: Managed o Unmanaged</param>
        public void Write(Assembly assembly, MethodBase method, Exception exception, TypeError typeError = TypeError.Unmanaged)
        {

            if (!IsEnabled)
                return;

            if (TypeLog == TypeLogger.LogOnDatabase)
                WriteOnDatabase(ref assembly, ref method, ref exception, ref typeError);
            else if (TypeLog == TypeLogger.LogOnFile)
                WriteOnFile(ref assembly, ref method, ref exception, ref typeError);



        }



        /// <summary>
        /// Fornisce la firma completa del metodo che ha provocato l'eccezione
        /// </summary>
        /// <param name="method">Il metodo causante l'eccezione</param>
        private void SetSignatureMethod(MethodBase method)
        {


            parameters = method.GetParameters();

            signatureMethod = method.Name + " (";
            for (int p = 0; p < parameters.Length; p++)
            {

                // aggiunge il nome e il tipo di parametro alla firma del metodo
                signatureMethod += parameters[p];

                // se il parametro ha un valore di default lo aggiunge
                if (parameters[p].RawDefaultValue != DBNull.Value)
                    signatureMethod += " = " + parameters[p].RawDefaultValue;

                // separa i parametri con la virgola
                if (p < parameters.Length - 1)
                    signatureMethod += ", ";

            }
            signatureMethod += ")";



        }




        /// <summary>
        /// Controlla se non esiste già il file di Log, altrimenti lo crea.
        /// </summary>
        private void CheckFileLog()
        {

            string fullNameLog = ParametersLogFile + @"\mFFramework.log";

            if (!File.Exists(fullNameLog))
            {
                fs = new FileStream(fullNameLog, FileMode.Create);
                fs.Close();
            }

            fs = new FileStream(fullNameLog, FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs);

        }








    }




}
