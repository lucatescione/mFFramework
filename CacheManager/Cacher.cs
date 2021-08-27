using mFFramework.LogManager;
using mFFramework.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.SessionState;



namespace mFFramework.CacheManager
{

    /// <summary>
    /// Gestore della Cache
    /// </summary>
    public class Cacher : ICacher
    {


       
        //private List<CacheContainer> deletingCacheContainers;



        // Istanzia la classe Cacher come Singleton
        //_________________________________________
        /// <summary>
        /// 
        /// </summary>
        private static Cacher istanceCacher;

           // costruttore
        /// <summary>
        /// 
        /// </summary>
           private Cacher()
           { 
               
              CacheContainers = new List<CacheContainer>();           
           } 

        /// <summary>
        /// 
        /// </summary>
           public static Cacher Istance 
            {
                get
                {
                    if (istanceCacher == null)
                        istanceCacher = new Cacher();

                    return istanceCacher;
                }
            }
       //_________________________________________




           private bool deletingYes;
           private int capacity;








        #region ----  PUBLIC  ----



        /// <summary>
        /// Lista dei <see cref="CacheContainer"/> nell'area di Cache 
        /// </summary>
        public List<CacheContainer> CacheContainers { get; set; }



        /// <summary>
        /// Rappresenta il <see cref="CacheContainer"/> delle informazioni e degli oggetti memorizzati, attualmente selezionato 
        /// </summary>
        public CacheContainer SelectedContainer { get; set; }


        private int expiredTime;
        /// <summary>
        /// Tempo di timeout
        /// </summary>
        /// <value></value>
        public int ExpiredTime 
        {
            get
            {
                if (expiredTime == null || expiredTime == 0)
                    return 120; // 2 minuti di default

                return expiredTime;
            }

            set { expiredTime = value; } 

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CacheRequest> GetCacheRequests()
        {

            return Istance.CacheContainers.Select(cc => cc.Request).ToList();
        
        }




        /// <summary>
        /// Recupera un determinato <see cref="CacheContainer"/> dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare il <see cref="CacheContainer"/></param>
        /// <returns>Il <see cref="CacheContainer"/> associato alla richiesta</returns>
        public CacheContainer GetCacheContainer(string nameRequest)
        {
            try
            {
                DateTime now = DateTime.Now;

                SelectedContainer = Istance.CacheContainers.Find(c => c.Request.Name == nameRequest);

                if (SelectedContainer != null
                     &&
                    now.Subtract(SelectedContainer.Request.Date).TotalSeconds > ExpiredTime 
                     && 
                    SelectedContainer.Request.IsExpirable == CacheExpirable.Yes)
                {
                    // se l'oggetto in Cache è espirabile e ha superato il tempo limite, lo si elimina dalla Cache
                    Istance.CacheContainers.Remove(SelectedContainer);
                    //SelectedContainer.Dispose(Istance.CacheContainers);
                    //SelectedContainer.Dispose();
                    SelectedContainer = null;

                }

                // se l'oggetto è presente in Cache, si aggiorna il numero di accessi
                if (SelectedContainer != null)              
                    SelectedContainer.Request.NumberOfAccess++;
               

                return SelectedContainer;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        }


        /// <summary>
        /// Recupera un determinato <see cref="CacheContainer"/>, riferito a una Sessione utente, dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare il <see cref="CacheContainer"/></param>
        /// <param name="Session">Sessione Utente</param>
        /// <returns>Il <see cref="CacheContainer"/> associato alla richiesta di quella Sessione utente</returns>
        public CacheContainer GetCacheContainerWithSessionState(string nameRequest, HttpSessionState Session)
        {
            try
            {
                DateTime now = DateTime.Now;

                SelectedContainer = Istance.CacheContainers.Find(c => c.Request.Name == nameRequest + "|" + Session.SessionID);

                
                // se l'oggetto è presente in Cache, si aggiorna il numero di accessi
                if (SelectedContainer != null)
                    SelectedContainer.Request.NumberOfAccess++;


                return SelectedContainer;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        }


        /// <summary>
        /// Recupera un determinato <see cref="CacheContainer"/>, riferito a una Sessione utente, dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare il <see cref="CacheContainer"/></param>
        /// <param name="SessionId">Id Sessione Utente</param>
        /// <returns>Il <see cref="CacheContainer"/> associato alla richiesta di quella Sessione utente</returns>
        public CacheContainer GetCacheContainerWithSession(string nameRequest, string SessionId)
        {
            try
            {
                DateTime now = DateTime.Now;

                SelectedContainer = Istance.CacheContainers.Find(c => c.Request.Name == nameRequest + "|" + SessionId);


                // se l'oggetto è presente in Cache, si aggiorna il numero di accessi
                if (SelectedContainer != null)
                    SelectedContainer.Request.NumberOfAccess++;


                return SelectedContainer;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        }

        /// <summary>
        /// Recupera una lista di <see cref="CacheContainer"/>, contenente parte della richiesta, dalla Cache
        /// </summary>
        /// <param name="nameRequest">Richiesta per la quale recuperare la lista di <see cref="CacheContainer"/></param>
        /// <returns>la lista di <see cref="CacheContainer"/></returns>
        public List<CacheContainer> GetContainedCacheContainers(string nameRequest)
        {

            try
            {

                DateTime now = DateTime.Now;

                CacheContainers = Istance.CacheContainers.Where(c => c.Request.Name.Contains(nameRequest)).ToList();

                Istance.CacheContainers.RemoveAll(cc => CacheContainers.Contains(cc));
                CacheContainers.ForEach(cc => 
                {

                    if (cc != null
                         &&
                        now.Subtract(cc.Request.Date).TotalSeconds > ExpiredTime
                         &&
                        cc.Request.IsExpirable == CacheExpirable.Yes)
                    {
                        // se l'oggetto in Cache è espirabile e ha superato il tempo limite, lo si elimina dalla Cache
                        //Delete(cc.Request.Name);
                        //cc.Dispose();
                        cc = null;
                    }
                    else
                        cc.Request.NumberOfAccess++;


                
                });
                // si elimina dalla lista gli oggetti nulli (espirati)
                CacheContainers.RemoveAll(cc => cc == null);

                return CacheContainers;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        }





        /// <summary>
        /// Aggiunge alla Cache un determinato <see cref="CacheContainer"/>
        /// </summary>
        /// <param name="entity">Oggetto per il quale verrà creato un <see cref="CacheContainer"/></param>
        /// <param name="nameRequest">Richiesta per la quale aggiungere il nuovo <see cref="CacheContainer"/></param>
        /// <param name="isExpirable">Enumerazione specificante se la Cache è espirabile o meno</param>
        /// <param name="state">Lo stato del <see cref="CacheContainer"/> (aggiunto o aggiornato)</param>
        /// <returns>True, se il <see cref="CacheContainer"/> è stato aggiunto; False altrimenti</returns>
        public bool Add(Object entity, string nameRequest, CacheExpirable isExpirable = CacheExpirable.Yes, CacheContainerState state = CacheContainerState.Added)
        {
            try
            {

                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainer(nameRequest);


                // se non esiste, il contenitore viene aggiunto
                if (SelectedContainer == null)
                {
                    Istance.CacheContainers.Add(new CacheContainer
                    {
                        Entity = entity
                        ,
                        Request = new CacheRequest
                        {
                            Name = nameRequest
                            ,
                            Date = DateTime.Now
                            ,
                            NumberOfAccess = 0
                            ,
                            IsExpirable = isExpirable
                            ,
                            State = state
                        }
                    });

                    return true;
                }

                return false;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error
        }


        /// <summary>
        /// Aggiunge alla Cache un determinato <see cref="CacheContainer"/> al quale è riferito una Sessione utente
        /// </summary>
        /// <param name="entity">Oggetto per il quale verrà creato un <see cref="CacheContainer"/></param>
        /// <param name="nameRequest">Richiesta per la quale aggiungere il nuovo <see cref="CacheContainer"/></param>
        /// <param name="Session">Sessione utente alla quale riferire il<see cref="CacheContainer"/></param>
        /// <param name="state">Lo stato del <see cref="CacheContainer"/> (aggiunto o aggiornato)</param>
        /// <returns>True, se il <see cref="CacheContainer"/> è stato aggiunto; False altrimenti</returns>
        public bool AddWithSessionState(Object entity, string nameRequest, HttpSessionState Session, CacheContainerState state = CacheContainerState.Added)
        {
            try
            {

                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainerWithSessionState(nameRequest, Session);


                // se non esiste, il contenitore viene aggiunto
                if (SelectedContainer == null)
                {
                    Istance.CacheContainers.Add(new CacheContainer
                    {
                        Entity = entity
                        ,
                        Request = new CacheRequest
                        {
                            Name = nameRequest + "|" + Session.SessionID
                            ,
                            Date = DateTime.Now
                            ,
                            NumberOfAccess = 0
                            ,
                            IsExpirable = CacheExpirable.No
                            ,
                            State = state
                        }
                    });

                    return true;
                }

                return false;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error
        }


        /// <summary>
        /// Aggiunge alla Cache un determinato <see cref="CacheContainer"/> al quale è riferito una Sessione utente
        /// </summary>
        /// <param name="entity">Oggetto per il quale verrà creato un <see cref="CacheContainer"/></param>
        /// <param name="nameRequest">Richiesta per la quale aggiungere il nuovo <see cref="CacheContainer"/></param>
        /// <param name="SessionId">Id Sessione utente alla quale riferire il<see cref="CacheContainer"/></param>
        /// <param name="state">Lo stato del <see cref="CacheContainer"/> (aggiunto o aggiornato)</param>
        /// <returns>True, se il <see cref="CacheContainer"/> è stato aggiunto; False altrimenti</returns>
        public bool AddWithSession(Object entity, string nameRequest, string SessionId, CacheContainerState state = CacheContainerState.Added)
        {
            try
            {

                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainerWithSession(nameRequest, SessionId);


                // se non esiste, il contenitore viene aggiunto
                if (SelectedContainer == null)
                {
                    Istance.CacheContainers.Add(new CacheContainer
                    {
                        Entity = entity
                        ,
                        Request = new CacheRequest
                        {
                            Name = nameRequest + "|" + SessionId
                            ,
                            Date = DateTime.Now
                            ,
                            NumberOfAccess = 0
                            ,
                            IsExpirable = CacheExpirable.No
                            ,
                            State = state
                        }
                    });

                    return true;
                }

                return false;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error
        }



        /// <summary>
        /// Aggiorna in Cache un determinato <see cref="CacheContainer"/>, se esiste
        /// </summary>
        /// <param name="entity">Oggetto da aggiornare per una specifica richiesta</param>
        /// <param name="nameRequest">Richiesta per la quale effettuare l'aggiornamento</param>
        /// <returns>True se è stato effettuato l'aggiornamento, False in caso contrario</returns>
        public bool Update(Object entity, string nameRequest)
        {


            try
            {


                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainer(nameRequest);

                if (SelectedContainer != null)
                {
                    // se esiste, il contenitore viene rimosso
                    Istance.CacheContainers.Remove(SelectedContainer);
                    // e ne viene aggiunto uno nuovo, con la stessa richiesta e con l'entità aggiornata
                    Add(entity, nameRequest, SelectedContainer.Request.IsExpirable, CacheContainerState.Updated);

                    return true;
                }


                return false;



            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error


        }
        

        /// <summary>
        /// Aggiorna in Cache un determinato <see cref="CacheContainer"/>, se esiste, al quale è riferito una Sessione utente
        /// </summary>
        /// <param name="entity">Oggetto da aggiornare per una specifica richiesta</param>
        /// <param name="nameRequest">Richiesta per la quale effettuare l'aggiornamento</param>
        /// <param name="Session">Sessione utente alla quale riferire il<see cref="CacheContainer"/></param>
        /// <returns>True se è stato effettuato l'aggiornamento, False in caso contrario</returns>
        public bool UpdateWithSessionState(Object entity, string nameRequest, HttpSessionState Session)
        {


            try
            {


                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainerWithSessionState(nameRequest, Session);


                if (SelectedContainer != null)
                {
                    // se esiste, il contenitore viene rimosso
                    Istance.CacheContainers.Remove(SelectedContainer);

                    // e ne viene aggiunto uno nuovo, con la stessa richiesta e con l'entità aggiornata
                    AddWithSessionState(entity, nameRequest, Session, CacheContainerState.Updated);

                    return true;
                }


                return false;



            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error


        }


        /// <summary>
        /// Aggiunge o Aggiorna in Cache un determinato <see cref="CacheContainer"/>
        /// </summary>
        /// <param name="entity">Oggetto da aggiornare per una specifica richiesta</param>
        /// <param name="nameRequest">Richiesta per la quale effettuare l'aggiornamento</param>
        /// <param name="isExpirable">Indica se la Cache avrà un tempo di scadenza</param>
        /// <returns>Addes, se l'oggeto entity è stato aggiunto; Updated, se l'oggetto entity è stato aggionato</returns>
        public CacheContainerState AddOrUpdate(Object entity, string nameRequest,  CacheExpirable isExpirable = CacheExpirable.Yes)
        {

            try
            {


                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainer(nameRequest);


                if (SelectedContainer != null)
                {

                    // se esiste, il contenitore viene rimosso
                    Istance.CacheContainers.Remove(SelectedContainer);

                    // e ne viene aggiunto uno nuovo, con la stessa richiesta e con l'entità aggiornata
                    Add(entity, nameRequest, SelectedContainer.Request.IsExpirable, CacheContainerState.Updated);

                    return CacheContainerState.Updated;
                }

                // il contenitore viene aggiunto per la prima volta
                Add(entity, nameRequest, isExpirable, CacheContainerState.Added);

                return CacheContainerState.Added;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return CacheContainerState.None;

            }
            #endregion Manage Error



        }



        /// <summary>
        /// Aggiunge o Aggiorna in Cache un determinato <see cref="CacheContainer"/> riferito a una Sessione utente
        /// </summary>
        /// <param name="entity">Oggetto da aggiornare per una specifica richiesta</param>
        /// <param name="nameRequest">Richiesta per la quale effettuare l'aggiornamento</param>
        /// <param name="Session"></param>
        /// <returns>Addes, se l'oggeto entity è stato aggiunto; Updated, se l'oggetto entity è stato aggionato</returns>
        [Obsolete]
        public CacheContainerState AddOrUpdateWithSessionState(Object entity, string nameRequest, HttpSessionState Session)
        {

            try
            {


               // seleziono il Container con la stessa richiesta
               SelectedContainer = GetCacheContainerWithSessionState(nameRequest, Session);


                if (SelectedContainer != null)
                {

                    //se esiste, il contenitore viene rimosso
                    Istance.CacheContainers.Remove(SelectedContainer);

                    //e ne viene aggiunto uno nuovo, con la stessa richiesta e con l'entità aggiornata
                    AddWithSessionState(entity, nameRequest, Session, CacheContainerState.Updated);

                    return CacheContainerState.Updated;
                }

                //il contenitore viene aggiunto per la prima volta
                AddWithSessionState(entity, nameRequest, Session, CacheContainerState.Added);

                return CacheContainerState.Added;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return CacheContainerState.None;

            }
            #endregion Manage Error



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="nameRequest"></param>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public CacheContainerState AddOrUpdateWithSession(Object entity, string nameRequest, string SessionId)
        {

            try
            {


                // seleziono il Container con la stessa richiesta
                SelectedContainer = GetCacheContainerWithSession(nameRequest, SessionId);


                if (SelectedContainer != null)
                {

                    // se esiste, il contenitore viene rimosso
                    Istance.CacheContainers.Remove(SelectedContainer);

                    // e ne viene aggiunto uno nuovo, con la stessa richiesta e con l'entità aggiornata
                    AddWithSession(entity, nameRequest, SessionId, CacheContainerState.Updated);

                    return CacheContainerState.Updated;
                }

                // il contenitore viene aggiunto per la prima volta
                AddWithSession(entity, nameRequest, SessionId, CacheContainerState.Added);

                return CacheContainerState.Added;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return CacheContainerState.None;

            }
            #endregion Manage Error



        }


        /// <summary>
        /// Rimuove dalla Cache un determinato <see cref="CacheContainer"/>
        /// </summary>
        /// <param name="nameRequest">Richiesta da rimuovere</param>
        /// <param name="isContained">False, specifica di cancellare una richiesta ben precisa; True, specifica di cancellare richieste che contengono una richiesta </param>
        /// <returns>True se è stato effettuato la cancellazioe, False in caso contrario</returns>
        public bool Delete(string nameRequest, bool isContained = false)
        {

            deletingYes = false;


            try
            {

                int capacity = Istance.CacheContainers.Count;

                if (!isContained)
                {


                    for (int c = 0; c < capacity; c++)
                    {
                        if (Istance.CacheContainers[c].Request.Name == nameRequest)
                        {
                            // annulla il contenitore di cache
                            Istance.CacheContainers[c] = null;
                            // rimuove il contenitore di cache dalla lista dei contenitori
                            Istance.CacheContainers.RemoveAt(c);
                            // cancellazione avvenuta
                            deletingYes = true;
                            // esco dal ciclo in quanto ho trovato il contenitore da cancellare
                            break;

                        }
                    }

                    
                }
                else
                {

                    for (int c = 0; c < capacity; c++)
                    {
                        if (Istance.CacheContainers[c].Request.Name.Contains(nameRequest))
                        {
                            // annulla il contenitore di cache
                            Istance.CacheContainers[c] = null;
                            // rimuove il contenitore di cache dalla lista dei contenitori
                            Istance.CacheContainers.RemoveAt(c);
                            // riallinea capacità e puntatore di lista
                            capacity--;
                            c--;
                            // cancellazione
                            deletingYes = true;
                          
                        }
                    }

              
                }


                return deletingYes;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error

        }
        

        /// <summary>
        /// Rimuove dalla Cache un elenco di <see cref="CacheContainer"/> riferiti a una Sessione utente
        /// </summary>
        /// <param name="Session">Sessione utente alla quale riferire il<see cref="CacheContainer"/></param>
        /// <param name="nameRequest"></param>
        /// <returns>True se è stato effettuato la cancellazioe, False in caso contrario</returns>
        public bool DeleteWithSessionState(HttpSessionState Session, string nameRequest = null)
        {

            deletingYes = false;

            try
            {

                capacity = Istance.CacheContainers.Count;


                if (string.IsNullOrEmpty(nameRequest))
                {

                    
                    for (int c = 0; c < capacity; c++)
                    {
                        if (Istance.CacheContainers[c].Request.Name.EndsWith("|" + Session.SessionID))
                        {
                            // annulla il contenitore di cache
                            Istance.CacheContainers[c] = null;
                            // rimuove il contenitore di cache dalla lista dei contenitori
                            Istance.CacheContainers.RemoveAt(c);
                            // riallinea capacità e puntatore di lista
                            capacity--;
                            c--;
                            // cancellazione avvenuta
                            deletingYes = true;
                        }
                    }


                  
                }
                else
                {

                     
                     for (int c = 0; c < capacity; c++)
                     {
                         if (Istance.CacheContainers[c].Request.Name == nameRequest + "|" + Session.SessionID)
                         {
                             // annulla il contenitore di cache
                             Istance.CacheContainers[c] = null;
                             // rimuove il contenitore di cache dalla lista dei contenitori
                             Istance.CacheContainers.RemoveAt(c);
                             // cancellazione avvenuta
                             deletingYes = true;
                             // esco dal ciclo in quanto ho trovato il contenitore da cancellare
                             break;
                         
                         }
                     }


                
                }

                return deletingYes;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error


        }

        /// <summary>
        /// Rimuove dalla Cache un elenco di <see cref="CacheContainer"/> riferiti a una Sessione utente
        /// </summary>
        /// <param name="SessionID">ID di Sessione utente alla quale riferire il<see cref="CacheContainer"/></param>
        /// <param name="nameRequest"></param>
        /// <returns>True se è stato effettuato la cancellazioe, False in caso contrario</returns>
        public bool DeleteWithSession(string SessionID, string nameRequest = null)
        {

            deletingYes = false;

            try
            {

                capacity = Istance.CacheContainers.Count;


                if (string.IsNullOrEmpty(nameRequest))
                {


                    for (int c = 0; c < capacity; c++)
                    {
                        if (Istance.CacheContainers[c].Request.Name.EndsWith("|" + SessionID))
                        {
                            // annulla il contenitore di cache
                            Istance.CacheContainers[c] = null;
                            // rimuove il contenitore di cache dalla lista dei contenitori
                            Istance.CacheContainers.RemoveAt(c);
                            // riallinea capacità e puntatore di lista
                            capacity--;
                            c--;
                            // cancellazione avvenuta
                            deletingYes = true;
                        }
                    }



                }
                else
                {


                    for (int c = 0; c < capacity; c++)
                    {
                        if (Istance.CacheContainers[c].Request.Name == nameRequest + "|" + SessionID)
                        {
                            // annulla il contenitore di cache
                            Istance.CacheContainers[c] = null;
                            // rimuove il contenitore di cache dalla lista dei contenitori
                            Istance.CacheContainers.RemoveAt(c);
                            // cancellazione avvenuta
                            deletingYes = true;
                            // esco dal ciclo in quanto ho trovato il contenitore da cancellare
                            break;

                        }
                    }



                }

                return deletingYes;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error


        }


        /// <summary>
        /// Svuota la Cache
        /// <param name="emptyListContainers"></param>
        /// </summary>
        public void Empty(bool emptyListContainers = false)
        {

            capacity = Istance.CacheContainers.Count;

            // annullo tutti i contenitori di cache
            for (int c = 0; c < capacity; c++)
                Istance.CacheContainers[c] = null;
          
            // rimuovo tutti i contenitori dalla lista
            Istance.CacheContainers.Clear();
            // annullo la lista dei contenitori
            if (emptyListContainers)
                Istance.CacheContainers = null;
         
        
        }


        #endregion ----  PUBLIC  ----



       
        /// <summary>
        /// Verifica se la richiesta è già presente in Cacher
        /// </summary>
        /// <param name="nameRequest">Richiesta da verificare</param>
        /// <returns>True, se la richiesta è presente in Cache; False, in caso contrario</returns>
        public bool CheckRequest(string nameRequest) //, DateTime dateRequest)
        {
            
            ///// <param name="dateRequest">Data della richiesta</param>

            return GetCacheContainer(nameRequest) != null;

            //// se la cache è disabilitata (con timeout <= 0) allora ritorna false e non fa nulla
            //if (ExpiredTime <= 0 || ExpiredTime == null)
            //    return false;
            

            //// la cache è abilitata (timeout > 0)
            //// recupero il contenitore con quella nameRequest, se lo trovo
            //SelectedContainer = GetCacheContainer(nameRequest);


            //if (SelectedContainer != null)
            //{

          
            //    // calcolo i secondi trascorsi dall'ultima Request
            //    //timeout = Convert.ToInt32(ConfigurationManager.AppSettings["CacheTimeout"]);
            //    ts = dateRequest.Subtract(SelectedContainer.Request.Date);
            //    seconds = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;


            //    if (seconds <= ExpiredTime || SelectedContainer.Request.IsExpirable == CacheExpirable.No)
            //    {
            //        // la Request può scadere (IsExpirable == Yes):
            //        //   se la richiesta precedente non è ancora scaduta: aggiorno con la nuova data di dateRequest e incremento il number di accessi
            //        // la Request non può scadere:
            //        //   
            //        SelectedContainer.Request.Date = dateRequest;
            //        SelectedContainer.Request.NumberOfAccess++;

            //        return true;
            //    }


            //    // la Request precedente è scaduta: il suo contenitore viene svuotato dalla lista e poi rilasciato
            //    SelectedContainer.Dispose(Istance.CacheContainers);
            //    SelectedContainer = null;

            //    return false;

            //}
            //else   // la Request non è contenuta in nessun contenitore di Cacher
            //    return false;
            

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameRequest"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        public bool CheckRequestWithSessionState(string nameRequest, HttpSessionState Session)
        {
            return GetCacheContainer(nameRequest + "|" + Session.SessionID) != null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameRequest"></param>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public bool CheckRequestWithSession(string nameRequest, string SessionId)
        {
            return GetCacheContainer(nameRequest + "|" + SessionId) != null;

        }
    }


}
