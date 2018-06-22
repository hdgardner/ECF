using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Shared;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Search
{
    /*
    public enum CatalogNodeFilter
    {
        /// <summary>
        /// Returns all nodes if none are specified
        /// </summary>
        All = 0,
        /// <summary>
        /// Returns only explicitly specified nodes, if only catalog is specified,
        /// then it will return entries that are not associated with any nodes.
        /// </summary>
        Specified = 1
    }
     * */

    /// <summary>
    /// Implements operations for the catalog search options. (Inherits <see cref="SearchOptions"/>.)
    /// </summary>
    [DataContract]
    public class CatalogSearchOptions : SearchOptions
    {
        /*
        CatalogNodeFilter _NodeFilter = CatalogNodeFilter.All;
        public CatalogNodeFilter NodeFilter
        {
            get
            {
                return _NodeFilter;
            }
            set
            {
                _NodeFilter = value;
            }
        }
         * */

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogSearchOptions"/> class.
        /// </summary>
        public CatalogSearchOptions() : base()
        {
            Namespace = "Mediachase.Commerce.Catalog";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogSearchOptions"/> class.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        public CatalogSearchOptions(CatalogSearchOptions searchOptions)
            : base(searchOptions)
        {
            Namespace = "Mediachase.Commerce.Catalog";
        }

        /*
        public override string CacheKey
        {
            get
            {                
                return base.CacheKey + "nf" + this.NodeFilter.ToString();
            }
        } 
         * */
    }
}
