using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using mFFramework.LogManager;
using System.Collections.Generic;
using mFFramework.Types;
using System.Linq;

namespace mFFramework.Utilities
{

    /// <summary>
    /// Gestore delle serializzazioni
    /// </summary>
    public class SerializerManager
    {

        private StringWriter stringstream;
        private XmlSerializer xml_serializer;

        private string string_serializer;
        private byte[] buffer;



        #region ----  PUBLIC  ----


        /// <summary>
        /// Costruttore del serializzatore
        /// </summary>
        public SerializerManager()
        {

            stringstream = new StringWriter();
        
        }



        /// <summary>
        /// Serializza un generico oggetto
        /// </summary>
        /// <typeparam name="TypeSource">Tipologia dell'oggetto</typeparam>
        /// <param name="typeSource">Oggetto da serializzare</param>
        /// <returns>La stringa dell'oggetto serializzato</returns>
        public StringWriter Serialize<TypeSource>(TypeSource typeSource)
        {
            try
            {
                xml_serializer = new XmlSerializer(typeof(TypeSource));
               
                xml_serializer.Serialize(stringstream, typeSource);

                return stringstream;
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
        /// Serializza una lista in una stringa, tramite una delle proprietà dell'oggetto listato
        /// </summary>
        /// <typeparam name="TypeSource">Tipologia dell'oggetto</typeparam>
        /// <param name="typeSources">Lista dell'oggetto da serializzare</param>
        /// <param name="property">Proprietà dell'oggetto tramite la quale serializzare la lista</param>
        /// <returns>La stringa di valori della proprietà dell'oggetto, separati dal pipe |</returns>
        public string SerializeToString<TypeSource>(List<TypeSource> typeSources, string property = null)
        {


            try
            {

                // se l'elenco è vuoto
                if (typeSources.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("La lista passata è vuota"), TypeError.Managed);
                    return null;
                }


                // se non esiste la proprietà
                if (!property.IsVoid() && typeSources[0].GetType().GetProperty(property) == null)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("Proprietà " + property + " non trovata nell'oggetto passato in lista"), TypeError.Managed);
                    return null;
                }

                string_serializer = string.Empty;
                if (!property.IsVoid())
                    typeSources.ForEach(ts => string_serializer += ts.GetType().GetProperty(property).GetValue(ts, null).ToString() + "|");
                else
                    typeSources.ForEach(ts => string_serializer += ts.ToString() + "|");

                string_serializer = string_serializer.Length > 0
                                          ? string_serializer.Remove(string_serializer.Length - 1, 1)
                                          : string_serializer;


                return string_serializer;

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
        /// 
        /// </summary>
        /// <typeparam name="TypeSource"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<TypeSource> DeserializeFromString<TypeSource>(string s)
        {


            try
            {

                // se l'elenco è vuoto
                if (s.IsVoid())
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("La lista passata è vuota"), TypeError.Managed);
                    return null;
                }

              
                List<TypeSource> ll = new List<TypeSource>();
                s.Split('|').ToList().ForEach(c =>

                    {
                        //TypeSource ts = (TypeSource)Convert.ChangeType(c, typeof(TypeSource));
                        ll.Add((TypeSource)Convert.ChangeType(c, typeof(TypeSource)));
                    
                    });

               


                return ll;

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
        /// 
        /// </summary>
        /// <typeparam name="TypeSource1"></typeparam>
        /// <typeparam name="TypeSource2"></typeparam>
        /// <param name="typeSources"></param>
        /// <returns></returns>
        public string SerializeToString<TypeSource1, TypeSource2>(Dictionary<TypeSource1, TypeSource2> typeSources)
        {


            try
            {

                // se il dizionario è vuoto
                if (typeSources.Count == 0)
                {
                    Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), new Exception("La lista passata è vuota"), TypeError.Managed);
                    return null;
                }


                string_serializer = string.Empty;
                typeSources.ToList().ForEach(ts => string_serializer += ts.Key.ToString() + ":" + ts.Value.ToString() + "|");
               
                string_serializer = string_serializer.Length > 0
                                          ? string_serializer.Remove(string_serializer.Length - 1, 1)
                                          : string_serializer;


                return string_serializer;

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
        /// Deserializza un generico oggetto
        /// </summary>
        /// <typeparam name="TypeSource">Tipologia dell'oggetto</typeparam>
        /// <param name="stream">Flusso dell'oggetto da deserializzare</param>
        /// <returns>Oggetto deserializzato</returns>
        public TypeSource Deserialize<TypeSource>(Stream stream)
        {
            try
            {

                xml_serializer = new XmlSerializer(typeof(TypeSource));

                return (TypeSource)xml_serializer.Deserialize(stream);
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
        /// Converte un file XML in un MemoryStream
        /// </summary>
        /// <param name="xmlFile">Percorso completo del file XML</param>
        /// <returns>L'array di byte rappresentante il file XML</returns>
        public MemoryStream XmlToMemoryStream(string xmlFile)
        {
            try
            {

                buffer = Encoding.UTF8.GetBytes(xmlFile.Replace(@"\", ""));
                return new MemoryStream(buffer);
            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return null;

            }
            #endregion Manage Error
        }


        #endregion ----  PUBLIC  ----

    }
}
