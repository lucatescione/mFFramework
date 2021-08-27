using mFFramework.Conversions;
using mFFramework.LogManager;
using mFFramework.Types;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;






namespace mFFramework.Utilities
{

    /// <summary>
    /// Funzioni statiche di uso generico
    /// </summary>
    public static class Functions
    {


        private static int MimeSampleSize = 256;
        private static string DefaultMimeType = "application/octet-stream";
        private static string ValueDefaultItem = Int64.MinValue + "|£%|&|^|" + Int64.MaxValue;

        private static string ValueDefaultItemString = Int64.MinValue + "|£%|&|^|" + Int64.MaxValue;
        private static Int16 ValueDefaultItemShort = Int16.MinValue;
        private static Int32 ValueDefaultItemInt = Int32.MinValue;
        private static Int64 ValueDefaultItemLong = Int64.MinValue;

        

        #region ----  PUBLIC  ----


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetValueDefaultItem()
        {

            return ValueDefaultItem;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Compare(string x, string y)
        {
            string s1 = x as string;
            if (s1 == null)
            {
                return 0;
            }
            string s2 = y as string;
            if (s2 == null)
            {
                return 0;
            }

            int len1 = s1.Length;
            int len2 = s2.Length;
            int marker1 = 0;
            int marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                char ch1 = s1[marker1];
                char ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                char[] space1 = new char[len1];
                int loc1 = 0;
                char[] space2 = new char[len2];
                int loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                    {
                        ch1 = s1[marker1];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                    {
                        ch2 = s2[marker2];
                    }
                    else
                    {
                        break;
                    }
                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                string str1 = new string(space1);
                string str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    long thisNumericChunk = long.Parse(str1);
                    long thatNumericChunk = long.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                {
                    result = str1.CompareTo(str2);
                }

                if (result != 0)
                {
                    return result;
                }
            }
            return len1 - len2;
        }
       

        /// <summary>
        /// Esclude l'item di default da una lista di elementi
        /// </summary>
        /// <param name="items">La lista di elementi</param>
        /// <returns>La lista di elementi senza l'elemento di default</returns>
        public static List<ListItem> ExcludeDefaultItem(this List<ListItem> items)
        {
            try
            {

                if (items.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items is null or empty"), TypeError.Managed);
                    return default(List<ListItem>);
                }


                return items.Where(i => i.Value != ValueDefaultItem).ToList();
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<ListItem>);

            }
            #endregion Manage Error
        }



        /// <summary>
        /// Aggiunge l'item di default da una lista di elementi
        /// </summary>
        /// <param name="defaultItem">L'item di default</param>
        /// <returns>La lista di elementi con l'elemento di default</returns>
        public static ListItem AddDefaultItem(string defaultItem)
        {

            try
            {
                if (string.IsNullOrEmpty(defaultItem))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items is null or empty"), TypeError.Managed);
                    return null;
                }

                return new ListItem { Text = defaultItem, Value = ValueDefaultItem };
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
        /// Crea l'elemento di default da aggiungere a una lista di elementi da caricare in un controllo web/form
        /// </summary>
        /// <typeparam name="TypeSource">>La tipologia dell'elemento di default</typeparam>
        /// <param name="propertyText">Nome della proprietà di testo dell'elemento di default</param>
        /// <param name="propertyValue">Nome della proprietà di valore dell'elemento di default</param>
        /// <param name="defaultItem">Il testo dell'elemento di default</param>
        /// <returns>L'elemento di default</returns>
        public static TypeSource CreateDefaultItem<TypeSource>(string propertyText, string propertyValue, string defaultItem)
        where TypeSource : new()
        {

            try
            {

                if (string.IsNullOrEmpty(defaultItem))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("items is null or empty"), TypeError.Managed);
                    return default(TypeSource);
                }


                // ricavo le due proprietà del typeSource da impostare
                TypeSource typesource = new TypeSource();
                PropertyInfo _propertyText = typesource.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                PropertyInfo _propertyValue = typesource.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

               
                // imposto la proprietà di testo  
                _propertyText.SetValue(typesource, defaultItem, null);


                // imposto la proprietà di valore
                switch (_propertyValue.PropertyType.Name.ToLower())
                {
                
                    default:
                    case "string":
                        _propertyValue.SetValue(typesource, ValueDefaultItemString, null);
                        break;

                    case "int16":
                        _propertyValue.SetValue(typesource, ValueDefaultItemShort, null);
                        break;

                    case "int32":
                        _propertyValue.SetValue(typesource, ValueDefaultItemInt, null);
                        break;

                    case "int64":
                        _propertyValue.SetValue(typesource, ValueDefaultItemLong, null);
                        break;
                
                }
               
               
                return typesource;
            
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
        /// Download di un file
        /// </summary>
        /// <param name="pathfile">Percorso fisico del file</param>
        /// <param name="webPage">Pagina dalla quale si sta scaricando il file</param>
        public static void Download(string pathfile, Page webPage)
        {


            try
            {
                FileInfo fileInfo = new FileInfo(pathfile);


                //imposta le headers
                //webPage.Response.Expires = -1;
                webPage.Response.Buffer = true;
                //webPage.Response.Clear();
                webPage.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                webPage.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                webPage.Response.ContentType = "application/x-rar-compressed";

                // leggo dal fileBuffer e scrivo nello stream di risposta
                //webPage.Response.WriteFile(path);
                webPage.Response.TransmitFile(pathfile);
                webPage.Response.End();
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                
            }
            #endregion Manage Error

        }



        /// <summary>
        /// Download di un file
        /// </summary>
        /// <param name="fileBuffer">L'array di byte relativo al file da scaricare</param>
        /// <param name="filePath">Il percorso completo del file che verrà scaricato</param>
        /// <param name="fileType">Il tipo di file da specificare nel content type</param>
        /// <param name="httpContextCache">Specifica la tipologia di Cache attivata per la pagina di download</param>
        public static void Download(byte[] fileBuffer
                                    , string filePath
                                    , TypeFile fileType
                                    , HttpContexCache httpContextCache = HttpContexCache.NoCache)
        {

            try
            {

                if (fileBuffer.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("fileBuffer is null or empty"), TypeError.Managed);
                    return;
                }


                string[] v_fileName = filePath.Split('\\');


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Expires = 0;
                HttpContext.Current.Response.ContentType = fileType.GetPattern();
                HttpContext.Current.Response.AddHeader("Content-Type", fileType.GetPattern());
                HttpContext.Current.Response.AddHeader("Content-Length", fileBuffer.Length.ToString());
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + v_fileName[v_fileName.Length - 1]);

                switch (httpContextCache.GetCode().ToLower())
                { 
                

                    case "nocache":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        break;


                    case "private":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Private);
                        break;


                    case "public":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
                        break;


                    case "server":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Server);
                        break;


                    case "serverandnocache":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                        break;


                    case "serverandprivate":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                        break;

                  
                }
                
                HttpContext.Current.Response.BinaryWrite(fileBuffer);
                HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.SuppressContent = true;
                //HttpContext.Current.ApplicationInstance.Request.
                HttpContext.Current.ApplicationInstance.CompleteRequest();

                //HttpContext.Current.Response.Close();
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);

            }
            #endregion Manage Error
        }



        /// <summary>
        /// Download di un file Xml
        /// </summary>
        /// <param name="fileBuffer">L'array di byte relativo al file da scaricare</param>
        /// <param name="filePath">Il percorso completo del file che verrà scaricato</param>
        /// <param name="httpContextCache">Specifica la tipologia di Cache attivata per la pagina di download</param>
        public static void DownloadXml(byte[] fileBuffer, string filePath, HttpContexCache httpContextCache = HttpContexCache.NoCache)
        {

            try
            {

                if (fileBuffer.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("fileBuffer is null or empty"), TypeError.Managed);
                    return;
                }


                string[] v_fileName = filePath.Split('\\');


                //string innerXml = Encoding.ASCII.GetString(fileBuffer, 0, fileBuffer.Length);

                //innerXml = Regex.Replace(innerXml, @"\p{C}+", string.Empty);


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Expires = 0;
                HttpContext.Current.Response.ContentType = "application/xml";
                HttpContext.Current.Response.AddHeader("Content-Type", "application/xml");
                HttpContext.Current.Response.AddHeader("Content-Length", fileBuffer.Length.ToString());
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + v_fileName[v_fileName.Length - 1]);

                switch (httpContextCache.GetCode().ToLower())
                {


                    case "nocache":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        break;


                    case "private":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Private);
                        break;


                    case "public":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
                        break;


                    case "server":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Server);
                        break;


                    case "serverandnocache":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                        break;


                    case "serverandprivate":
                        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                        break;



                }


                HttpContext.Current.Response.OutputStream.Write(fileBuffer, 0, fileBuffer.Length);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);

            }
            #endregion Manage Error
        }


        
        /// <summary>
        /// Recupera il MIMEType di un file a partire dal suo filebuffer
        /// </summary>
        /// <param name="filebuffer">Il filebuffer relativo al file da scaricare</param>
        /// <returns>La stringa rappresentante l'estensione del file</returns>
        public static string GetMimeFromBytes(byte[] filebuffer)
        {
            try
            {
                uint mimeType;
                FindMimeFromData(0, null, filebuffer, (uint)MimeSampleSize, null, 0, out mimeType, 0);

                var mimePointer = new IntPtr(mimeType);
                var mime = Marshal.PtrToStringUni(mimePointer);
                Marshal.FreeCoTaskMem(mimePointer);

                return mime ?? DefaultMimeType;
            }
            catch
            {
                return DefaultMimeType;
            }
        }



        /// <summary>
        /// Recupera il percorso completo del nodo in un file XML
        /// </summary>
        /// <param name="element">Elemento XML del quale recuperare il percorso</param>
        /// <returns>Il percordo completo del nodo</returns>
        public static string GetPathNode(XmlElement element)
        {

            try
            {
                string path = "." + element.Name;
                XmlElement parentElement = null;

                if (element.ParentNode.NodeType != XmlNodeType.Document)
                    parentElement = (XmlElement)element.ParentNode;

                if (parentElement != null)
                {
                    // Gets the position within the parent element.             
                    // However, this position is irrelevant if the element is unique under its parent:             
                    XmlNodeList siblings = parentElement.SelectNodes(element.Name);
                    if (siblings != null && siblings.Count > 1) // There's more than 1 element with the same name             
                    {
                        int position = 1;
                        foreach (XmlElement sibling in siblings)
                        {
                            if (sibling == element)
                                break;
                            position++;
                        }
                        path = path + "[" + position + "]";
                    }

                    // Climbing up to the parent elements:             
                    path = (parentElement != null) ? GetPathNode(parentElement) + path : path;
                }



                return path;

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
        /// Controlla se una generica lista è vuota o nulla
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti contenuti nella lista</typeparam>
        /// <param name="list">La lista di oggetti</param>
        /// <returns>True, se la lista è vuota o nulla; False altrimenti</returns>
        public static bool IsVoid<TypeSource>(this List<TypeSource> list)
        {


            if (list == null)
                return true;

            if (list.Count == 0)
                return true;

            return false;

        }



        /// <summary>
        /// Controlla se una generico array è vuoto o nullo
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti contenuti nell'array</typeparam>
        /// <param name="array">L'array di oggetti</param>
        /// <returns>True, se l'array è vuoto o nullo; False altrimenti</returns>
        public static bool IsVoid<TypeSource>(this TypeSource[] array)
        {


            if (array == null)
                return true;

            if (array.Length == 0)
                return true;

            if (array.Length == 1 && string.IsNullOrEmpty(array[0].ToStringValue()))
                return true;

            return false;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        public static void Empty(this TextBox tb)
        {

            tb.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lb"></param>
        public static void Empty(this Label lb)
        {

            lb.Text = string.Empty;
        }


    


        /// <summary>
        /// Ricerca un controllo A all'interno di un controllo Z, in modo ricorsivo.
        /// </summary>
        /// <param name="rootControl">Il controllo padre Z</param>
        /// <param name="typeParameter"></param>
        /// <param name="idControl">L'id del controllo figlio A</param>
        /// <returns>Il riferimento al controllo figlio A</returns>
        public static Control FindControlRecursive(Control rootControl                                                 
                                                  , string idControl
                                                  , TypeParameterCollectionControls typeParameter = TypeParameterCollectionControls.ID)
        {

            // se il controllo è il controllo figlio allora lo ha trovato
            string searchParameterControl = string.Empty;
            Type typeControl = null;
            switch (typeParameter)
            {

                case TypeParameterCollectionControls.ID:
                    searchParameterControl = rootControl.ID;
                    break;

                case TypeParameterCollectionControls.CommandName:
                    typeControl = rootControl.GetType();
                    searchParameterControl = rootControl.ID;
                    if (typeControl == typeof(Button))
                        searchParameterControl = ((Button)rootControl).CommandName;
                    else if (typeControl == typeof(ImageButton))
                        searchParameterControl = ((ImageButton)rootControl).CommandName;
                    else if (typeControl == typeof(LinkButton))
                        searchParameterControl = ((LinkButton)rootControl).CommandName;
                    else
                        searchParameterControl = rootControl.ID;
                    break;

                case TypeParameterCollectionControls.CommandArgument:
                    typeControl = rootControl.GetType();
                    searchParameterControl = rootControl.ID;
                    if (typeControl == typeof(Button))
                        searchParameterControl = ((Button)rootControl).CommandArgument;
                    else if (typeControl == typeof(ImageButton))
                        searchParameterControl = ((ImageButton)rootControl).CommandArgument;
                    else if (typeControl == typeof(LinkButton))
                        searchParameterControl = ((LinkButton)rootControl).CommandArgument;
                    else
                        searchParameterControl = rootControl.ID;
                    break;

            }
            if (searchParameterControl == idControl)
                return rootControl;

            // ricerca in tutti i controlli figli del controllo padre
            foreach (Control control in rootControl.Controls)
            {
                // esegue la ricerca ricorsivamente: se un controllo figlio a sua volta contiene altri controlli
                // richiama la funzione di ricerca
                Control foundControl = FindControlRecursive(control, idControl, typeParameter);

                // se il controllo è trovato lo espone
                if (foundControl != null)
                    return foundControl;

            }

            // se esce naturalmente dal ciclo di ricerca vuol dire che il controllo figlio non è stato trovato
            return null;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootControl"></param>
        /// <param name="idControl"></param>
        /// <param name="typeParameter"></param>
        /// <param name="typeSearch"></param>
        /// <param name="rootControls"></param>
        /// <returns></returns>
        public static List<Control> CollectControlRecursive(Control rootControl
                                                         , string idControl
                                                         , TypeParameterCollectionControls typeParameter = TypeParameterCollectionControls.ID
                                                         , TypeSearchCollectionControls typeSearch = TypeSearchCollectionControls.StartWith
                                                         , List<Control> rootControls = null)
        {
          
            if (rootControls == null)
                rootControls = new List<Control>();

            // se il controllo è il controllo figlio allora lo ha trovato
            string searchParameterControl = string.Empty;
            Type typeControl = null;
            switch (typeParameter)
            {

                case TypeParameterCollectionControls.ID:
                    searchParameterControl = rootControl.ID;
                    break;

                case TypeParameterCollectionControls.CommandName:
                    typeControl = rootControl.GetType();
                    searchParameterControl = rootControl.ID;
                    if (typeControl == typeof(Button))
                        searchParameterControl = ((Button)rootControl).CommandName;
                    else if (typeControl == typeof(ImageButton))
                        searchParameterControl = ((ImageButton)rootControl).CommandName;
                    else if (typeControl == typeof(LinkButton))
                        searchParameterControl = ((LinkButton)rootControl).CommandName;
                    else
                        searchParameterControl = rootControl.ID;
                    break;

                case TypeParameterCollectionControls.CommandArgument:
                    typeControl = rootControl.GetType();
                    searchParameterControl = rootControl.ID;
                    if (typeControl == typeof(Button))
                        searchParameterControl = ((Button)rootControl).CommandArgument;
                    else if (typeControl == typeof(ImageButton))
                        searchParameterControl = ((ImageButton)rootControl).CommandArgument;
                    else if (typeControl == typeof(LinkButton))
                        searchParameterControl = ((LinkButton)rootControl).CommandArgument;
                    else
                        searchParameterControl = rootControl.ID;
                    break;

            }

           

            if (rootControl != null && rootControl.ID != null)
            {
                if (
                    (typeSearch == TypeSearchCollectionControls.StartWith && searchParameterControl.ToLower().StartsWith(idControl.ToLower()))
                     ||
                    (typeSearch == TypeSearchCollectionControls.EndWith && searchParameterControl.ToLower().EndsWith(idControl.ToLower()))
                     ||
                    (typeSearch == TypeSearchCollectionControls.Contains && searchParameterControl.ToLower().Contains(idControl.ToLower()))
                     ||
                    (typeSearch == TypeSearchCollectionControls.StartWith && searchParameterControl.ToLower() == idControl.ToLower())
                    )
                    rootControls.Add(rootControl);
            }

            // ricerca in tutti i controlli figli del controllo padre
            foreach (Control control in rootControl.Controls)
            {
                // esegue la ricerca ricorsivamente: se un controllo figlio a sua volta contiene altri controlli
                // richiama la funzione di ricerca
                CollectControlRecursive(control, idControl, typeParameter, typeSearch, rootControls);

            }

            return rootControls;

        }

        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetPostBackControlId(this Page page)
        {

            if (!page.IsPostBack)
                return string.Empty;


            Control control = null;
            Control foundControl = null;
            string controlId = string.Empty;

            // first we will check the "__EVENTTARGET" because, if post back made by the controls
            // which used "_doPostBack" function also available in Request.Form collection.
            string controlName = page.Request.Params["__EVENTTARGET"];
            if (!String.IsNullOrEmpty(controlName))
                control = page.FindControl(controlName);
            else
            {
                // if __EVENTTARGET is null, the control is a button type and we need to
                // iterate over the form collection to find it
                foreach (string ctl in page.Request.Form)
                {
                    // handle ImageButton they having an additional "quasi-property" 
                    // in their Id which identifies mouse x and y coordinates
                    if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        controlId = ctl.Substring(0, ctl.Length - 2);
                        foundControl = page.FindControl(controlId);
                    }
                    else
                        foundControl = page.FindControl(ctl);


                    if (!(foundControl is Button || foundControl is ImageButton || foundControl is LinkButton)) 
                        continue;

                    control = foundControl;
                    break;
                }
            }

            return control == null ? string.Empty : control.ID;
        }



        /// <summary>
        /// Ottiene la lista di oggetti <see cref="Month"/>
        /// </summary>
        /// <param name="minimumMonth">Il numero di mese inferiore</param>
        /// <param name="maximumMonth">Il numero di mese superiore</param>
        /// <param name="year">L'anno di riferimento</param>
        /// <returns>La lista di oggetti <see cref="Month"/></returns>
        public static List<Month> GetMonths(int minimumMonth, int maximumMonth, int? year = null)
        {

            try
            {

                // se il numero del mese non è valido
                if (minimumMonth <= 0 || maximumMonth > 12 || minimumMonth > maximumMonth)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("minimumMonth <= 0 || maximumMonth > 12 || minimumMonth > maximumMonth"), TypeError.Managed);
                    return default(List<Month>);
                }

                DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;


                List<Month> months = new List<Month>();
                for (int month = minimumMonth; month <= maximumMonth; month++)
                    months.Add(new Month
                    {
                        Number = month
                        ,
                        Name = dtfi.GetMonthName(month)
                        ,
                        NumberOfDays = (year != null) ? (int?)DateTime.DaysInMonth(year.Value, month) : null
                        ,
                        FirstDay = (year != null) ? dtfi.GetDayName(new DateTime(year.Value, month, 1).DayOfWeek) : null
                        ,
                        LastDay = (year != null) ? dtfi.GetDayName(new DateTime(year.Value, month, DateTime.DaysInMonth(year.Value, month)).DayOfWeek) : null
                    });



                return months;
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<Month>);

            }
            #endregion Manage Error

        }



        /// <summary>
        /// Ottiene la lista di oggetti <see cref="Day"/>
        /// </summary>
        /// <param name="minimumDay">Il numero del giorno inferiore</param>
        /// <param name="maximDay">Il numero del giorno superiore</param>
        /// <param name="month">Il numero del mese di riferimeno</param>
        /// <param name="year">L'anno di riferimento</param>
        /// <returns>La lista di oggetti <see cref="Day"/></returns>
        public static List<Day> GetDays(int minimumDay, int maximDay, int month, int year)
        {

            try
            {

                // se il numero del mese non è valido
                if (minimumDay <= 0 || maximDay > 31 || minimumDay > maximDay)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("minimumDay <= 0 || maximDay > 31 || minimumDay > maximDay"), TypeError.Managed);
                    return default(List<Day>);
                }


                DateTime datetime;
                int ordinalInWeek;
                DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;


                List<Day> days = new List<Day>();
                for (int day = minimumDay; day <= maximDay; day++)
                {

                    datetime = new DateTime(year, month, day);

                    ordinalInWeek = 0;
                    switch (datetime.DayOfWeek)
                    {

                        case DayOfWeek.Monday:
                            ordinalInWeek = 1;
                            break;

                        case DayOfWeek.Tuesday:
                            ordinalInWeek = 2;
                            break;

                        case DayOfWeek.Wednesday:
                            ordinalInWeek = 3;
                            break;

                        case DayOfWeek.Thursday:
                            ordinalInWeek = 4;
                            break;

                        case DayOfWeek.Friday:
                            ordinalInWeek = 5;
                            break;

                        case DayOfWeek.Saturday:
                            ordinalInWeek = 6;
                            break;

                        case DayOfWeek.Sunday:
                            ordinalInWeek = 7;
                            break;

                    }

                    days.Add(new Day
                    {

                        Name = dtfi.GetDayName(datetime.DayOfWeek)
                        ,
                        OrdinalInYear = datetime.DayOfYear
                        ,
                        OrdinalInMonth = day
                        ,
                        OrdinalInWeek = ordinalInWeek

                    });



                }


                return days;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return default(List<Day>);

            }
            #endregion Manage Error

        }



        /// <summary>
        /// Filtra i caratteri speciali da una stringa
        /// </summary>
        /// <param name="s">La string da filtrare</param>
        /// <param name="forSql">Indica se la stringa è da filtrare per utilizzo in una query/stored procedure</param>
        /// <returns>La stringa filtrata</returns>
        public static string FilterSpecialChars(string s, bool forSql = true)
        {

            if (string.IsNullOrEmpty(s))
                return s;


            if (forSql)
                return s.Replace("'", "''");


            return s.Replace("'", string.Empty)
                    .Replace("<", string.Empty)
                    .Replace(">", string.Empty);



        }


        /// <summary>
        /// Converte un'immagine in un vettore di byte
        /// </summary>
        /// <param name="image">Immagine da convertire</param>
        /// <param name="imageFormat">Formato dell'immagine</param>
        /// <returns>Il vettore di byte corrispondente all'immagine</returns>
        public static byte[] ToByteArray(this System.Drawing.Image image, ImageFormat imageFormat)
        {

            MemoryStream ms = new MemoryStream();

            image.Save(ms, imageFormat);

            return ms.ToArray();
            
        
        }


        /// <summary>
        /// Converte un vettore di byte in immagine
        /// </summary>
        /// <param name="byteArray">Il vettore di byte da convertire</param>
        /// <returns>L'immagine corrispondente al vettore di byte</returns>
        public static System.Drawing.Image ToImage(this byte[] byteArray)
        {

            MemoryStream ms = new MemoryStream(byteArray);

            return System.Drawing.Image.FromStream(ms);

        }



        /// <summary>
        /// Controlla il servizio nell'elenco dei servizi di Windows
        /// </summary>
        /// <param name="nameService">Il nome del servizio.</param>
        /// <returns>True, se il servizio è presente; False, se il servizio è assente</returns>
        public static bool CheckService(string nameService)
        {


            List<ServiceController> services = ServiceController.GetServices()
                                                                .Where(sc =>
                                                                        (
                                                                          sc.ServiceName.ToLower() == nameService.ToLower()
                                                                            ||
                                                                          (sc.ServiceName.ToLower().Contains(nameService.ToLower())
                                                                            &&
                                                                           sc.ServiceName.ToLower().Replace(nameService.ToLower(),string.Empty).Contains("mssql")
                                                                           )
                                                                        )
                                                                         &&
                                                                        sc.Status == ServiceControllerStatus.Running
                                                                       ).ToList();

            if (services.Count() > 0)
                return true;

            return false;
        }


        /// <summary>
        /// Identifica univocamente una stringa
        /// </summary>
        /// <param name="s">Stringa da identificare</param>
        /// <returns>Identificativo univoco della stringa</returns>
        public static string Identify(this string s)
        {

           return !s.IsVoid() ? s.ToLower().Trim().Replace(" ", string.Empty) : string.Empty;

           
        }


        /// <summary>
        /// Identifica univocamente un vettore di stringhe
        /// </summary>
        /// <param name="v">Vettore da identificare</param>
        /// <returns>Identificativo univoco del vettore</returns>
        public static string[] Identify(this string[] v)
        {
            if (!v.IsVoid())
                for (int index = 0; index < v.Length; index++)
                    v[index] = v[index].Identify();

            return v;

        }


        /// <summary>
        /// Identifica univocamente una lista di stringhe
        /// </summary>
        /// <param name="v">Lista da identificare</param>
        /// <returns>Identificativo univoco della lista</returns>
        public static List<string> Identify(this List<string> v)
        {
            if (!v.IsVoid())
                for (int index = 0; index < v.Count(); index++)
                    v[index] = v[index].Identify();

            return v;

        }



        /// <summary>
        /// Verifica se la string è nulla o vuota
        /// </summary>
        /// <param name="s">>Stringa da verificare</param>
        /// <returns></returns>
        public static bool IsVoid(this string s)
        {

            return string.IsNullOrEmpty(s);

        }


        /// <summary>
        /// Converte un oggetto generico nella sua rappresentazione in stringa
        /// </summary>
        /// <param name="obj">Oggetto da convertire</param>
        /// <returns>La rappresentazione in formato stringa</returns>
        public static string ToStringValue(this object obj)
        {

            return obj != null ? obj.ToString() : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lb"></param>
        /// <returns></returns>
        public static double MeasureText(this Label lb)
        {
                    
            
            Unit unit = Unit.Pixel(lb.Text.Length);

            return unit.Value;

         
        }



       


        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In(this int number, params int[] values)
        {
            bool check = false;
            foreach (long value in values)
            {

                if (value == number)
                {
                    check = true;
                    break;
                }

            }

            return check;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool NotIn(this int number, params int[] values)
        {
            bool check = true;
            foreach (long value in values)
            {

                if (value == number)
                {
                    check = false;
                    break;
                }

            }

            return check;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TypeEnumeration"></typeparam>
        /// <param name="typeEnumeration"></param>
        /// <param name="valuesEnumeration"></param>
        /// <returns></returns>
        public static bool NotIn<TypeEnumeration>(this TypeEnumeration typeEnumeration, params TypeEnumeration[] valuesEnumeration)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {

            bool check = true;
            object obj = null;
            Enum findEnum = null;

            if (!typeEnumeration.GetType().IsEnum)
            {
                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typeEnumeration is not Enum"), TypeError.Managed);
                return false;
            }


      
            foreach (TypeEnumeration valueEnumeration in valuesEnumeration)
            {

                obj = Enum.Parse(typeof(TypeEnumeration), valueEnumeration.ToString(), true);
                findEnum = (Enum)obj;

                if (findEnum.ToString() == typeEnumeration.ToString())
                {
                    check = false;
                    break;
                }

            }


            return check;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TypeEnumeration"></typeparam>
        /// <param name="typeEnumeration"></param>
        /// <param name="valuesEnumeration"></param>
        /// <returns></returns>
        public static bool In<TypeEnumeration>(this TypeEnumeration typeEnumeration, params TypeEnumeration[] valuesEnumeration)
        where TypeEnumeration : struct, IComparable, IConvertible, IFormattable
        {
            bool check = false;
            object obj = null;
            Enum findEnum = null;

            if (!typeEnumeration.GetType().IsEnum)
            {
                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("typeEnumeration is not Enum"), TypeError.Managed);
                return false;
            }



            foreach (TypeEnumeration valueEnumeration in valuesEnumeration)
            {

                obj = Enum.Parse(typeof(TypeEnumeration), valueEnumeration.ToString(), true);
                findEnum = (Enum)obj;

                if (findEnum.ToString() == typeEnumeration.ToString())
                {
                    check = true;
                    break;
                }

            }


            return check;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In(this long number, params long[] values)
        {
            bool check = false;
            foreach (long value in values)
            {

                if (value == number)
                {
                    check = true;
                    break;
                }

            }

            return check;
  
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool NotIn(this long number, params long[] values)
        {
            bool check = true;
            foreach (long value in values)
            {

                if (value == number)
                {
                    check = false;
                    break;
                }

            }

            return check;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_string"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool In(this string _string, params string[] values)
        {
            bool check = false;
            foreach (string value in values)
            {

                if (value.Identify() == _string.Identify())
                {
                    check = true;
                    break;
                }

            }

            return check;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_string"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool NotIn(this string _string, params string[] values)
        {
            bool check = true;
            foreach (string value in values)
            {

                if (value.Identify() == _string.Identify())
                {
                    check = false;
                    break;
                }

            }

            return check;

        }



        /// <summary>
        /// Permette la navigazione del Browser in base al numero di versione permesso
        /// </summary>
        /// <param name="browser">Oggetto browser (Spazio dei Nomi: HttpBrowserCapabilities)</param>
        /// <param name="browsersMinimumVersionAllow">Elenco delle versioni minime permesse (es. Internet Explorer:9|Google Chrome:61)</param>
        /// <param name="userAgent">UserAgent specificato dalla Request</param>
        /// <returns>True, se la navigazione con il browser è permessa; False, altrimenti</returns>
        public static bool AllowNavigation(this HttpBrowserCapabilities browser, string browsersMinimumVersionAllow, string userAgent)
        {

            // se non ci sono browser specificati nel filtro, allora sono ammessi alla navigazione tutti i browser
            if (browsersMinimumVersionAllow.IsVoid())
                return true;


            // inizializza le variabili necessarie
            string browserClient = null;
            string trident = null;
            int versionTrident = 0;
            int versionBrowserFromTrident = 0;
            Dictionary<string, int> browsersVersionAllow = new Dictionary<string, int>();


            // crea una lista di browser, con le relative versioni minime, a partire dalle quali è ammessa la navigazione
            string[] v_browsers = browsersMinimumVersionAllow.Split('|');
            foreach (string s_browser in v_browsers)
                browsersVersionAllow.Add(s_browser.Split(':')[0].Identify(), s_browser.Split(':')[1].ToInt().Value);

            // identifica il browser del client
            browserClient = browser.Browser.Identify();

            // corregge il nome del browser per Internet Explorer, uniformandolo alle ultime versioni
            if (browserClient == "ie" || browserClient == "msie")
                browserClient = "internetexplorer";

            // se l'elenco dei browser permessi non contiene il browser del client, non permette la navigazione
            if (!browsersVersionAllow.ContainsKey(browserClient))
                return false;


            // se il browser del client è Internet Explorer legge il Trident da UserAgent
            if (browserClient == "internetexplorer")
            {

                // recupera il Trident dall'UserAgent
                trident = userAgent.Identify()
                                   .Split(';')
                                   .ToList()
                                   .Find(ua => ua.Contains("trident"));

                // se il Trident non è trovato, non permette la navigazione (versione di Internet Explorer dalla 4 alla 7)
                if (trident == null)
                    return false;

                // recupera la versione del Trident e identifica la corrispettiva versione di Explorer
                versionTrident = trident.Split('/')[1]
                                        .Split('.')[0]
                                        .ToInt().Value;
                switch (versionTrident)
                {

                    // non esistono Trident con versioni precedenti alla 4

                    //  Trident/4.0 Internet Explorer 8
                    case 4:
                        versionBrowserFromTrident = 8;
                        break;

                    //  Trident/5.0 Internet Explorer 9 
                    case 5:
                        versionBrowserFromTrident = 9;
                        break;

                    //  Trident/6.0 Internet Explorer 10 
                    case 6:
                        versionBrowserFromTrident = 10;
                        break;

                    //  Trident/7.0 Internet Explorer 11
                    //  Trident/8.0 Internet Explorer 11 su Windows 10
                    case 7:
                    case 8:
                        versionBrowserFromTrident = 11;
                        break;
                
                }


                // se la versione browser (da Trident) è uguale o superiore a quella permessa, la navigazione è permessa;
                // altrimenti no
                return versionBrowserFromTrident >= browsersVersionAllow[browserClient];


            }

            // se la versione browser (da Major Versione) è uguale o superiore a quella permessa, la navigazione è permessa;
            // altrimenti no
            return browser.MajorVersion >= browsersVersionAllow[browserClient];


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string Formatted(this string s, params object[] parameters)
        {
            
            string concat = string.Format(s, parameters);

            return concat;
        }

        #endregion ----  PUBLIC  ----





        /// <summary>
        /// Ordinamento di una lista di oggetti generici, per valore o per testo
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti contenunti nella lista</typeparam>
        /// <param name="items">La lista di oggetti</param>
        /// <param name="order">L'ordine da impartire alla lista di oggetti</param>
        /// <param name="propertyText">Il nome della proprietà di testo degli oggetti</param>
        /// <param name="propertyValue">Il nome della proprietà di valore degli oggetti</param>
        /// <returns>La lista di oggetti ordinata</returns>
        internal static TypeSource[] OrderList<TypeSource>(TypeSource[] items, Order order, string propertyText, string propertyValue)
        {

           
            
            TypeSource[] itemsexceptfirst = null;
            int contItems = 0;


            // ordino gli item per etichetta_testo
            switch (order)
            {

                case Order.AscendentAllForText:
                    items = items.OrderBy(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    break;


                case Order.AscendentAllForValue:
                    items = items.OrderBy(item => item.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    break;


                case Order.DescendentAllForText:
                    items = items.OrderByDescending(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    break;


                case Order.DescendentAllForValue:
                    items = items.OrderByDescending(item => item.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    break;


                case Order.DescendentExceptFirstForText:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(items.Count() - 1).OrderByDescending(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    itemsexceptfirst.ToList().ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;


                case Order.DescendentExceptFirstForValue:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(items.Count() - 1).OrderByDescending(item => item.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    itemsexceptfirst.ToList().ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;


                case Order.AscendentExceptFirstForText:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(items.Count() - 1).OrderBy(item => item.GetType().GetProperty(propertyText, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    itemsexceptfirst.ToList().ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;


                case Order.AscendentExceptFirstForValue:
                    contItems = 0;
                    itemsexceptfirst = items.Skip(0).Take(items.Count() - 1).OrderBy(item => item.GetType().GetProperty(propertyValue, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(item, null)).ToArray();
                    itemsexceptfirst.ToList().ForEach(i =>
                    {
                        contItems++;
                        items[contItems] = i;

                    });
                    break;


            }


            return items;
        
        }


        #region Imports External DLLs


        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static uint FindMimeFromData(
            uint pBC,
            [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            uint cbSize,
            [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
            uint dwMimeFlags,
            out uint ppwzMimeOut,
            uint dwReserverd
        );


        #endregion Imports External DLLs






    }
}
