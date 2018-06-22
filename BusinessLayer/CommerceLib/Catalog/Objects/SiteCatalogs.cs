using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the site catalogs.
    /// </summary>
    [DataContract]
    public class SiteCatalogs
    {
        /// <summary>
        /// Represents the collection of <see cref="SiteCatalog"/> objects.
        /// </summary>
        private SiteCatalog[] _Catalog;

        public SiteCatalog[] Catalog
        {
            get { return _Catalog; }
            set { _Catalog = value; }
        }
    }
}
