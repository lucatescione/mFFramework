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
    /// Metodi estesi per il WebControl CheckBoxList
    /// </summary>
    public static class CheckBoxListExtensions
    {




        #region ----  PUBLIC  -----




        /// <summary>
        /// Carica gli elementi in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList da caricare</param>
        /// <param name="datasource">La lista di elementi da caricare nella CheckBoxList</param>
        /// <param name="propertyText">Il nome della proprietà di testo i cui valori appariranno nell'elenco della CheckBoxList</param>
        /// <param name="propertyVakue">Il nome della proprietà di valore i cui valori saranno associati agli elementi della CheckBoxList</param>
        /// <param name="order">L'ordine da associare alla lista</param>
        /// <param name="defaultItem">L'elemento iniziale di default</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare anche gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int Bind<TypeSource>(this CheckBoxList cbl, List<TypeSource> datasource, string propertyText, string propertyVakue, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                if (datasource.Find(o => o.GetType().GetProperty(propertyVakue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null) != null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyValue == null"));
                    return Int32.MinValue;
                }


                // se necessario, rimuovo gli items vuoti
                items = (thereAreItemsEmpty) ? datasource.ToArray() : datasource.Where(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() != string.Empty).ToArray();


                // ordino gli item
                items = Functions.OrderList<TypeSource>(items, order, propertyText, propertyVakue);



                // associo finalmente il datasource
                cbl.DataSource = items;
                cbl.DataTextField = propertyText;
                cbl.DataValueField = propertyVakue;
                cbl.DataBind();


                // se è necessario, inserisco l'item di default al primo posto
                if (!string.IsNullOrEmpty(defaultItem))
                    cbl.Items.Insert(0, Functions.AddDefaultItem(defaultItem));


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
        /// Recupera la lista di elementi contenuti in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare gli elementi</param>
        /// <param name="index">Indice al quale recuperare un elemento specifico</param>
        /// <returns>La lista di elementi contenuti nella CheckBoxList</returns>
        public static List<TypeSource> GetList<TypeSource>(this CheckBoxList cbl, int index = 0)
        where TypeSource : new()
        {

            try
            {


                //  recupero tutti gli item presenti nella CheckBoxList o, se specificato l'indice, quello corrispondente alla posizione
                List<ListItem> items = new List<ListItem>();
                if (index <= 0)
                    items = cbl.Items.OfType<ListItem>().ToList().ExcludeDefaultItem();
                else
                    items.Add(cbl.Items[index]);


                //  recupero le proprietà corrispondenti all'propertyText e al propertyValue della CheckBoxList
                TypeSource typesource = new TypeSource();
                PropertyInfo propertyText = typesource.GetType().GetProperty(cbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                PropertyInfo propertyValue = typesource.GetType().GetProperty(cbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);


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
        /// Aggiunge un nuovo elemento agli elementi presenti in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList alla quale aggiungere il nuovo elemento</param>
        /// <param name="newItem">L'elemento da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento iniziale di default</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il mumero di elementi presenti nella CheckBoxList</returns>
        public static int Add<TypeSource>(this CheckBoxList cbl, TypeSource newItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // logger se il newItem è nullo
                if (newItem == null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("newItem == null"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // aggiungo quello nuovo
                typesources.Add(newItem);


                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Aggiunge nuovi elementi agli elementi presenti in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList alla quale aggiungere i nuovi elementi</param>
        /// <param name="newItems">La lista di elementi da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il mumero di elementi presenti nella CheckBoxList</returns>
        public static int Add<TypeSource>(this CheckBoxList cbl, List<TypeSource> newItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // aggiungo quelli nuovi
                newItems.ForEach(newItem => { if (newItem != null) typesources.Add(newItem); });


                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Inserisce un nuovo elemento, in una determinata posizione, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList in cui inserire l'elemento</param>
        /// <param name="newItem">L'elemento da inserire</param>
        /// <param name="index">La posizione alla quale inserire l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int InsertAt<TypeSource>(this CheckBoxList cbl, TypeSource newItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


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
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Inserisce nuovi elementi, in determinate posizioni, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList in cui inserire gli elementi</param>
        /// <param name="newItems">La lista di elementi da inserire in determinate posizioni</param>
        /// <param name="indexs">La lista di indici ai quali inserire gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int InsertAt<TypeSource>(this CheckBoxList cbl, List<TypeSource> newItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                if (indexs.Count != newItems.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs.Count != newItems.Count"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                //if (indexs.Count >= typesources.Count)
                //{

                //    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
                //    return Int32.MinValue;
                //}


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
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove un elemento, in una determinata posizione, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale rimuovere l'elemento</param>
        /// <param name="index">La posizione alla quale rimuovere l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int RemoveAt<TypeSource>(this CheckBoxList cbl, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se indice oltre i limiti della matrice
                if (index >= typesources.Count)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;
                }


                // rimuovo l'oggetto in posizione
                typesources.RemoveAt(index);


                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove gli elementi, in determinate posizioni, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale rimuovere gli elementi</param>
        /// <param name="indexs">La lista di indici ai quali rimuovere gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int RemoveAt<TypeSource>(this CheckBoxList cbl, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


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
                        typesourcesremoved.Add(cbl.GetItemAt<TypeSource>(index));
                });
                typesourcesremoved.ForEach(typesourceremoved => typesources.Remove(typesourceremoved));



                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove un determinato elemento dalla CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale rimuovere l'elemento</param>
        /// <param name="removeItem">L'elemento da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int Remove<TypeSource>(this CheckBoxList cbl, TypeSource removeItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }

                // rimuovo l'elemento
                typesources.Remove(removeItem);


                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove gli elementi all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale rimuovere gli elementi</param>
        /// <param name="removeItems">Lista degli elementi da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int Remove<TypeSource>(this CheckBoxList cbl, List<TypeSource> removeItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }

                // rimuovo gli elementi
                removeItems.ForEach(item => { if (item != null) typesources.Remove(item); });


                // carico la lista di elementi
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Sostituisce con un nuovo elemento, quello presente in una determinata posizione, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList in cui sostituire l'elemento</param>
        /// <param name="replaceItem">L'elemento che sostituisce quello esistente</param>
        /// <param name="index">La posizione nella quale sostituire l'elemento esistente</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int ReplaceAt<TypeSource>(this CheckBoxList cbl, TypeSource replaceItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


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
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Sostituisce con nuovi elementi, quelli presenti in determinate posizioni, all'interno di una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList in cui sostituire gli elementi</param>
        /// <param name="replaceItems">La lista di elementi che sostituiscono quelli esistenti</param>
        /// <param name="indexs">La lista delle posizioni nelle quali sostituire gli elementi esistenti</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella CheckBoxList</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella CheckBoxList</returns>
        public static int ReplaceAt<TypeSource>(this CheckBoxList cbl, List<TypeSource> replaceItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                if (indexs.Count != replaceItems.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs.Count != replaceItems.Count"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = cbl.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                //if (indexs.Count >= typesources.Count)
                //{

                //    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
                //    return Int32.MinValue;
                //}


                // aggiorno gli elementi nuovi
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
                cbl.Bind<TypeSource>(typesources, cbl.DataTextField, cbl.DataValueField, order, defaultItem, thereAreItemsEmpty);


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
        /// Recupera l'elemento da una CheckBoxList, presente in una determinata posizione
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare l'elemento</param>
        /// <param name="index">Posizione alla quale recuperare l'elemento</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItemAt<TypeSource>(this CheckBoxList cbl, int index)
        where TypeSource : new()
        {


            try
            {


                if (index < 0)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index < 0"), TypeError.Managed);
                    return default(TypeSource);
                }


                if (index >= cbl.Items.Count)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= cbl.Items.Count"), TypeError.Managed);
                    return default(TypeSource);
                }


                return cbl.GetList<TypeSource>(index)[0];

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
        /// Recupera gli elementi, presenti in determinate posizioni, da una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare gli elementi</param>
        /// <param name="indexs">La lista delle posizioni alle quali recuperare gli elementi</param>
        /// <returns>La lista di elementi recuperati</returns>
        public static List<TypeSource> GetItemAt<TypeSource>(this CheckBoxList cbl, List<int> indexs)
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

                    TypeSource typesource = cbl.GetItemAt<TypeSource>(index);
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
        /// Recupera un determinato elemento da una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare l'elemento</param>
        /// <param name="item">Elemento della CheckBoxList da recuperare</param>
        /// <param name="typeItem">Indica se la proprietà per recuperare l'elemento è un Testo o un Valore</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItem<TypeSource>(this CheckBoxList cbl, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
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
                        ? cbl.GetList<TypeSource>().Find(typesource => typesource.GetType().GetProperty(cbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim())
                        : cbl.GetList<TypeSource>().Find(typesource => typesource.GetType().GetProperty(cbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim());

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
        /// Recupera determinati elementi da una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare gli elementi</param>
        /// <param name="items">La lista delle proprietà degli elementi da recuperare</param>
        /// <param name="typeItem">Indica se le proprietà listate sono il testo o il valore degli elementi</param>
        /// <returns>La lista di elementi recuperati</returns>
        public static List<TypeSource> GetItem<TypeSource>(this CheckBoxList cbl, List<string> items, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
        where TypeSource : new()
        {



            try
            {

                // logger se la lista di proprietà è nulla o vuota
                if (items.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items != null && items.Count == 0"), TypeError.Managed);
                    return default(List<TypeSource>);

                }


                // recupero gli item già presenti nel controllo
                List<TypeSource> alltypesources = cbl.GetList<TypeSource>();


                List<TypeSource> typesources = new List<TypeSource>();
                items.ForEach(proprieta =>
                {
                    if (!string.IsNullOrEmpty(proprieta))
                    {

                        if (typeItem == ItemBoundedProperty.Text)
                            typesources.AddRange(
                                alltypesources.Where(item => item.GetType().GetProperty(cbl.DataTextField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
                                );
                        else
                            typesources.AddRange(
                                 alltypesources.Where(item => item.GetType().GetProperty(cbl.DataValueField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
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
        /// Recupera il dizionario degli elementi di una CheckBoxList
        /// </summary>
        /// <param name="cbl">CheckBoxList dalla quale recuperare la lista di elementi</param>
        /// <returns>Un dizionario con le coppie (Testo,Valore) degli elementi contenuti nella CheckBoxList</returns>
        public static Dictionary<string, string> GetDictionary(this CheckBoxList cbl)
        {
            try
            {
                Dictionary<string, string> DictionaryItems = new Dictionary<string, string>();


                IQueryable<ListItem> items = cbl.Items.AsQueryable().Cast<ListItem>();


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
        /// Recupera l'elemento selezionato in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare l'elemento selezionato</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource GetSelectedItem<TypeSource>(this CheckBoxList cbl)
        where TypeSource : new()
        {


            try
            {




                return cbl.GetList<TypeSource>(cbl.SelectedIndex)[0];


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
        /// Recupera gli elementi selezionati in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList dalla quale recuperare gli elementi selezionati</param>
        /// <returns>L'elemento selezionato</returns>
        public static List<TypeSource> GetSelectedItems<TypeSource>(this CheckBoxList cbl)
        where TypeSource : new()
        {


            try
            {


                int cont_item = -1;
                List<TypeSource> selectedtypesource = new List<TypeSource>();
                cbl.Items.OfType<ListItem>().ToList().ForEach(item =>
                {
                    cont_item++;
                    if (item.Selected)
                        selectedtypesource.Add(cbl.GetList<TypeSource>(cont_item)[0]);

               });

                return selectedtypesource;


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
        /// Imposta l'elemento da selezionare in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList nella quale impostare l'elemento selezionato</param>
        /// <param name="item">Elemento della CheckBoxList da selezionare</param>
        /// <param name="typeItem">Indica se la proprietà di selezione dell'elemento è un Testo o un Valore</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource SetSelectedItem<TypeSource>(this CheckBoxList cbl, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
        where TypeSource : new()
        {



            try
            {
                if (string.IsNullOrEmpty(item))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("item is null or empty"), TypeError.Managed);
                    return default(TypeSource);
                }


                cbl.SelectedIndex = -1;

                if (typeItem == ItemBoundedProperty.Text)
                    cbl.Items.FindByText(item.Trim()).Selected = true;
                else
                    cbl.Items.FindByValue(item.Trim()).Selected = true;

                return cbl.GetItemAt<TypeSource>(cbl.SelectedIndex);

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
        /// Imposta gli elementi da selezionare in una CheckBoxList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella CheckBoxList</typeparam>
        /// <param name="cbl">CheckBoxList nella quale impostare gli elementi da selezionare</param>
        /// <param name="items">Lista degli elementi della ChecktBoxList da selezionare</param>
        /// <param name="typeItem">Indica se la proprietà di selezione di ogni elemento è un Testo o un Valore</param>
        /// <returns>L'elemento selezionato</returns>
        public static List<TypeSource> SetSelectedItem<TypeSource>(this CheckBoxList cbl, List<string> items, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
        where TypeSource : new()
        {


            try
            {


                // logger se la lista di proprietà è nulla o vuota
                if (items.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items == null || items.Count == 0"), TypeError.Managed);
                    return default(List<TypeSource>);

                }


               


                cbl.SelectedIndex = -1;

                if (typeItem == ItemBoundedProperty.Text)
                    items.ForEach(item => cbl.Items.FindByText(item.Trim()).Selected = true);
                else
                    items.ForEach(item => cbl.Items.FindByValue(item.Trim()).Selected = true);


                return cbl.GetSelectedItems<TypeSource>();

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<TypeSource>);

            }
            #endregion Manage Error

        }



        #endregion ----  PUBLIC  -----



    }

}
