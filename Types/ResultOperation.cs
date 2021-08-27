using System.Linq;
using System.Collections.Generic;



namespace mFFramework.Types
{

    /// <summary>
    /// Definisce la struttura contenente l'esito di una generica operazione
    /// </summary>
    interface IOutcome
    {

        /// <summary>
        /// Il numero dell'errore
        /// </summary>
        int NumberError { get; set; }
        /// <summary>
        /// Il messaggio dell'errore
        /// </summary>
        string MessageError { get; set; }
        /// <summary>
        /// La linea di codice dove si è verificato l'errore
        /// </summary>
        int? LineError { get; set; }
        /// <summary>
        /// True se c'è un errore, False altrimenti
        /// </summary>
        bool HasError { get; }

    
    }



    
    


    /// <summary>
    /// Contiene le informazioni relative all'esito di una StoredProcedure (non di Select)
    /// </summary> 
    public class OutcomeStoredProcedure : IOutcome
    {

        /// <summary>
        /// Il numero T-SQL dell'errore
        /// </summary>
        public int NumberError { get; set; }
        /// <summary>
        /// Il messaggio T-SQL dell'errore
        /// </summary>
        public string MessageError { get; set; }
        /// <summary>
        /// La linea di codice T-SQL dove si è verificato l'errore
        /// </summary>
        public int? LineError { get; set; }
        /// <summary>
        /// Parametri di output della query/storedprocedure, separati dal carattere speciale |
        /// </summary>
        public string PipelineOutParameters { get; set; }
        /// <summary>
        /// True se c'è un errore, False altrimenti
        /// </summary>
        public bool HasError
        {

            get
            {
                return NumberError < 0;
            }

        
        }

        /// <summary>
        /// Restituisce la lista di parametri di output della query/storedprocedure
        /// </summary>
        public List<string> ListOutParameters
        {

            get
            {
               
                return !string.IsNullOrEmpty(PipelineOutParameters)
                            ? PipelineOutParameters.Split('|').AsQueryable()
                                                              .Where(s => !string.IsNullOrEmpty(s))
                                                              .ToList()
                            : null;

            }

        }


        /// <summary>
        /// Restituisce la lista dei messaggi T-SQL dell'errore
        /// </summary>
        public List<string> MessageErrors
        {

            get
            {

                return !string.IsNullOrEmpty(MessageError)
                            ? MessageError.Split('|').AsQueryable()
                                                              .Where(s => !string.IsNullOrEmpty(s))
                                                              .ToList()
                            : null;

            }

        }
       
    }


}
