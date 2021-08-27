using System.Xml;
using System.Xml.XPath;



namespace mFFramework.Validations
{

    /// <summary>
    /// Classe astratta per le validazioni
    /// </summary>
    public abstract class Validator
    {


        private XmlDocument xmlDocument;
        private XmlNamespaceManager xmlNameSpace;
        private XPathNavigator xmlnavigator;

        private string targetNameSpace = "mF";


        /// <summary>
        /// Imposta un file xml
        /// </summary>
        /// <param name="xmlFile">Il percorso fisico del file xml</param>
        /// <param name="xsdNameSpace">Namespace del file xsd</param>
        protected void SetXML(string xmlFile, string xsdNameSpace)
        {

            // Imposta e carica il documento xml
            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlFile);


            // Imposta il namespace dello schema XSD, all'interno del file XML
            xmlNameSpace = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNameSpace.AddNamespace(targetNameSpace, xsdNameSpace);
            

            // Imposta il xmlnavigator del documento
            xmlnavigator = xmlDocument.CreateNavigator();

        }


      
        /// <summary>
        /// Namespace del file xml
        /// </summary>
        public XmlNamespaceManager XMLNamespace
        {

            get { return xmlNameSpace; }
        
        }


        /// <summary>
        /// Oggetto XmlDocument del file xml
        /// </summary>
        public XmlDocument XMLDocument
        {

            get { return xmlDocument; }

        }


        /// <summary>
        /// Oggetto XmlNavigator del file xml
        /// </summary>
        public XPathNavigator XMLNavigator
        {

            get { return xmlnavigator; }

        }


        /// <summary>
        /// TargetNamespace del file xml
        /// </summary>
        public string TargetNamespace
        {

            get { return "//" + targetNameSpace + ":"; }
        }


    }

}
