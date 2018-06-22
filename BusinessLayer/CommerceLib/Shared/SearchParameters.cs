using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Search parameters that can be specified to control what SQL Command is executed on the server.
    /// </summary>
    [DataContract]
    public abstract class SearchParameters
    {
        // Fields
        private string _advancedFreeTextSearchPhrase = String.Empty;
        private string _freeTextSearchPhrase = String.Empty;
        private string _sqlWhereClause = String.Empty;
        private string _sqlMetaWhereClause = String.Empty;
        private string _OrderByClause = String.Empty;
        private string _JoinType = String.Empty;
        private string _JoinSourceTable = String.Empty;
        private string _JoinTargetTable = String.Empty;
        private string _JoinSourceTableKey = String.Empty;
        private string _JoinTargetTableKey = String.Empty;


        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchParameters"/> class.
        /// </summary>
        public SearchParameters()
        {
        }

        // Properties
        /// <summary>
        /// Gets or sets the type of the join used in the query. 
        /// The example types are: inner join, outer join, left join and right join.
        /// </summary>
        /// <value>The type of the join.</value>
        public string JoinType
        {
            get { return _JoinType; }
            set { _JoinType = value; }
        }

        /// <summary>
        /// Gets or sets the join source table. This value has to be a table name that exists in original
        /// query. For example it will be CatalogEntry for the Catalog Entry Search or CatalogNode for the Catalog
        /// Node Search.
        /// </summary>
        /// <value>The join source table.</value>
        public string JoinSourceTable
        {
            get { return _JoinSourceTable; }
            set { _JoinSourceTable = value; }
        }

        /// <summary>
        /// Gets or sets the join target query. This can either be a table name or a complete query. For exammple
        /// the table name can be something like CatalogEntryEx for the Catalog Entry search. This particular join will allow
        /// sorting by meta fields. This is not possible without a join. In some cases the target table can contain multiple 
        /// records for the same record in source table. That will cause problems with constraints that are inforces by the
        /// search DTOs. In order to overcome this, you can specify the query, so if our CatalogEntryEx table had multiple
        /// records for one CatalogEntry, we would join it using the follow query:
        /// (select distinct ObjectId, DisplayName from CatalogEntryEx) CatalogEntryEx
        /// </summary>
        /// <value>The join target table.</value>
        public string JoinTargetQuery
        {
            get { return _JoinTargetTable; }
            set { _JoinTargetTable = value; }
        }

        /// <summary>
        /// Gets or sets the join source table key.
        /// </summary>
        /// <value>The join source table key.</value>
        public string JoinSourceTableKey
        {
            get { return _JoinSourceTableKey; }
            set { _JoinSourceTableKey = value; }
        }

        /// <summary>
        /// Gets or sets the join target table key.
        /// </summary>
        /// <value>The join target table key.</value>
        public string JoinTargetTableKey
        {
            get { return _JoinTargetTableKey; }
            set { _JoinTargetTableKey = value; }
        }

        /// <summary>
        /// Gets or sets the advanced free text search phrase. You can use advanced keywords for Microsoft Full-Text search engine.
        /// </summary>
        /// <value>The advanced free text search phrase.</value>
        public string AdvancedFreeTextSearchPhrase
        {
            get
            {
                return this._advancedFreeTextSearchPhrase;
            }
            set
            {
                this._advancedFreeTextSearchPhrase = value;
            }
        }

        /// <summary>
        /// Gets or sets the free text search phrase.
        /// </summary>
        /// <value>The free text search phrase.</value>
        public string FreeTextSearchPhrase
        {
            get
            {
                return this._freeTextSearchPhrase;
            }
            set
            {
                this._freeTextSearchPhrase = value;
            }
        }

        /// <summary>
        /// Gets or sets the SQL where clause. You can specify filter conditions against the tables returned by the query.
        /// </summary>
        /// <value>The SQL where clause.</value>
        public string SqlWhereClause
        {
            get
            {
                return this._sqlWhereClause;
            }
            set
            {
                this._sqlWhereClause = value;
            }
        }

        /// <summary>
        /// Gets or sets the SQL meta where clause. You can specify filters for meta tables. Make sure to specifiy columns that
        /// exist for items searched.
        /// </summary>
        /// <value>The SQL meta where clause.</value>
        public string SqlMetaWhereClause
        {
            get
            {
                return this._sqlMetaWhereClause;
            }
            set
            {
                this._sqlMetaWhereClause = value;
            }
        }

        /// <summary>
        /// Gets or sets the order by clause.
        /// </summary>
        /// <value>The order by clause.</value>
        public string OrderByClause
        {
            get
            {
                return this._OrderByClause;
            }
            set
            {
                this._OrderByClause = value;
            }
        }

        /// <summary>
        /// Gets the cache key. Used to generate hash that will be used to store data in memory if needed.
        /// </summary>
        /// <value>The cache key.</value>
        public virtual string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                key.Append("ad" + AdvancedFreeTextSearchPhrase);
                key.Append("fts" + FreeTextSearchPhrase);
                key.Append("sqlw" + SqlWhereClause);
                key.Append("sqlmw" + SqlMetaWhereClause);
                key.Append("fts" + FreeTextSearchPhrase);
                key.Append("obc" + this.OrderByClause);

                return key.ToString();
            }
        }
    }
}
