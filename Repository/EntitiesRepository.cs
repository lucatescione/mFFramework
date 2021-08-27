using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using mFFramework.LogManager;
using mFFramework.Types;
using mFFramework.Utilities;

namespace mFFramework.Repository
{




    /// <summary>
    /// Classe astratta per un repository generico di entità generiche
    /// </summary>
    /// <typeparam name="Entity">Entità generica</typeparam>
    public abstract class EntitiesRepository<Entity> : IEntitiesRepository<Entity> 
        where Entity : EntityObject, new()
    {

        private PropertyInfo property;
        private List<NavigationProperty> navigationProperties;

        private Type entityType;
        private EntityContainer entitiesContainer;


        private Entity entity;

        /// <summary>
        /// Elenco delle proprietà di navigazione (1° livello) dell'entità
        /// </summary>
        protected List<PropertyInfo> NavigationProperties { get; set; }
       

        /// <summary>
        /// 
        /// </summary>
        protected  ObjectContext entitiesContext;

      

        /// <summary>
        /// 
        /// </summary>
        protected ObjectSet<Entity> entitiesSet;

      
        



        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesContext"></param>
        public EntitiesRepository(ObjectContext entitiesContext)
        {

            this.entitiesContext = entitiesContext;

            entitiesSet = entitiesContext.CreateObjectSet<Entity>();

            GetNavigationProperties();
           
        }




        /// <summary>
        /// 
        /// </summary>
        public EntitiesRepository()
        {



        }


        




        /// <summary>
        /// Crea un'espressione di ordinamento a partire dal nome del campo
        /// </summary>
        /// <param name="nameParameter">il name del campo da ordinare</param>
        /// <returns></returns>
        public Expression<Func<Entity, object>> GetSortingExpression(string nameParameter)
        {
            
            ParameterExpression x = Expression.Parameter(typeof(Entity), "x");
            Expression property = Expression.PropertyOrField(x, nameParameter);
            
            return Expression.Lambda<Func<Entity, object>>(Expression.Convert(property, typeof(object)), x);
        }


        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IQueryable<Entity> All()
        {


            //return entitiesContext.CreateObjectSet<Entity>().AsQueryable();
            return entitiesSet.AsQueryable();

        }

        private ObjectSet<Entity> GetAll()
        {

            return entitiesSet;

        }


        /// <summary>
        /// Seleziona un collezione di Entità in base all'espressione Lambda specificata
        /// </summary>
        /// <param name="expression">Espressione Lambda da valutare</param>
        /// <param name="includeEntitiesNames">Elenco dei nomi delle entità da includere (1° livello)</param>
        /// <returns>Un collezione queryable di Entità</returns>
        public IQueryable<Entity> FilterBy(Expression<Func<Entity, bool>> expression, string[] includeEntitiesNames = null)
        {

            ObjectQuery<Entity> query = entitiesSet; 

            // includo le proprietà di navigazione, se sono presenti in quelle della entità considerata.
            if (!includeEntitiesNames.IsVoid())
            {
                foreach (string name in includeEntitiesNames)
                {
                    // recupero la proprietà di navigazione e la includo
                    property = NavigationProperties.Find(np => np.Name == name);
                    if (property != null)
                        query = query.Include(property.Name);
                }


            }

            // eseguo il filtro sulle entità
            return query.Where(expression);


        }

    

     
        /// <summary>
        /// Seleziona una collezione di Entità in base all'espressione lambda specificata, successivamente paginata
        /// </summary>
        /// <param name="expression">Espressione lambda da valutare</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<Entity> FilterByWithPaging(Expression<Func<Entity, bool>> expression, int pageNumber, int pageSize)
        {

            return All().Where(expression).Skip(pageNumber * pageSize).Take(pageSize).AsQueryable();

        }




        /// <summary>
        /// Seleziona una collezione di Entità in base all'espressione lambda specificata, successivamente ordinata
        /// </summary>
        /// <param name="expression">Espressione lambda da valutare</param>
        /// <param name="sortBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IQueryable<Entity> FilterByWithSorting(Expression<Func<Entity, bool>> expression, Expression<Func<Entity, object>> sortBy, Order order = Order.AscendentAllForText)
        {

            switch (order)
            { 
            
                default:
                case Order.AscendentAllForText:
                case Order.AscendentExceptFirstForText:
                case Order.AscendentAllForValue:
                case Order.AscendentExceptFirstForValue:
                    return All().Where(expression).OrderBy(sortBy).AsQueryable();

                case Order.DescendentAllForText:
                case Order.DescendentExceptFirstForText:
                case Order.DescendentAllForValue:
                case Order.DescendentExceptFirstForValue:
                    return All().Where(expression).OrderByDescending(sortBy).AsQueryable();
            
            
            }


            
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IQueryable<Entity> FilterByWithPagingAndSorting(Expression<Func<Entity, bool>> expression, Expression<Func<Entity, object>> sortBy, int pageNumber, int pageSize, Order order = Order.AscendentAllForText)
        {

            switch (order)
            {

                default:
                case Order.AscendentAllForText:
                case Order.AscendentExceptFirstForText:
                case Order.AscendentAllForValue:
                case Order.AscendentExceptFirstForValue:
                    return All().Where(expression).OrderBy(sortBy).Skip(pageNumber * pageSize).Take(pageSize);



                case Order.DescendentAllForText:
                case Order.DescendentExceptFirstForText:
                case Order.DescendentAllForValue:
                case Order.DescendentExceptFirstForValue:
                    return All().Where(expression).OrderByDescending(sortBy).Skip((pageNumber) * pageSize).Take(pageSize);

            }


            
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Entity FindBy(Expression<Func<Entity, bool>> expression)
        {
            
            return FilterBy(expression).SingleOrDefault();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<Entity, bool>> expression)
        {
            return All().Where(expression).Count();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string GetEntitySetName(Entity entity)
        {

            entityType = entity.GetType();

            entitiesContainer = entitiesContext.MetadataWorkspace.GetEntityContainer(entitiesContext.DefaultContainerName, DataSpace.CSpace);

            return (from entitySet in entitiesContainer.BaseEntitySets
                    where entitySet.ElementType.Name.Equals(entityType.Name)
                    select entitySet.Name).Single();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Entity entity)
        {
           
            entitiesContext.AddObject(GetEntitySetName(entity), entity);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Add(List<Entity> entities)
        {
            entities.ForEach(entity => entitiesContext.AddObject(GetEntitySetName(entity), entity));

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public bool Delete(Entity entity)
        {
            try
            {
                entitiesContext.DeleteObject(entity);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public bool Delete(List<Entity> entities)
        {
            try
            {
                entities.ForEach(entity => entitiesContext.DeleteObject(entity));

                return true;
            }
            catch(Exception ex)
            {
                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void SaveChanges()
        {

            entitiesContext.SaveChanges();
        
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void SaveChanges(SaveOptions options)
        {

            entitiesContext.SaveChanges(options);

        }

        /// <summary>
        /// 
        /// </summary>
        public void Update() //, Entity modifiedEntity)
        {
            


            //Set non-nav props
            //.CurrentValues.SetValues(modifiedEntity);

            //Set nav props
            //var navProps = GetNavigationProperties(originalEntity);
            //foreach (var navProp in navProps)
            //{
            //    //Set originalEntity prop value to modifiedEntity value
            //    navProp.SetValue(originalEntity, navProp.GetValue(modifiedEntity));
            //}

        
        }


        /// <summary>
        /// 
        /// </summary>
        private void GetNavigationProperties()
        {

           

            // recupero le proprietà di navigazione dell'entità
            navigationProperties = entitiesSet.EntitySet.ElementType.NavigationProperties
                                              .ToList();


            //entitiesContainer = entitiesContext.MetadataWorkspace.GetEntityContainer(entitiesContext.DefaultContainerName, DataSpace.CSpace);


            // memorizzo queste proprie di navigazione nel repository
            entity = new Entity();
            entityType = entity.GetType();
            NavigationProperties = new List<PropertyInfo>();
            navigationProperties.ForEach(navigationProperty => 
            {

                //EntitySet es = ((EntitySet)entitiesContainer.BaseEntitySets.Where(e => e.Name == navigationProperty.Name).First());
                //entitiesContext.g
                   
                 //(from entitySet in entitiesContainer.GetEntitySetByName(navigationProperty.Name,false)
                 //       where entitySet.ElementType.Name.Equals(navigationProperty.Name)
                 // select entitySet).

                NavigationProperties.Add(entityType.GetProperty(navigationProperty.Name));
            });

           
        }


        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //    //throw new NotImplementedException();
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (entitiesContext != null)
        //        {
        //            entitiesContext.Dispose();
        //            entitiesContext = null;
        //        }
        //    }
        //}

        //~EntitiesRepository()
        //{
        //    //Dispose(true);
        //}

    }




}


