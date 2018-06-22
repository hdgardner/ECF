using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.IO;
using System.Xml.XPath;
using System.Configuration.Provider;
using System.Xml;
using System.Runtime.Serialization;
using System.Globalization;

namespace Mediachase.Commerce.Engine.Template.Providers
{
    /// <summary>
    /// XSL Driven Template Provider.
    /// </summary>
    public class XslTemplateProvider : TemplateProvider
    {
        private string _ApplicationName;

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public override string ApplicationName
        {
            get
            {
                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        private string _TemplateSource;

        /// <summary>
        /// Gets or sets the template source. The source should be of type
        /// c:\mysite\templates\{0}\{1}.xsl where {0} will be either replaced with specific language
        /// or default and {1} will be replace with a template name specified.
        /// </summary>
        /// <value>The template source.</value>
        public string TemplateSource
        {
            get
            {
                return _TemplateSource;
            }
            set
            {
                _TemplateSource = value;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"></see> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        public override void Initialize(string name,
            NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "XslTemplateProvider";

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Initialize _applicationName
            _ApplicationName = config["applicationName"];

            if (string.IsNullOrEmpty(_ApplicationName))
                _ApplicationName = "/";

            config.Remove("applicationName");

            // Initialize _TemplateSource
            _TemplateSource = config["templateSource"];

            if (string.IsNullOrEmpty(_TemplateSource))
            {
                throw new ProviderException("templateSource is a required element for XslTemplateProvider");
            }
            config.Remove("templateSource");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

        }


        /// <summary>
        /// Processes the specified template with a context provided.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The processed template with context embedded.
        /// </returns>
        public override string Process(string template, CultureInfo culture, System.Collections.IDictionary context)
        {
            // 1. Serialize all context variables into XML            
            MemoryStream stream = new MemoryStream();

            // Start creating xml document
            XmlWriterSettings xmlFileSettings = new XmlWriterSettings();
            xmlFileSettings.Indent = true;
            XmlWriter exportWriter = XmlWriter.Create(stream, xmlFileSettings);

            // Start the Xml Document
            exportWriter.WriteStartDocument();

            exportWriter.WriteStartElement("ContextDoc", "");

            // Cycle through dictionary

            foreach (string key in context.Keys)
            {
                object contextObject = context[key];
                XmlSerializer serializer = new XmlSerializer(contextObject.GetType());
                serializer.Serialize(exportWriter, contextObject);
            }

            exportWriter.WriteEndElement(); // End of ContextDoc
            exportWriter.WriteEndDocument();
            exportWriter.Close(); // Close the XmlWriter Stream

            stream.Position = 0;

            /*
            string xml = new StreamReader(stream).ReadToEnd();

            stream.Position = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
             * */

            // 2. Locate XSL Template
            XslCompiledTransform xslt = new XslCompiledTransform();

            string path = String.Format(TemplateSource, culture.Name, template);
            string specificPath = String.Empty;
            // Check default path if language specific one doesn't exist
            if (!File.Exists(path))
            {
                specificPath = path;
                path = String.Format(TemplateSource, "Default", template);
            }

            // Generate exception if path doesn't exist
            if (!File.Exists(path))
            {
                throw new ProviderException(String.Format("The template was not found at the default path \"{0}\" nor at the specific path \"{1}\". Please either modify settings in web.config for XSL Provider or create an xsl template in the path specified.", path, specificPath));
            }

            // Load otherwise
            xslt.Load(path);

            // 3. Transform Content
            stream.Position = 0;
            XPathDocument pathDoc = new XPathDocument(stream);
            MemoryStream outputStream = new MemoryStream();
            StringWriter writer = new StringWriter();
            xslt.Transform(pathDoc, null, writer);

            // Return contents
            return writer.ToString();
        }
    }
}
