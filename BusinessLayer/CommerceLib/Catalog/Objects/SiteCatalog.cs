using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the site catalog.
    /// </summary>
    [DataContract]
    public class SiteCatalog
    {
        /// <summary>
        /// Represents the catalog ID.
        /// </summary>
        private string _CatalogId;

        public string CatalogId
        {
            get { return _CatalogId; }
            set { _CatalogId = value; }
        }

        /// <summary>
        /// Represents the catalog name.
        /// </summary>
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}
