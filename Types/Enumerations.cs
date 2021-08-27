


namespace mFFramework.Types
{

    /// <summary>
    /// 
    /// </summary>
    public enum SqlCommandType
    {

        /// <summary>
        /// 
        /// </summary>
        Text = 0,
        /// <summary>
        /// 
        /// </summary>
        StoredProcedure = 1,
        /// <summary>
        /// 
        /// </summary>
        TableDirect = 2

    }



    /// <summary>
    /// 
    /// </summary>
    public enum HttpContexCache
    {
        /// <summary>
        /// 
        /// </summary>
        [Code("None")]
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        [Code("NoCache")]
        NoCache = 1,
        /// <summary>
        /// 
        /// </summary>
        [Code("Private")]
        Private = 2,
        /// <summary>
        /// 
        /// </summary>
        [Code("Public")]
        Public = 3,
        /// <summary>
        /// 
        /// </summary>
        [Code("Server")]
        Server = 4,
        /// <summary>
        /// 
        /// </summary>
        [Code("ServerAndPrivate")]
        ServerAndPrivate = 5,

        /// <summary>
        /// 
        /// </summary>
        [Code("ServerAndNoCache")]
        ServerAndNoCache = 6



    }



    /// <summary>
    /// Elenco delle tipologie di ordinamento
    /// </summary>
    public enum Order
    {

        /// <summary>
        /// Nessun ordinamento
        /// </summary>
        None = 0,
        /// <summary>
        /// Ordinamento crescente per tutti gli elementi di un lista, in base al testo
        /// </summary>
        AscendentAllForText = 1,
        /// <summary>
        /// Ordinamento decrescente per tutti gli elementi di una lista, in base al testo
        /// </summary>
        DescendentAllForText = 2,
        /// <summary>
        /// Ordinamento crescente per tutti gli elementi di una lista, escluso il primo, in base al testo
        /// </summary>
        AscendentExceptFirstForText = 3,
        /// <summary>
        /// Ordinamento decrescente per tutti gli elementi di una lista, escluso il primo, in base al testo
        /// </summary>
        DescendentExceptFirstForText = 4,
        /// <summary>
        /// Ordinamento crescente per tutti gli elementi di un lista, in base al valore
        /// </summary>
        AscendentAllForValue = 5,
        /// <summary>
        /// Ordinamento decrescente per tutti gli elementi di una lista, in base al valore
        /// </summary>
        DescendentAllForValue = 6,
        /// <summary>
        /// Ordinamento crescente per tutti gli elementi di una lista, escluso il primo, in base al valore
        /// </summary>
        AscendentExceptFirstForValue = 7,
        /// <summary>
        /// Ordinamento decrescente per tutti gli elementi di una lista, escluso il primo, in base al valore
        /// </summary>
        DescendentExceptFirstForValue = 8


    }



    /// <summary>
    /// Elenco delle tipologie di password
    /// </summary>
    public enum Password
    {

        /// <summary>
        /// Password con caratteri alfanumerici
        /// </summary>
        AlphaNumeric = 0,
        /// <summary>
        /// Password con caratteri numerici
        /// </summary>
        Numerical = 1,
        /// <summary>
        /// Password con caratteri letterali
        /// </summary>
        Literal = 3,
        /// <summary>
        /// Password con caratteri letterali e numerici
        /// </summary>
        LiteralAndNumerical = 4




    }



    /// <summary>
    /// Elenco delle codifiche
    /// </summary>
    public enum Encode
    {

        /// <summary>
        /// Codifica
        /// </summary>
        [Code("C")]
        Coding = 1,
        /// <summary>
        /// Decodifica
        /// </summary>
        [Code("D")]
        Decoding = 2

    }



    /// <summary>
    /// Elenco delle scadenze di una richiesta in Cache
    /// </summary>
    public enum CacheExpirable
    {

        /// <summary>
        /// La richiesta in Cache scade
        /// </summary>
        Yes = 0,
        /// <summary>
        /// La richiesta in Cache non scade
        /// </summary>
        No = 1


    }



    /// <summary>
    /// Elenco delgli stati di una richiesta in Cache
    /// </summary>
    public enum CacheContainerState
    {

        /// <summary>
        /// Il contenitore in Cache non è stato aggiunto nè aggiornato
        /// </summary>
        None = 0,
        /// <summary>
        /// Il contenitore in Cache è stato aggiunto (nuovo)
        /// </summary>
        Added = 1,
        /// <summary>
        /// Il contenitore in Cache è stato aggionato
        /// </summary>
        Updated = 2


    }



    /// <summary>
    /// Elenco dei tipi di proprietà di un generico item collegato a un datasource
    /// </summary>
    public enum ItemBoundedProperty
    {

        /// <summary>
        /// La proprietà dell'item è un testo
        /// </summary>
        Text = 1,
        /// <summary>
        /// La proprietà dell'item è un valore
        /// </summary>
        Value = 2

    }



    /// <summary>
    /// Elenco delle tipologie di errore
    /// </summary>
    public enum TypeError
    {


        /// <summary>
        /// 
        /// </summary>
        [Description("Errore di validazione del tracciato")]
        XSD = 1,

        /// <summary>
        /// 
        /// </summary>
        [Description("Error")]
        Error = 2,
        /// <summary>
        /// 
        /// </summary>
        [Description("Warning")]
        Warning = 3,
        /// <summary>
        /// 
        /// </summary>
        [Description("Error managed")]
        Managed = 4,
        /// <summary>
        /// 
        /// </summary>
        [Description("Error not managed")]
        Unmanaged = 5




    }



    /// <summary>
    /// Elenco delle tipologie di log
    /// </summary>
    public enum TypeLogger
    {

        /// <summary>
        /// Nessun log
        /// </summary>
        NoLog = 0,
        /// <summary>
        /// Log su database
        /// </summary>
        LogOnDatabase = 1,
        /// <summary>
        /// Log su file di testo
        /// </summary>
        LogOnFile = 2

    }



    /// <summary>
    /// 
    /// </summary>
    public enum TypeFile
    {

        /// <summary>
        /// 
        /// </summary>
        [Pattern("image/gif")]
        GIF = 1,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("image/jpeg")]
        JPEG = 2,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("image/jpg")]
        JPG = 3,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/pdf")]
        PDF = 4,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("text/plain")]
        TXT = 5,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/ms-word")]
        DOC = 6,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        DOCX = 7,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/ms-excel")]
        XLS = 8,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        XLSX = 9,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        PPTX = 10,
        /// <summary>
        /// 
        /// </summary>
        [Pattern("application/ms-powerpoint")]
        PPT = 11



    }



    /// <summary>
    /// 
    /// </summary>
    public enum TypeSearchCollectionControls
    {
        /// <summary>
        /// 
        /// </summary>
        StartWith = 1
        ,
        /// <summary>
        /// 
        /// </summary>
        EndWith = 2
        ,
        /// <summary>
        /// 
        /// </summary>
        Contains = 3
        ,
        /// <summary>
        /// 
        /// </summary>
        Equal = 4
    }


    /// <summary>
    /// 
    /// </summary>
    public enum TypeParameterCollectionControls
    {
        /// <summary>
        /// 
        /// </summary>
        ID = 1
        ,
        /// <summary>
        /// 
        /// </summary>
        CommandName = 2
        ,
        /// <summary>
        /// 
        /// </summary>
        CommandArgument = 3

    }


    /// <summary>
    /// 
    /// </summary>
    public enum TypeSendMail
    {

        /// <summary>
        /// 
        /// </summary>
        [Description("Impossibile inviare la richiesta: nessuna connessione internet attiva.")]
        NoNetworkConnection = 1,

        /// <summary>
        /// 
        /// </summary>
        [Description("Email inviata correttamente")]
        SendOK = 2,

        /// <summary>
        /// 
        /// </summary>
        [Description("Email inviata correttamente")]
        SendKO = 3,

        /// <summary>
        /// 
        /// </summary>
        [Description("Apertura pagina web con la mail di test")]
        SendTest = 4

    }


    /// <summary>
    /// Tipologia di formato monetario
    /// </summary>
    public enum TypeCurrency
    {

        /// <summary>
        /// 
        /// </summary>
        [Code("it-IT")]
        Euro = 1,

        /// <summary>
        /// 
        /// </summary>
        [Code("en-US")]
        Dollar = 2,

        /// <summary>
        /// 
        /// </summary>
        [Code("pl-PL")]
        Zloti = 3

    }


}
