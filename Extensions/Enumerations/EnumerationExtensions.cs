using System;
using System.Collections.Generic;
using System.Reflection;
using mFFramework.LogManager;
using mFFramework.Types;



namespace mFFramework.Extensions.Enumerations
{


    /// <summary>
    /// Metodi Estesi per Enumerazioni
    /// </summary>
    public static class EnumerationExtensions
    {

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="order"></param>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //public static Order Convert(this Order order, GridViewSortEventArgs e)
        //{

        //    if (e.SortDirection == SortDirection.Ascending)
        //        return Order.AscendentAllForText;
        //    else if (e.SortDirection == SortDirection.Descending)
        //        return Order.DescendentAllForText;


        //    return Order.None;

        //}

        #region ----   PUBLIC  ----


        /// <summary>
        /// Recupera uno dei valori di Enumerazione, dall'attributo Code
        /// </summary>
        /// <typeparam name="TypeEnumeration">La tipologia di enumerazione</typeparam>
        /// <param name="typeEnumeration">L'enumerazione dalla quale recuperare il valore</param>
        /// <param name="value">Il valore dell'attributo Code</param>
        /// <returns>Il valore dell'enumerazione corrispondente all'attributo Code</returns>
        public static TypeEnumeration? GetValueByCode<TypeEnumeration>(this TypeEnumeration typeEnumeration, string value)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {
            try
            {

                bool trovato = false;
                object obj = null;
                TypeEnumeration findValueEnumeration = default(TypeEnumeration);
                Enum findEnum = null;



                if (!typeEnumeration.GetType().IsEnum)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typeEnumeration is not Enum"), TypeError.Managed);
                    return findValueEnumeration;
                }


                TypeEnumeration[] valuesEnumeration = (TypeEnumeration[])typeEnumeration.GetType().GetEnumValues();

                foreach (TypeEnumeration valueEnumeration in valuesEnumeration)
                {

                    obj = Enum.Parse(typeof(TypeEnumeration), valueEnumeration.ToString(), true);
                    findEnum = (Enum)obj;

                    if (findEnum.GetCode().ToLower() == value.ToLower())
                    {

                        //findValueEnumeration = (TypeEnumeration)obj;
                        trovato = true;
                        break;
                    }

                }


                return trovato ? (TypeEnumeration)obj : (TypeEnumeration?)null;


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeEnumeration?);

            }
            #endregion Manage Error

        }



        /// <summary>
        /// Recupera uno dei valori di Enumerazione, dall'attributo Description
        /// </summary>
        /// <typeparam name="TypeEnumeration">La tipologia di enumerazione</typeparam>
        /// <param name="typeEnumeration">L'enumerazione dalla quale recuperare il valore</param>
        /// <param name="value">Il valore dell'attributo Code</param>
        /// <returns>Il valore dell'enumerazione corrispondente all'attributo Description</returns>
        public static TypeEnumeration? GetValueByDescription<TypeEnumeration>(this TypeEnumeration typeEnumeration, string value)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {

            try
            {

                bool trovato = false;
                object obj = null;
                TypeEnumeration findValueEnumeration = default(TypeEnumeration);
                Enum findEnum = null;



                if (!typeEnumeration.GetType().IsEnum)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typeEnumeration is not Enum"), TypeError.Managed);
                    return findValueEnumeration;
                }


                TypeEnumeration[] valuesEnumeration = (TypeEnumeration[])typeEnumeration.GetType().GetEnumValues();

                foreach (TypeEnumeration valueEnumeration in valuesEnumeration)
                {

                    obj = Enum.Parse(typeof(TypeEnumeration), valueEnumeration.ToString(), true);
                    findEnum = (Enum)obj;

                    if (findEnum.GetDescription().ToLower() == value.ToLower())
                    {

                        //findValueEnumeration = (TypeEnumeration)obj;
                        trovato = true;
                        break;
                    }

                }


                return trovato ? (TypeEnumeration)obj : (TypeEnumeration?)null;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeEnumeration?);

            }
            #endregion Manage Error

        }



        /// <summary>
        /// Recupera gli elementi di una enumerazione
        /// </summary>
        /// <typeparam name="TypeEnumeration">La tipologia di enumerazione</typeparam>
        /// <param name="typeEnumeration">L'enumerazione da elencare</param>
        /// <returns>La lista di ItemEnumeration dell'enumerazione</returns>
        public static List<ItemEnumeration> GetList<TypeEnumeration>(this TypeEnumeration typeEnumeration)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {

            try
            {

                Enum findEnum = null;

                // se non è un'enumerazione
                if (!typeEnumeration.GetType().IsEnum)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typeEnumeration is not Enum"), TypeError.Managed);
                    return default(List<ItemEnumeration>);
                }


                // recupera i valori dell'enumerazione
                TypeEnumeration[] valuesEnumeration = (TypeEnumeration[])typeEnumeration.GetType().GetEnumValues();


                // riempie la lista di oggetti ItemEnumeration, per l'enumerazione corrente
                List<ItemEnumeration> itemsEnumeration = new List<ItemEnumeration>();
                for (int i = 0; i < valuesEnumeration.Length; i++)
                {

                    findEnum = (Enum)Enum.Parse(typeof(TypeEnumeration), valuesEnumeration[i].ToString(), true);

                    findEnum.GetCode();
                    itemsEnumeration.Add(new ItemEnumeration
                    {
                        Name = valuesEnumeration[i].ToString()
                        ,
                        Ordinal = System.Convert.ToInt32(valuesEnumeration[i])
                        ,
                        Code = findEnum.GetCode()
                        ,
                        Description = findEnum.GetDescription()


                    });


                }


                return itemsEnumeration;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<ItemEnumeration>);

            }
            #endregion Manage Error

        }



        #endregion ----   PUBLIC  ----


    }


}
