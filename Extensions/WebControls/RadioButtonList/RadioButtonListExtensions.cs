using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using mFFramework.Conversions;
using mFFramework.LogManager;
using mFFramework.Types;
using mFFramework.Utilities;




namespace mFFramework.Extensions.WebControls
{


    /// <summary>
    /// Metodi Estesi per il WebControl radioButtonList
    /// </summary>
    public static class RadioButtonListExtensions
    {

       

        /// <summary>
        /// Carica gli elementi in una DropoDownList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList da caricare</param>
        /// <param name="datasource">La lista di elementi da caricare nella RadioButtonList</param>
        /// <param name="propertyText">Name della proprietà di testo i cui valori appariranno nell'elenco della RadioButtonList</param>
        /// <param name="propertyValue">Name della proprietà di value i cui valori saranno associati all'etichetta di testo</param>
        /// <param name="order">L'ordine da associare alla lista</param>
        /// <param name="defaultItem">Elemento di default</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare anche gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int Bind<TypeSource>(this RadioButtonList rbl, List<TypeSource> datasource, string propertyText, string propertyValue, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        {

            try
            {

                TypeSource[] items = null;


                // se il datasource non esiste (vuoto o non istanziato)
                if (datasource.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("datasource == null or datasource.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // se il datasource ha almeno un oggetto che non ha la proprietà propertyText
                if (datasource.Find(o => o.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null) != null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyText == null"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // se il datasource ha almeno un oggetto che non ha la proprietà propertyValue
                if (datasource.Find(o => o.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null) != null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyValue == null"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // se necessario, rimuovo gli items vuoti
                items = (thereAreItemsEmpty) ? datasource.ToArray() : datasource.Where(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() != string.Empty).ToArray();


                // ordino gli item
                items = Functions.OrderList<TypeSource>(items, order, propertyText, propertyValue);



                // associo finalmente il datasource
                rbl.DataSource = items;
                rbl.DataTextField = propertyText;
                rbl.DataValueField = propertyValue;
                rbl.DataBind();


                // se è necessario, inserisco l'item di default al primo posto
                if (!string.IsNullOrEmpty(defaultItem))
                    rbl.Items.Insert(0, Functions.AddDefaultItem(defaultItem));


                return items.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Recupera la lista di elementi contenuti in una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare gli elementi</param>
        /// <param name="index">Indice al quale recuperare un elemento specifico (opzionale)</param>
        /// <returns>La lista di elementi contenuti nella RadioButtonList</returns>
        public static List<TypeSource> GetList<TypeSource>(this RadioButtonList rbl, int index = 0)
        where TypeSource : new()
        {

            try
            {


                //  recupero tutti gli item presenti nella RadioButtonList o, se specificato l'indice, quello corrispondente alla posizione
                List<ListItem> items = new List<ListItem>();
                if (index <= 0)
                    items = rbl.Items.OfType<ListItem>().ToList().ExcludeDefaultItem();
                else
                    items.Add(rbl.Items[index]);


                //  recupero le proprietà corrispondenti all'propertyText e al propertyValue della RadioButtonList
                TypeSource typesource = new TypeSource();
                PropertyInfo propertyText = typesource.GetType().GetProperty(rbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                PropertyInfo propertyValue = typesource.GetType().GetProperty(rbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);


                if (propertyText == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyText == null"), TypeError.Managed);
                    return default(List<TypeSource>);
                }


                if (propertyValue == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyValue == null"), TypeError.Managed);
                    return default(List<TypeSource>);
                }


                // converto gli item in typesource
                List<TypeSource> typesources = new List<TypeSource>();
                items.ForEach(item =>
                {

                    typesource = new TypeSource();
                    propertyText.SetValue(typesource, item.Text, null);


                    switch (propertyValue.PropertyType.Name.ToLower())
                    {

                        case "int32":
                            propertyValue.SetValue(typesource, item.Value.ToInt(), null);
                            break;


                        case "int64":
                            propertyValue.SetValue(typesource, item.Value.ToLong(), null);
                            break;


                        default:
                        case "string":
                            propertyValue.SetValue(typesource, item.Value, null);
                            break;

                    }


                    typesources.Add(typesource);

                });


                return typesources;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<TypeSource>);

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Aggiunge un nuovo elemento agli elementi presenti in una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList alla quale aggiungere il nuovo elemento</param>
        /// <param name="newItem">L'elemento da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int Add<TypeSource>(this RadioButtonList rbl, TypeSource newItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try{



                // logger se il newItem è nullo
                if (newItem == null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("newItem == null"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;
                
                }


                // aggiungo quello nuovo
                typesources.Add(newItem);


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Aggiunge nuovi elementi agli elementi presenti in una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList alla quale aggiungere il nuovo elemento</param>
        /// <param name="newItems">La lista di elementi da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int Add<TypeSource>(this RadioButtonList rbl, List<TypeSource> newItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {



                // logger se il newItem è nullo
                if (newItems.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("newItems == null || newItems.Count() == 0"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // aggiungo quelli nuovi
                newItems.ForEach(newItem => { if (newItem != null) typesources.Add(newItem); });


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }
        


        
        /// <summary>
        /// Inserisce un nuovo elemento in una determinata posizione, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList in cui inserire l'elemento</param>
        /// <param name="newItem">L'elemento da inserire</param>
        /// <param name="index">La posizione alla quale inserire l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int InsertAt<TypeSource>(this RadioButtonList rbl, TypeSource newItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // logger se newItem è nullo
                if (newItem == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("newItem == null"), TypeError.Managed);
                    return Int32.MinValue;

                }

                
                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                if (index >= typesources.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // inserisco quello nuovo
                typesources.Insert(index, newItem);


                //if (index > 0 && (order == Order.DescendentExceptFirst
                //                  || order == Order.DescendentAll
                //                  || order == Order.AscendentExceptFirst
                //                  || order == Order.AscendentAll))
                if (index > 0)
                    order = Order.None;


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error


        }




        /// <summary>
        /// Inserisce nuovi elementi in determinata posizioni, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList in cui inserire l'elemento</param>
        /// <param name="newItems">La lista di elementi da inserire in determinate posizioni</param>
        /// <param name="indexs">La lista di indici ai quali inserire gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int InsertAt<TypeSource>(this RadioButtonList rbl, List<TypeSource> newItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // logger se la lista è nulla o vuota
                if (newItems.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("newItems == null || newItems.Count() == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se la lista è nulla o vuota
                if (indexs.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs == null || indexs.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                //  logger se le liste non hanno lo stesso number di elementi
                //if (indexs.Count != newItems.Count)
                //{

                //    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count != newItems.Count");
                //    return Int32.MinValue;
                
                //}


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                if (indexs.Count >= typesources.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs.Count >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // inserisco gli elementi nuovi
                int contItem = -1;
                indexs.ForEach(index =>
                {

                    contItem++;
                    if (index >= 0  // l'indice deve essere >= 0 
                         &&
                         newItems[contItem] != null  // l'item da inserire non è nullo 
                         &&
                         index < typesources.Count) // l'indice non è oltre il limite della lista
                        typesources.Insert(index, newItems[contItem]);

                });
                


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error


        }
        



        /// <summary>
        /// Rimuove un elemento, in una determinata posizione, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale rimuovere l'elemento</param>
        /// <param name="index">La posizione alla quale rimuovere l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int RemoveAt<TypeSource>(this RadioButtonList rbl, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = rbl.GetList<TypeSource>();

                
                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se indice oltre i limiti della matrice
                if (index >= typesources.Count)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(),new Exception("index >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // rimuovo l'oggetto in posizione
                typesources.RemoveAt(index);


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Rimuove gli elementi, in determinate posizioni, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale rimuovere l'elemento</param>
        /// <param name="indexs">La lista di indici ai quali rimuovere gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int RemoveAt<TypeSource>(this RadioButtonList rbl, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se indice oltre i limiti della matrice
                //if (indexs.Count >= typesources.Count)
                //{
                //    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
                //    return Int32.MinValue;
                //}


                // recupero gli elementi da rimuovere in ognuna delle posizioni specificate
                // e li rimuovo dall lista di elementi del controllo
                List<TypeSource> typesourcesremoved = new List<TypeSource>();
                indexs.ForEach(index =>
                {
                    if (index < typesources.Count)
                        typesourcesremoved.Add(rbl.GetItem<TypeSource>(index));
                });
                typesourcesremoved.ForEach(typesourceremoved => typesources.Remove(typesourceremoved));

                

                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Rimuove un determinato elemento dalla RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale rimuovere l'elemento</param>
        /// <param name="removeItem">L'elemento da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int Remove<TypeSource>(this RadioButtonList rbl, TypeSource removeItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {

                

                // log se removeItem è nullo
                if (removeItem == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("removeItem == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }

                // rimuovo l'elemento
                typesources.Remove(removeItem);


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();
            
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }

        


        /// <summary>
        /// Rimuove un determinato elemento dalla RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale rimuovere l'elemento</param>
        /// <param name="removeItems">Lista degli elementi da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int Remove<TypeSource>(this RadioButtonList rbl, List<TypeSource> removeItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                
                // logger se removeItem è nullo o non contiene elementi
                if (removeItems.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("removeItems == null or removeItems.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }

                // rimuovo gli elementi
                removeItems.ForEach(item => { if (item != null) typesources.Remove(item); });


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }




        /// <summary>
        /// Inserisce un nuovo elemento in una determinata posizione, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList in cui inserire l'elemento</param>
        /// <param name="replaceItem">L'elemento da inserire</param>
        /// <param name="index">La posizione alla quale inserire l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int ReplaceAt<TypeSource>(this RadioButtonList rbl, TypeSource replaceItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // logger se newItem è nullo
                if (replaceItem == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("replaceItem == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                if (index >= typesources.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // aggiorno l'elemento con quello nuovo
                typesources[index] = replaceItem;


                //if (index > 0 && (order == Order.DescendentExceptFirst
                //                  || order == Order.DescendentAll
                //                  || order == Order.AscendentExceptFirst
                //                  || order == Order.AscendentAll))
                if (index > 0)
                    order = Order.None;


                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error


        }





        /// <summary>
        /// Inserisce nuovi elementi in determinata posizioni, all'interno di una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList in cui inserire l'elemento</param>
        /// <param name="replaceItems">La lista di elementi da inserire in determinate posizioni</param>
        /// <param name="indexs">La lista di indici ai quali inserire gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella RadioButtonList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il number di elementi presenti nella RadioButtonList</returns>
        public static int ReplaceAt<TypeSource>(this RadioButtonList rbl, List<TypeSource> replaceItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // logger se la lista è nulla o vuota
                if (replaceItems.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("replaceItems == null || replaceItems.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se la lista è nulla o vuota
                if (indexs.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs == null || indexs.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                //  logger se le liste non hanno lo stesso number di elementi
                //if (indexs.Count != replaceItems.Count)
                //{

                //    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count != replaceItems.Count");
                //    return Int32.MinValue;

                //}


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = rbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                if (indexs.Count >= typesources.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs.Count >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // inserisco gli elementi nuovi
                int contItem = -1;
                indexs.ForEach(index =>
                {

                    contItem++;
                    if (index >= 0 // l'indice deve essere >= 0 
                        &&
                        replaceItems[contItem] != null  // l'item da sostituire non è nullo 
                        &&
                        index < typesources.Count)  // l'indice non è oltre il limite della lista
                        typesources[index] = replaceItems[contItem];

                });



                // carico la lista di elementi
                rbl.Bind<TypeSource>(typesources, rbl.DataTextField, rbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


                return typesources.Count();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error


        }



        
        /// <summary>
        /// Recupera l'elemento da una RadioButtonList in una determinata posizione
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare l'elemento</param>
        /// <param name="index">Posizione alla quale recuperare l'elemento</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItem<TypeSource>(this RadioButtonList rbl, int index)
        where TypeSource : new()
        {


            try
            {
                

                if (index < 0)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index < 0"), TypeError.Managed);
                    return default(TypeSource);
                }


                if (index >= rbl.Items.Count)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= rbl.Items.Count"), TypeError.Managed);
                    return default(TypeSource);
                }

             
                return rbl.GetList<TypeSource>(index)[0];

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeSource);

            }
            #endregion Manage Error
        }




        /// <summary>
        /// Recupera l'elemento da una RadioButtonList in una determinata posizione
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare l'elemento</param>
        /// <param name="indexs">Lista delle posizioni alla quali recuperare gli elementi</param>
        /// <returns>La lista di elementi recuperato</returns>
        public static List<TypeSource> GetItem<TypeSource>(this RadioButtonList rbl, List<int> indexs)
        where TypeSource : new()
        {


            try
            {


                if (indexs.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs == null || indexs.Count() == 0"), TypeError.Managed);
                    return default(List<TypeSource>);
                }



                List<TypeSource> typesources = new List<TypeSource>();
                indexs.ForEach(index => 
                {

                    TypeSource typesource = rbl.GetItem<TypeSource>(index);
                    if (typesource != null) typesources.Add(typesource); 
                });

                return typesources;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<TypeSource>);

            }
            #endregion Manage Error
        }

        


        /// <summary>
        /// Recupera un determinato elemento da una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare l'elemento</param>
        /// <param name="item">Elemento della RadioButtonlist da recuperar</param>
        /// <param name="typeItem">Indica se la proprietà per recuperare l'elemento è un Testo o un Valore</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItem<TypeSource>(this RadioButtonList rbl, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
        where TypeSource : new()
        {

         

            try
            {

                if (string.IsNullOrEmpty(item))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("item is null or empty"), TypeError.Managed);
                    return default(TypeSource);
                }
            
                

                return (typeItem == ItemBoundedProperty.Text)
                        ? rbl.GetList<TypeSource>().Find(typesource => item.GetType().GetProperty(rbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim())
                        : rbl.GetList<TypeSource>().Find(typesource => item.GetType().GetProperty(rbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim());
            
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeSource);

            }
            #endregion Manage Error
        }




        /// <summary>
        /// Recupera determinati elementi da una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare gli elementi</param>
        /// <param name="items">Lista degli elementi della RadioButtonList da recuperare</param>
        /// <param name="typeItem">Indica se le proprietà listate sono il Testo o il Valore degli elementi</param>
        /// <returns>La lista di elementi recuperati</returns>
        public static List<TypeSource> GetItem<TypeSource>(this RadioButtonList rbl, List<string> items, ItemBoundedProperty typeItem = ItemBoundedProperty.Text )
        where TypeSource : new()
        {



            try
            {

                // logger se la lista di proprietà è nulla o vuota
                if (items.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items == null or items.Count == 0"), TypeError.Managed);
                    return default(List<TypeSource>);

                }


                // recupero gli item già presenti nel controllo
                List<TypeSource> alltypesources = rbl.GetList<TypeSource>();


                List<TypeSource> typesources = new List<TypeSource>();
                items.ForEach(proprieta =>
                {
                    if (!string.IsNullOrEmpty(proprieta))
                    {
                        
                        if (typeItem == ItemBoundedProperty.Text)
                            typesources.AddRange(
                                alltypesources.Where(item => item.GetType().GetProperty(rbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
                                );
                        else
                            typesources.AddRange(
                                 alltypesources.Where(item => item.GetType().GetProperty(rbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
                                 );
                    }
                });

                return typesources;
            }

            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<TypeSource>);

            }
            #endregion Manage Error
        }




        /// <summary>
        /// Recupera il dizionario degli elementi di una RadioButtonList
        /// </summary>
        /// <param name="rbl">RadioButtonList dalla quale recuperare la lista di elementi</param>
        /// <returns>Un dizionario con le coppie (Testo,Valore) degli elementi contenuti nella RadioButtonList</returns>
        public static Dictionary<string, string> GetDictionary(this RadioButtonList rbl)
        {
            try
            {
                Dictionary<string, string> DictionaryItems = new Dictionary<string, string>();


                IQueryable<ListItem> items = rbl.Items.AsQueryable().Cast<ListItem>();


                items.ToList().ForEach(item =>
                {

                    DictionaryItems.Add(item.Text, item.Value);


                });

                return DictionaryItems;
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
        /// Recupera l'elemento selezionato in una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList dalla quale recuperare l'elemento selezionato</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource GetSelectedItem<TypeSource>(this RadioButtonList rbl)
        where TypeSource : new()
        {

        
            try
            {

                return rbl.GetList<TypeSource>(rbl.SelectedIndex)[0];


            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeSource);

            }
            #endregion Manage Error
        }




        /// <summary>
        /// Imposta l'elemento da selezionare in una RadioButtonList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella RadioButtonList</typeparam>
        /// <param name="rbl">RadioButtonList nella quale impostare l'elemento selezionato</param>
        /// <param name="item">Proprietà della RadioButtonList mediante la quale impostare l'elemento da selezionare</param>
        /// <param name="typeItem">Indica se la proprietà di selezione è un Testo o un Valore</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource SetSelectedItem<TypeSource>(this RadioButtonList rbl, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text )
        where TypeSource : new()
        {

        

            try
            {
                if (string.IsNullOrEmpty(item))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("item is null or empty"), TypeError.Managed);
                    return default(TypeSource);
                }


                rbl.SelectedIndex = -1;

                if (typeItem == ItemBoundedProperty.Text)
                    rbl.Items.FindByText(item.Trim()).Selected = true;
                else
                    rbl.Items.FindByValue(item.Trim()).Selected = true;

                return rbl.GetItem<TypeSource>(rbl.SelectedIndex);

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeSource);

            }
            #endregion Manage Error
            
        }



    }

}
