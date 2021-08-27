using mFFramework.LogManager;
using mFFramework.Types;
using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Reflection;


namespace mFFramework.Repository
{


    /// <summary>
    /// Classe per l'implementazione e l'utilizzo di uno specifico repository, derivato da quello generico.
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        /// Stringa di connessione del modello EF
        /// </summary>
        public static ConnectionStringSettings EntityConnection { get; set; }
        /// <summary>
        /// ObjectContext del modello EF
        /// </summary>
        public static ObjectContext EntitiesContext { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string ServerName{get;set;}

        /// <summary>
        /// 
        /// </summary>
        public static string DatabaseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string UserID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string ProviderName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static string MetaData { get; set; }
  

        private EntityConnection entityConnectionModel;
        private Connection connection;

        private RepositoryFactory()
        {

            try
            {

                if (EntityConnection == null)
                {
                    connection = new Connection(ServerName, DatabaseName, UserID, Password, ProviderName, MetaData);
                    entityConnectionModel = new EntityConnection(connection.EntityConnectionString);
                }
                else
                    entityConnectionModel = new EntityConnection(EntityConnection.ToString());


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);

            }
            #endregion Manage Error

        }

        /// <summary>
        /// 
        /// </summary>
        private static RepositoryFactory instance;

        /// <summary>
        /// Istanza singleton della classe RepositoryFactory
        /// </summary>
        public static RepositoryFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new RepositoryFactory();

                return instance;
            }
        }

      

        /// <summary>
        /// Crea un repository specifico, derivato dal repository generico, di un'entità specifica
        /// </summary>
        /// <typeparam name="DerivateRepository">Repository specifico</typeparam>
        /// <typeparam name="Entity">Entità specifica</typeparam>
        /// <returns></returns>
        public DerivateRepository CreateDerivateRepository<DerivateRepository, Entity>()
            where DerivateRepository : class, new()
            where Entity : EntityObject, new()
        {

            DerivateRepository derivativeRepository = null; 
            ConstructorInfo constructorDerivateRepository = null;

            if (typeof(DerivateRepository).IsSubclassOf(typeof(EntitiesRepository<Entity>)))
            {

                // è un repository derivato da EntitiesRepository

                derivativeRepository = new DerivateRepository();
                constructorDerivateRepository = derivativeRepository.GetType().GetConstructor(new Type[] { typeof(ObjectContext) });
                derivativeRepository = EntitiesContext == null
                                            ? (DerivateRepository)constructorDerivateRepository.Invoke(new object[] { new ObjectContext(entityConnectionModel) })
                                            : (DerivateRepository)constructorDerivateRepository.Invoke(new object[] { EntitiesContext });

                
                return derivativeRepository;

            }

            return default(DerivateRepository);




        }


    }
}
