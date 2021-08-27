using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using mFFramework.LogManager;
using mFFramework.Types;
using mFFramework.Utilities;


namespace mFFramework.Extensions.WebControls
{

    /// <summary>
    /// Metodi Estesi per il WebControl GridView
    /// </summary>
    public static class GridViewExtensions
    {
        


        #region ----  PRIVATE  ----

        /// <summary>
        /// Verifica la congruenza dei campi collegati alla GridView con le proprietà dell'oggetto generico
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti associata nella GridView</typeparam>
        /// <param name="boundfields">Campi della GridView</param>
        /// <param name="typeSources">Lista di oggetti da collegare alla GridView</param>
        /// <returns>Una stringa NON vuota se c'è un campo incongruente, una stringa vuota se NON esiste incongruenza</returns>
        private static string CheckCongruency<TypeSource>(ref List<BoundField> boundfields, ref List<TypeSource> typeSources)
        {



            TypeSource typeSource = default(TypeSource);
            string incongruency = null;

            // se il datasource è vuoto, non c'è incongruenza nei dati
            if (typeSources.IsVoid())
                return null;

            typeSource = typeSources[0];

            boundfields.ForEach(bf =>
            {

                if (incongruency == null)
                    if (typeSource.GetType().GetProperty(bf.DataField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) == null)
                        incongruency = bf.DataField;
                        
            });


            return incongruency;
        }


        #endregion ----  PRIVATE  ----


        #region ----  PUBLIC  ----


        /// <summary>
        /// Carica gli elementi in una GridView
        /// </summary>
        /// <typeparam name="TypeSource">La tipologia di oggetti presente nella GridView</typeparam>
        /// <param name="gv">GridView da caricare</param>
        /// <param name="datasource">La lista di elementi da caricare nella GridView</param>
        /// <param name="emptyText">Il testo che compare quando la GridView è vuota</param>
        /// <param name="pagination">Il numero di righe per pagina</param>
        /// <param name="captionText">Il testo che compare quale intestazione della GridView</param>
        /// <param name="sortExpression">Il nome del campo dati della GridView per il quale ordinarla</param>
        /// <returns>Il numero di righe presenti nella GridView</returns>
        public static int Bind<TypeSource>(this GridView gv, List<TypeSource> datasource, string emptyText = null, string captionText = null, int? pagination = null, string sortExpression = null)
        {

            try
            {

                BoundField boundField = null;
                List<BoundField> boundFields = null;
                string incongruency = null;

               

                // se il datasource è vuoto e non è stato specificato alcun testo vuoto, allora la GridView non viene visualizzata
                if (datasource.IsVoid() && string.IsNullOrEmpty(emptyText))
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("datasource is null or empty and emptyText is null or empty"), TypeError.Managed);
                    gv.DataSource = null;
                    gv.DataBind();
                    return Int32.MinValue;
                }

                // recupero la lista di campi collegati alla gridview
                boundFields = new List<BoundField>();
                foreach (DataControlField dataControlField in gv.Columns)
                {

                    if (dataControlField.GetType() == typeof(BoundField))
                        boundFields.Add((BoundField)dataControlField);

                }


                // verifica la congruenza tra la GridView e l'oggetto da collegare
                incongruency = CheckCongruency<TypeSource>(ref boundFields, ref datasource);


                if (incongruency == null)
                {

                    if (!string.IsNullOrEmpty(sortExpression))
                    {

                          

                            #region  ----  Si richiede l'ordinamento della griglia  ----

                            gv.AllowSorting = true;

                            // si imposta il primo numero di pagina
                            if (gv.Attributes["CurrentPageIndex"] == null)
                                gv.Attributes["CurrentPageIndex"] = "0";

                            // se l'ordine attuale della GridView, per campo sortExpression, è discendente (o assente), si ordina per crescente
                            if (gv.Attributes["CurrentSortDirection"] == null || gv.Attributes["CurrentSortDirection"] == Order.DescendentAllForText.ToString())
                            {
                                if (Convert.ToInt32(gv.Attributes["CurrentPageIndex"]) == gv.PageIndex)
                                {
                                    // si ordina crescente e si memorizza lo stato del nuovo ordine, non c'è stato cambio pagina
                                    datasource = datasource.OrderBy(ts => ts.GetType().GetProperty(sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(ts, null)).ToList();
                                    gv.Attributes["CurrentSortDirection"] = Order.AscendentAllForText.ToString();
                                }
                                else
                                {
                                    // si mantiene l'ordine decrescente, c'è stato cambio pagina
                                    datasource = datasource.OrderByDescending(ts => ts.GetType().GetProperty(sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(ts, null)).ToList();
                                    gv.Attributes["CurrentPageIndex"] = gv.PageIndex.ToString();
                                }

                            }
                            // se l'ordine attuale della GridView, per campo sortExpression, è ascendente, si ordina per decrescente
                            else if (gv.Attributes["CurrentSortDirection"] == Order.AscendentAllForText.ToString())
                            {
                                if (Convert.ToInt32(gv.Attributes["CurrentPageIndex"]) == gv.PageIndex)
                                {
                                    // si ordina decrescente e si memorizza lo stato del nuovo ordine, non c'è stato cambio pagina
                                    datasource = datasource.OrderByDescending(ts => ts.GetType().GetProperty(sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(ts, null)).ToList();
                                    gv.Attributes["CurrentSortDirection"] = Order.DescendentAllForText.ToString();
                                }
                                else
                                {
                                    // si mantiene l'ordine crescente, c'è stato cambio pagina
                                    datasource = datasource.OrderBy(ts => ts.GetType().GetProperty(sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).GetValue(ts, null)).ToList();
                                    gv.Attributes["CurrentPageIndex"] = gv.PageIndex.ToString();
                                }
                               
                                    
                            }



                            foreach (DataControlField dataControlField in gv.Columns)
                            {

                                // si rimuovono le icone di ordinamento da tutte le intestazioni di colonna
                                dataControlField.HeaderText = dataControlField.HeaderText.Replace(" ▲", string.Empty).Replace(" ▼", string.Empty);


                                // si recupera il relativo BoundField, campo collegato della GridView
                                if (dataControlField.GetType() == typeof(BoundField))
                                {
                                    boundField = (BoundField)dataControlField;


                                    if (boundField.DataField.Trim().ToLower() == sortExpression.Trim().ToLower())
                                    {
                                        // se il boundField è quello corrispondente al campo da ordinare, si impostano le icone di ordinamento
                                        if (gv.Attributes["CurrentSortDirection"] == Order.AscendentAllForText.ToString())
                                            dataControlField.HeaderText = dataControlField.HeaderText + " ▲";
                                        else
                                            dataControlField.HeaderText = dataControlField.HeaderText + " ▼";

                                    }
                                }


                            }


                            #endregion  ----  Si richiede l'ordinamento della griglia  ----
                        

                    } // sortexpression

                    
                    // si binda la GridView:
                    // 1. si imposta il numero di righe
                    // 2. si imposta l'intestazione
                    // 3. si imposta il testo di griglia vuota
                    // 4. si imposta la paginazione
                    // ---------------------------------------

                    // 1. Numero di righe
                    int numeroRighe = !datasource.IsVoid() ? datasource.Count : 0;
                    gv.Attributes["numberOfRows"] = numeroRighe.ToString();

                    // 2. Intestazione griglia
                    gv.Caption = !string.IsNullOrEmpty(captionText) && captionText.EndsWith(":nr") && numeroRighe  > 0
                                            ? captionText.Substring(0, captionText.Length - 3) + " : " + numeroRighe
                                            : !string.IsNullOrEmpty(captionText) && !captionText.EndsWith(":nr") && numeroRighe  > 0 
                                                    ? captionText
                                                    : string.Empty;
                    if (datasource.IsVoid())
                        gv.Caption = string.Empty;

                    // 3. Griglia vuota
                    gv.EmptyDataText = emptyText;

                    // 4. Paginazione
                    bool isTherePagination = pagination.HasValue && pagination.Value > 0 && numeroRighe > 0; // determina se è presente un valore di paginazione > 0
                    gv.AllowPaging = isTherePagination; // imposta la paginazione
                    int resto = 0; // serve a determinare l'ultima pagina
                    int numeroPagine = isTherePagination ? Math.DivRem(numeroRighe, pagination.Value, out resto) : 1; // determina il numero di pagine
                    if (resto > 0 && resto < pagination) numeroPagine++; // serve a determinare l'ultima pagina
                    gv.PageSize = isTherePagination ? pagination.Value : 1; // imposta il numero di righe per pagina
                    gv.PagerSettings.Mode = PagerButtons.NumericFirstLast; // imposta la paginazione numerica, comprensiva di prima e ultima pagina
                    gv.PagerSettings.FirstPageText = "1"; // imposta la prima pagina
                    gv.PagerSettings.LastPageText = isTherePagination ? numeroPagine.ToString() : "1"; // imposta l'ultima pagina
                    gv.DataSource = datasource;
                    gv.DataBind();

                }
                else
                {

                    // se viene rilevata una incongruenza la si espone nella riga vuota
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("Incongruenza con " + incongruency), TypeError.Managed);
                    gv.DataSource = null;
                    gv.EmptyDataRowStyle.BackColor = Color.Red;
                    gv.EmptyDataRowStyle.ForeColor = Color.Black;
                    gv.EmptyDataRowStyle.Font.Bold = true;
                    gv.EmptyDataText = "mFFramework: Incongruenza con " + incongruency;
                    gv.PageSize = 1;
                    gv.DataBind();
                
                }


                return (datasource.IsVoid()) ? 0 : datasource.Count;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return Int32.MinValue;

            }
            #endregion Manage Error

        }


        #endregion ----  PUBLIC  ----


    }



}
