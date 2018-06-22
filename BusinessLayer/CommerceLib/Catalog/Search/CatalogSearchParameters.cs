using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Mediachase.Commerce.Shared;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Search
{
    /// <summary>
    /// Implements operations for and represents the catalog search paramters. (Inherits <see cref="SearchParameters"/>.)
    /// </summary>
    [DataContract]
    public sealed class CatalogSearchParameters : SearchParameters
    {
        private StringCollection _catalogNamesCollection;
        private StringCollection _nodesCollection;
        private string _language = String.Empty;

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogSearchParameters"/> class.
        /// </summary>
        public CatalogSearchParameters()
        {
            this._catalogNamesCollection = new StringCollection();
            this._nodesCollection = new StringCollection();
        }

        /// <summary>
        /// Gets the catalog names.
        /// </summary>
        /// <value>The catalog names.</value>
        public StringCollection CatalogNames
        {
            get
            {
                return this._catalogNamesCollection;
            }
        }

        /// <summary>
        /// Gets the catalog nodes.
        /// </summary>
        /// <value>The catalog nodes.</value>
        public StringCollection CatalogNodes
        {
            get
            {
                return this._nodesCollection;
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get
            {
                return this._language;
            }
            set
            {
                this._language = value;
            }
        }

        /// <summary>
        /// Gets the cache key. Used to generate hash that will be used to store data in memory if needed.
        /// </summary>
        /// <value>The cache key.</value>
        public override string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                key.Append("cs" + CommerceHelper.ConvertToString(this.CatalogNames, ","));
                key.Append("cn" + CommerceHelper.ConvertToString(this.CatalogNodes, ","));
                key.Append("l" + this.Language);

                return base.CacheKey + key.ToString();
            }
        }
    }
}
