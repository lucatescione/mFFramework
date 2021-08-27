

namespace mFFramework.Types
{

    /// <summary>
    /// Definisce l'elemento di una generica enumerazione
    /// </summary>
    public class ItemEnumeration
    {

        /// <summary>
        /// Il valore ordinale dell'elemento
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// Il nome dell'elemento
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Il codice dell'elemento (se assente = null)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// La descrizione dell'elemento (se assente = null)
        /// </summary>
        public string Description { get; set; }
    }
}
