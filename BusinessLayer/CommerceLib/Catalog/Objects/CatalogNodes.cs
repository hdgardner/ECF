using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains collection of <see cref="CatalogNode"/> elements.
    /// </summary>
    [DataContract]
    public partial struct CatalogNodes
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CatalogNode")]
        public CatalogNode[] _CatalogNode;

        public CatalogNode[] CatalogNode
        {
            get { return _CatalogNode; }
            set { _CatalogNode = value; }
        }
    }
}
