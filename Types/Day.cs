
namespace mFFramework.Types
{

    /// <summary>
    /// Definisce il giorno nel mFFramework
    /// </summary>
    public class Day
    {

        /// <summary>
        /// Numero del giorno nella settimana
        /// </summary>
        public int OrdinalInWeek { get; set; }
        /// <summary>
        /// Numero del giorno nel mese
        /// </summary>
        public int OrdinalInMonth { get; set; }
        /// <summary>
        /// Numero del giorno nell'anno
        /// </summary>
        public int OrdinalInYear { get; set; }
        /// <summary>
        /// Nome del giorno
        /// </summary>
        public string Name { get; set; }


    }
}
