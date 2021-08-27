using System;
using System.Collections.Generic;
using mFFramework.Types;



namespace mFFramework.CacheManager
{

    /// <summary>
    /// Contenitore nella Cache
    /// </summary>
    public class CacheContainer : IDisposable
    {
        
        
        #region ----  PUBLIC  ----


        /// <summary>
        /// Oggetto generico memorizzato nel contenitore
        /// </summary>
        public Object Entity  { get; set; }

        /// <summary>
        /// Il tipo di oggetto memorizzato nel contenitore
        /// </summary>
        public Type EntityType { get { return Entity.GetType(); } }

        /// <summary>
        /// La richiesta di Cache <see cref="CacheRequest"/>
        /// </summary>
        public CacheRequest Request { get; set; }



        /// <summary>
        /// Rilascia le risorse di una lista di contenitori
        /// </summary>
        /// <param name="containers">La lista di contenitori</param>
        public void Dispose(List<CacheContainer> containers)
        {

            this.containers = containers;

            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }



        #endregion ----  PUBLIC  ----



        #region ----  PRIVATE  ----

        /// <summary>
        /// 
        /// </summary>
        private List<CacheContainer> containers;
        /// <summary>
        /// 
        /// </summary>
        private bool disposed = false;


        #endregion ----  PRIVATE  ----


        
        /// <summary>
        /// Rilascia le risorse del contenitore
        /// </summary>
        public void Dispose()
        {

            //Implement IDisposable.
            //Do not make this method virtual.
            //A derived class should not be able to override this method.



            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


       


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {

            //// Dispose(bool disposing) executes in two distinct scenarios.
            //// If disposing equals true, the method has been called directly
            //// or indirectly by a user's code. Managed and unmanaged resources
            //// can be disposed.
            //// If disposing equals false, the method has been called by the
            //// runtime from inside the finalizer and you should not reference
            //// other objects. Only unmanaged resources can be disposed.
          
            //lock (obj)
            //{
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this.containers.Remove(this);
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                
                // Note disposing has been done.
                disposed = true;

            }

            //} // blocco lock

        }

       
        /// <summary>
        /// Distruttore del contenitore
        /// </summary>
        ~CacheContainer()
        {

            // Use C# destructor syntax for finalization code.
            // This destructor will run only if the Dispose method
            // does not get called.
            // It gives your base class the opportunity to finalize.
            // Do not provide destructors in types derived from this class.


            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }


    }


    /// <summary>
    /// Richiesta di Cache
    /// </summary>
    public class CacheRequest
    {

        
        /// <summary>
        /// Il name univoco che identifica la richiesta
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// La Data della richiesta
        /// </summary>
        /// <value></value>
        public DateTime Date { get; set; }
        /// <summary>
        /// Il numero di accessi in Cache, effettuati per questa richiesta
        /// </summary>
        /// <value>Il numero di accessi</value>
        public int NumberOfAccess { get; set; }
        /// <summary>
        /// Indica se una richiesta, in Cache, può scadere
        /// </summary>
        /// <value></value>
        public CacheExpirable IsExpirable { get; set; }
        /// <summary>
        /// Indica se una richiesta, in Cache, è stata aggiunta o aggiornata
        /// </summary>
        /// <value>True, se la richiesta è scaduta; false, altrimenti</value>
        public CacheContainerState State { get; set; }
    
    }



}
