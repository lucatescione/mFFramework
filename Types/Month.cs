
namespace mFFramework.Types
{

    /// <summary>
    /// Definisce il mese nel mFFramework
    /// </summary>
    public class Month
    {

        /// <summary>
        /// Numero del mese
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Nome del mese
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Numero di giorni del mese
        /// </summary>
        public int? NumberOfDays { get; set; }
        /// <summary>
        /// Il nome del primo giorno del mese
        /// </summary>
        public string FirstDay { get; set; }
        /// <summary>
        /// Il nome dell'ultimo giorno del mese
        /// </summary>
        public string LastDay { get; set; }

    }
}
