using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using mFFramework.Types;
using mFFramework.Utilities;
using System;
using mFFramework.LogManager;
using System.Reflection;



namespace mFFramework.Validations.XSD
{

    /// <summary>
    /// Validatore XSD di un file XML
    /// </summary>
    public class XSDValidator : Validator
    {

        private XmlReaderSettings xsdSettings;
        private XmlReader xmlReader;
        private XmlNode node;

        private SerializerManager serializermanager;

        private string value;
        private string nodeParent;
        private string nodeFather;



        /// <summary>
        /// La lista di errori nel file xml dovuti alla validazione xsd
        /// </summary>
        public List<XSDError> XSDErrors { get; set; }



        /// <summary>
        /// Costruttore del Validatore XSD
        /// </summary>
        /// <param name="xmlFile">Il path fisico del file xml</param>
        /// <param name="xsdFile">Il path fisico dello schema xsd</param>
        /// <param name="xsdNameSpace">Il namespace dello schema xsd all'interno del file xml</param>
        public XSDValidator(string xmlFile, string xsdFile, string xsdNameSpace)
        {


            base.SetXML(xmlFile, xsdNameSpace);

            serializermanager = new SerializerManager();
            XSDErrors = new List<XSDError>();

            xsdSettings = new XmlReaderSettings();
            xsdSettings.ValidationType = ValidationType.Schema;
            xsdSettings.Schemas.Add(null, xsdFile);
            xsdSettings.IgnoreComments = true;
            xsdSettings.IgnoreWhitespace = true;
            xsdSettings.ValidationEventHandler += new ValidationEventHandler(Validation);

            // lettore xml
            xmlReader = XmlReader.Create(serializermanager.XmlToMemoryStream(xmlFile), xsdSettings);


        }



        /// <summary>
        /// Funzione delegata per la validazione xsd
        /// </summary>
        /// <param name="sender">Oggetto sende</param>
        /// <param name="e">Argomenti di validazione</param>
        private void Validation(object sender, ValidationEventArgs e)
        {


            if (xmlReader.NodeType != XmlNodeType.Attribute)
                node = XMLDocument.SelectSingleNode(TargetNamespace + xmlReader.Name + "[text()='" + value + "']", XMLNamespace);
            else if (xmlReader.NodeType == XmlNodeType.Attribute)
                node = XMLDocument.SelectSingleNode(TargetNamespace + nodeParent + "[@" + xmlReader.Name + "='" + xmlReader.Value + "']", XMLNamespace);

            // riempie la lista di errori
            XSDErrors.Add(new XSDError
            {
                Description = e.Exception.Message
            ,
                Line = e.Exception.LineNumber
            ,
                TagXML = (node != null) ? Functions.GetPathNode((XmlElement)node) : string.Empty
            ,
                Code = TypeError.XSD.ToString()
            ,
                Validation = TypeError.XSD.GetDescription()

            });



        }



        /// <summary>
        /// Validazione di un file xml secondo lo schema xsd
        /// </summary>
        /// <returns>True, se il file xml rispetta le regole dello schema xsd; False altrimenti</returns>
        public bool Valid()
        {

            try
            {

                while (xmlReader.Read())
                {

                    nodeFather = xmlReader.Name;

                    if (xmlReader.HasAttributes)
                        nodeParent = xmlReader.Name;

                    value = xmlReader.Value;
                };

                return (XSDErrors.Count > 0) ? false : true;

            }
            #region Manage Error
            catch (Exception ex)
            {

                Logger.Istance.Write(Assembly.GetExecutingAssembly(), MethodBase.GetCurrentMethod(), ex);
                return false;

            }
            #endregion Manage Error


        }
    }

}