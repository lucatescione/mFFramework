
namespace mFFramework.Validations.XSD
{

    /// <summary>
    /// Definisce l'errore di una validazione XSD
    /// </summary>
    public class XSDError
    {

        /// <summary>
        /// Codice di errore della validazione
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Descrizione dell'errore
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Linea del file xml alla quale è stato individuato l'errore
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// Tag del file xml sul quale è stato individuato l'errore
        /// </summary>
        public string TagXML { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Validation { get; set; }
    }
}
