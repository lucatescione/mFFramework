using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using mFFramework.Types;


namespace mFFramework.Repository
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public interface IEntitiesRepository<Entity> 
        where Entity : EntityObject
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        IQueryable<Entity> FilterBy(Expression<Func<Entity, bool>> expression, string[] names = null);
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IQueryable<Entity> FilterByWithPaging(Expression<Func<Entity, bool>> expression, int pageNumber, int pageSize);
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sortBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IQueryable<Entity> FilterByWithSorting(Expression<Func<Entity, bool>> expression, Expression<Func<Entity, object>> sortBy, Order order = Order.AscendentAllForText);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IQueryable<Entity> FilterByWithPagingAndSorting(Expression<Func<Entity, bool>> expression, Expression<Func<Entity, object>> sortBy, int pageNumber, int pageSize, Order order = Order.AscendentAllForText);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Entity FindBy(Expression<Func<Entity, bool>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        int Count(Expression<Func<Entity, bool>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Add(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Add(List<Entity> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete(Entity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Delete(List<Entity> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameParameter"></param>
        /// <returns></returns>
        Expression<Func<Entity, object>> GetSortingExpression(string nameParameter);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        string GetEntitySetName(Entity entity);


    }
}
