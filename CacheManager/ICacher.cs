using mFFramework.Types;
using System;
using System.Collections.Generic;

namespace mFFramework.CacheManager
{
    /// <summary>
    /// Metodi per la gestione della Cache
    /// </summary>
    interface ICacher
    {

        /// <summary>
        /// Recupera un determinato <see cref="CacheContainer"/> dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare il <see cref="CacheContainer"/></param>
        /// <returns>Il <see cref="CacheContainer"/> associato alla richiesta</returns>
        CacheContainer GetCacheContainer(string nameRequest);


        /// <summary>
        /// Recupera una lista di <see cref="CacheContainer"/>, contenente parte della richiesta, dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare la lista di <see cref="CacheContainer"/></param>
        /// <returns>la lista di <see cref="CacheContainer"/></returns>
        List<CacheContainer> GetContainedCacheContainers(string nameRequest);


        /// <summary>
        /// Aggiunge alla Cache un determinato <see cref="CacheContainer"/>
        /// </summary>
        /// <param name="entity">Oggetto per il quale verrà creato un <see cref="CacheContainer"/></param>
        /// <param name="nameRequest">Richiesta per la quale aggiungere il nuovo <see cref="CacheContainer"/></param>
        /// <param name="isExpirable">Enumerazione specificante se la Cache è espirabile o meno</param>
        /// <param name="state">Lo stato del <see cref="CacheContainer"/> (aggiunto o aggiornato)</param>
        /// <returns>True, se il <see cref="CacheContainer"/> è stato aggiunto; False altrimenti</returns>
        bool Add(Object entity, string nameRequest, CacheExpirable isExpirable, CacheContainerState state);


        /// <summary>
        /// Aggiorna in Cache un determinato <see cref="CacheContainer"/>, se esiste
        /// </summary>
        /// <param name="entity">Oggetto da aggiornare per una specifica richiesta</param>
        /// <param name="nameRequest">Richiesta per la quale effettuare l'aggiornamento</param>
        /// <returns>True se è stato effettuato l'aggiornamento, False in caso contrario</returns>
        bool Update(Object entity, string nameRequest);


        /// <summary>
        /// Rimuove dalla Cache un determinato <see cref="CacheContainer"/>
        /// </summary>
        /// <param name="nameRequest">Richiesta da rimuovere</param>
        /// <param name="isContained">False, specifica di cancellare una richiesta ben precisa; True, specifica di cancellare richieste che contengono una richiesta </param>
        /// <returns>True se è stato effettuato la cancellazioe, False in caso contrario</returns>
        bool Delete(string nameRequest, bool isContained);



        /// <summary>
        /// Verifica se la richiesta è già presente in Cacher
        /// </summary>
        /// <param name="nameRequest">Richiesta da verificare</param>
        /// <returns>True, se la richiesta è presente in Cache; False, in caso contrario</returns>
        bool CheckRequest(string nameRequest); 

        
    
    }
}
