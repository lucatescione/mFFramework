using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using mFFramework.Conversions;
using mFFramework.LogManager;
using mFFramework.Types;
using mFFramework.Utilities;
using System.Windows.Forms;




namespace mFFramework.Extensions.WindowsControls
{


    /// <summary>
    /// Metodi Estesi per il WebControl ComboBox
    /// </summary>
    public static class ComboBoxExtensions
    {
       
        
        
        
        #region ---  PRIVATE  ----

        
        private static void BindPrimitive<TypeSource>(ref ComboBox cb, ref List<TypeSource> datasource, Order order, ref string defaultItem)
        {


            List<TypeSource> items = null;
            List<TypeSource> itemsexceptfirst = null;
            int contItems = 0;


            items = datasource.Where(o => o != null).ToList();
            

            // ordino gli item per propertyText
            switch (order)
            {

                case Order.AscendentAllForText:
                case Order.AscendentAllForValue:
                    items = items.OrderBy(item => item).ToList();
                    break;


                case Order.DescendentAllForText:
                case Order.DescendentAllForValue:
                    items = items.OrderByDescending(item => item).ToList();
                    break;


                case Order.DescendentExceptFirstForText:
                case Order.DescendentExceptFirstForValue:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(datasource.Count - 1).OrderByDescending(item => item).ToList();
                    itemsexceptfirst.ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;


                case Order.AscendentExceptFirstForText:
                case Order.AscendentExceptFirstForValue:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(datasource.Count - 1).OrderBy(item => item).ToList();
                    itemsexceptfirst.ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;

            }

            cb.DataSource = items;
            //cb.DataBind();

            // se è necessario, inserisco l'item di default al primo posto
            if (!string.IsNullOrEmpty(defaultItem))
                cb.Items.Insert(0, Functions.AddDefaultItem(defaultItem));
        
        }


        #endregion ---  PRIVATE  ----




        #region ---  PUBLIC  ----



        /// <summary>
        /// Carica gli elementi in una DropoDownList
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox da caricare</param>
        /// <param name="datasource">La lista di elementi da caricare nella ComboBox</param>
        /// <param name="propertyText">Nome della proprietà di testo i cui valori appariranno nell'elenco della ComboBox</param>
        /// <param name="propertyValue">Nome della proprietà di valore i cui valori saranno associati all'etichetta di testo</param>
        /// <param name="order">L'ordine da associare alla lista</param>
        /// <param name="defaultItem">Elemento di default</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare anche gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int Bind<TypeSource>(this ComboBox cb, List<TypeSource> datasource, string propertyText, string propertyValue, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                TypeSource[] items = null;
                TypeSource[] itemsExtended = null;

                // se il datasource non esiste (vuoto o non istanziato)
                if (datasource == null || datasource.Count == 0)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("datasource == null or datasource.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;
                }


                if (datasource[0].GetType().IsPrimitive)
                {

                    BindPrimitive<TypeSource>(ref cb, ref datasource, order, ref defaultItem);
                    return datasource.Count;
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

                   

                // se è necessario, inserisco l'item di default al primo posto
                if (!string.IsNullOrEmpty(defaultItem))
                {
                    itemsExtended = new TypeSource[1];
                    itemsExtended[0] = Functions.CreateDefaultItem<TypeSource>(propertyText, propertyValue, defaultItem);
                    itemsExtended = itemsExtended.Concat<TypeSource>(items).ToArray();

                }
                else
                    itemsExtended = items;
            

                // associo finalmente il datasource
                cb.DisplayMember = propertyText;
                cb.ValueMember = propertyValue;
                cb.DataSource = itemsExtended;


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
        /// Recupera la lista di elementi contenuti in una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare gli elementi</param>
        /// <param name="index">Indice al quale recuperare un elemento specifico</param>
        /// <returns>La lista di elementi contenuti nella ComboBox</returns>
        public static List<TypeSource> GetList<TypeSource>(this ComboBox cb, int index = 0)
        where TypeSource : new()
        {

            try
            {


                //  recupero tutti gli item presenti nella ComboBox o, se specificato l'indice, quello corrispondente alla posizione
                List<TypeSource> typesources = new List<TypeSource>();
                if (index <= 0)
                    typesources = cb.Items.OfType<TypeSource>().ToList(); //.EscludiDefaultItem();
                else
                    typesources.Add((TypeSource)cb.Items[index]);



                //  recupero le proprietà corrispondenti all'propertyText e al propertyValue della ComboBox
                TypeSource typesource = new TypeSource();
                PropertyInfo propertyText = typesource.GetType().GetProperty(cb.DisplayMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                PropertyInfo propertyValue = typesource.GetType().GetProperty(cb.ValueMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);


                // se l'etichetta di testo non è una proprietà di TypeSource segnalo il log
                if (propertyText == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyText == null"), TypeError.Managed);
                    return default(List<TypeSource>);
                }


                // se l'etichetta di value non è una proprietà di TypeSource segnalo il log
                if (propertyValue == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("propertyValue == null"), TypeError.Managed);
                    return default(List<TypeSource>);
                }



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
        /// Aggiunge un nuovo elemento agli elementi presenti in una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox alla quale aggiungere il nuovo elemento</param>
        /// <param name="newItem">L'elemento da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int Add<TypeSource>(this ComboBox cb, TypeSource newItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // aggiungo quello nuovo
                typesources.Add(newItem);


                // carico la lista di elementi
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Aggiunge nuovi elementi agli elementi presenti in una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox alla quale aggiungere il nuovo elemento</param>
        /// <param name="newItems">La lista di elementi da aggiungere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int Add<TypeSource>(this ComboBox cb, List<TypeSource> newItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // aggiungo quelli nuovi
                newItems.ForEach(newItem => { if (newItem != null) typesources.Add(newItem); });


                // carico la lista di elementi
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Inserisce un nuovo elemento, in una determinata posizione, all'interno di una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox in cui inserire l'elemento</param>
        /// <param name="newItem">L'elemento da inserire</param>
        /// <param name="index">La posizione alla quale inserire l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int InsertAt<TypeSource>(this ComboBox cb, TypeSource newItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
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
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Inserisce nuovi elementi, in determinate posizioni, all'interno di una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox in cui inserire gli elementi</param>
        /// <param name="newItems">La lista di elementi da inserire in determinate posizioni</param>
        /// <param name="indexs">La lista di indici ai quali inserire gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int InsertAt<TypeSource>(this ComboBox cb, List<TypeSource> newItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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


                //  logger se le liste non hanno lo stesso numero di elementi
                if (indexs.Count != newItems.Count)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("indexs.Count != newItems.Count"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // recupero gli item già presenti nel controllo, ad esclusione di quello di default
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                //if (indexs.Count >= typesources.Count)
                //{

                //    logger.WriteWarning(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
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
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove un elemento, in una determinata posizione, all'interno di una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale rimuovere l'elemento</param>
        /// <param name="index">La posizione alla quale rimuovere l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int RemoveAt<TypeSource>(this ComboBox cb, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
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
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove gli elementi, in determinate posizioni, all'interno di una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale rimuovere gli elementi</param>
        /// <param name="indexs">La lista di indici ai quali rimuovere gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int RemoveAt<TypeSource>(this ComboBox cb, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
        where TypeSource : new()
        {

            try
            {


                // recupero gli item già presenti nel controllo
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources == null)
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se indice oltre i limiti della matrice
                //if (indexs.Count >= typesources.Count)
                //{
                //    logger.WriteWarning(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
                //    return Int32.MinValue;
                //}


                // recupero gli elementi da rimuovere in ognuna delle posizioni specificate
                // e li rimuovo dall lista di elementi del controllo
                List<TypeSource> typesourcesremoved = new List<TypeSource>();
                indexs.ForEach(index =>
                {
                    if (index < typesources.Count)
                        typesourcesremoved.Add(cb.GetItem<TypeSource>(index));
                });
                typesourcesremoved.ForEach(typesourceremoved => typesources.Remove(typesourceremoved));



                // carico la lista di elementi
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove un determinato elemento dalla ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale rimuovere l'elemento</param>
        /// <param name="removeItem">L'elemento da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int Remove<TypeSource>(this ComboBox cb, TypeSource removeItem, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null"), TypeError.Managed);
                    return Int32.MinValue;

                }

                // rimuovo l'elemento
                typesources.Remove(removeItem);


                // carico la lista di elementi
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Rimuove gli elementi dalla ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale rimuovere l'elemento</param>
        /// <param name="removeItems">Lista degli elementi da rimuovere</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int Remove<TypeSource>(this ComboBox cb, List<TypeSource> removeItems, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"));
                    return Int32.MinValue;

                }

                // rimuovo gli elementi
                removeItems.ForEach(item => { if (item != null) typesources.Remove(item); });


                // carico la lista di elementi
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Sostituisce un elemento, presente in una determinata posizione, all'interno di una ComboBox, con uno nuovo
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox in cui inserire l'elemento</param>
        /// <param name="replaceItem">L'elemento da sostituire a quello presente</param>
        /// <param name="index">La posizione alla quale sostituire l'elemento</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int ReplaceAt<TypeSource>(this ComboBox cb, TypeSource replaceItem, int index, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


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
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Sostituisce nuovi elementi a quelli presenti, in determinata posizioni, all'interno di una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox in cui inserire l'elemento</param>
        /// <param name="replaceItems">La lista di elementi da sostituire in determinate posizioni</param>
        /// <param name="indexs">La lista di indici ai quali sostituire gli elementi della lista</param>
        /// <param name="order">L'ordine della lista di elementi aggiornata</param>
        /// <param name="defaultItem">L'elemento di default nella ComboBox</param>
        /// <param name="thereAreItemsEmpty">Indica se considerare gli elementi vuoti</param>
        /// <returns>Il numero di elementi presenti nella ComboBox</returns>
        public static int ReplaceAt<TypeSource>(this ComboBox cb, List<TypeSource> replaceItems, List<int> indexs, Order order = Order.None, string defaultItem = null, bool thereAreItemsEmpty = true)
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
                List<TypeSource> typesources = cb.GetList<TypeSource>();


                // logger se non ci sono elementi presenti nel controllo
                if (typesources.IsVoid())
                {

                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typesources == null or typesources.Count == 0"), TypeError.Managed);
                    return Int32.MinValue;

                }


                // logger se l'indice è oltre il limite della matrice
                //if (indexs.Count >= typesources.Count)
                //{

                //    logger.WriteWarning(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), "indexs.Count >= typesources.Count");
                //    return Int32.MinValue;
                //}


                // sostituisco gli elementi nuovi
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
                cb.Bind<TypeSource>(typesources, cb.DisplayMember, cb.ValueMember, order, defaultItem, thereAreItemsEmpty);


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
        /// Recupera l'elemento da una ComboBox, in una determinata posizione
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare l'elemento</param>
        /// <param name="index">Posizione alla quale recuperare l'elemento</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItem<TypeSource>(this ComboBox cb, int index)
        where TypeSource : new()
        {


            try
            {


                if (index < 0)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index < 0"), TypeError.Managed);
                    return default(TypeSource);
                }


                if (index >= cb.Items.Count)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("index >= gv.Items.Count"), TypeError.Managed);
                    return default(TypeSource);
                }


                return cb.GetList<TypeSource>(index)[0];

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
        /// Recupera gli elementi da una ComboBox, presenti in determinate posizioni
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare l'elemento</param>
        /// <param name="indexs">Lista delle posizioni alla quali recuperare gli elementi</param>
        /// <returns>La lista di elementi recuperati</returns>
        public static List<TypeSource> GetItem<TypeSource>(this ComboBox cb, List<int> indexs)
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

                    TypeSource typesource = cb.GetItem<TypeSource>(index);
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
        /// Recupera un determinato elemento da una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare l'elemento</param>
        /// <param name="item">Elemento della ComboBox da recuperare</param>
        /// <param name="typeItem">Indica se la proprietà per recuperare l'elemento è un Testo o un Valore</param>
        /// <returns>L'elemento recuperato</returns>
        public static TypeSource GetItem<TypeSource>(this ComboBox cb, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
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
                        ? cb.GetList<TypeSource>().Find(typesource => typesource.GetType().GetProperty(cb.DisplayMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim())
                        : cb.GetList<TypeSource>().Find(typesource => typesource.GetType().GetProperty(cb.ValueMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(typesource, null).ToString().Trim() == item.Trim());

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
        /// Recupera determinati elementi da una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare gli elementi</param>
        /// <param name="items">Lista degli elementi della ComboBox da recuperare</param>
        /// <param name="typeItem">Indica se la proprietà per recuperare gli elementi è un Testo o un Valore</param>
        /// <returns>La lista di elementi recuperati</returns>
        public static List<TypeSource> GetItem<TypeSource>(this ComboBox cb, List<string> items, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
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
                List<TypeSource> alltypesources = cb.GetList<TypeSource>();


                List<TypeSource> typesources = new List<TypeSource>();
                items.ForEach(proprieta =>
                {
                    if (!string.IsNullOrEmpty(proprieta))
                    {

                        if (typeItem == ItemBoundedProperty.Text)
                            typesources.AddRange(
                                alltypesources.Where(item => item.GetType().GetProperty(cb.DisplayMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
                                );
                        else
                            typesources.AddRange(
                                 alltypesources.Where(item => item.GetType().GetProperty(cb.ValueMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null).ToString().Trim() == proprieta.Trim()).ToList()
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
        /// Recupera il dizionario degli elementi di una ComboBox
        /// </summary>
        /// <param name="cb">ComboBox dalla quale recuperare la lista di elementi</param>
        /// <returns>Un dizionario con le coppie (Testo,Valore) degli elementi contenuti nella ComboBox</returns>
        public static Dictionary<string, string> GetDictionary(this ComboBox cb)
        {
            try
            {
                Dictionary<string, string> DictionaryItems = new Dictionary<string, string>();


                IQueryable<ListItem> items = cb.Items.AsQueryable().Cast<ListItem>();


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
        /// Recupera l'elemento selezionato in una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox dalla quale recuperare l'elemento selezionato</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource GetSelectedItem<TypeSource>(this ComboBox cb)
        where TypeSource : new()
        {


            try
            {

                return cb.GetItem<TypeSource>(cb.SelectedIndex);


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
        /// Imposta l'elemento da selezionare in una ComboBox
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella ComboBox</typeparam>
        /// <param name="cb">ComboBox nella quale impostare l'elemento selezionato</param>
        /// <param name="item">Proprietà della ComboBox mediante la quale impostare l'elemento da selezionare</param>
        /// <param name="typeItem">Indica se la proprietà di selezione è un Testo o un Valore</param>
        /// <returns>L'elemento selezionato</returns>
        public static TypeSource SetSelectedItem<TypeSource>(this ComboBox cb, string item, ItemBoundedProperty typeItem = ItemBoundedProperty.Text)
        where TypeSource : new()
        {



            try
            {
                if (string.IsNullOrEmpty(item))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("item is null or empty"), TypeError.Managed);
                    return default(TypeSource);
                }


                cb.SelectedIndex = -1;

                
//                if (typeItem == ItemBoundedProperty.Text)
//                    cb.SelectedItem = item; //cb.Items[cb.Items.IndexOf(item.Trim())];
////                    cb.Items[cb.Items.IndexOf(item.Trim())].Selected = true;
//                else
                   
//                    cb.Items.FindByValue(item.Trim()).Selected = true;


                return cb.GetItem<TypeSource>(cb.SelectedIndex);

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(TypeSource);

            }
            #endregion Manage Error

        }



        #endregion ---  PUBLIC  ----






    }

}
