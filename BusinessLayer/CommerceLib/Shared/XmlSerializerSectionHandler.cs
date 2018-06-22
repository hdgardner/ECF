using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements the handler for the XML serializer section. (Inherits <see cref="IConfigurationSectionHandler"/>.)
    /// </summary>
    public class XmlSerializerSectionHandler :
         IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public virtual object Create(
             object parent,
             object configContext,
             System.Xml.XmlNode section)
        {
            XPathNavigator nav = section.CreateNavigator();
            string typename = (string)nav.Evaluate("string(@type)");
            return CreateInternal(parent, configContext, section, typename);
        }

        /// <summary>
        /// Creates the internal.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="configContext">The config context.</param>
        /// <param name="section">The section.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        protected object CreateInternal(
             object parent,
             object configContext,
             System.Xml.XmlNode section,
            string typeName)
        {            
            Type t = Type.GetType(typeName);
            XmlSerializer ser = new XmlSerializer(t);
            return ser.Deserialize(new XmlNodeReader(section));
        }
    }
}
