using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Catalog.Search
{
    /// <summary>
    /// Implements operations for and represents the catalog search.
    /// </summary>
    public class CatalogSearch
    {
        private CatalogSearchOptions _catalogSearchOptions;
        private CatalogSearchParameters _catalogSearchParameters;
        //private ICatalogSystem _Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogSearch"/> class.
        /// </summary>
        internal CatalogSearch()//ICatalogSystem context)
        {
            //_Context = context;
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <param name="searchGuid">The search GUID.</param>
        /// <returns></returns>
        public int SearchEntries(Guid searchGuid)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntrySearch");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Language", SearchParameters.Language, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("Catalogs", CommerceHelper.ConvertToString(SearchParameters.CatalogNames, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("CatalogNodes", CommerceHelper.ConvertToString(SearchParameters.CatalogNodes, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("SQLClause", SearchParameters.SqlWhereClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("MetaSQLClause", SearchParameters.SqlMetaWhereClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("FTSPhrase", SearchParameters.FreeTextSearchPhrase, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("AdvancedFTSPhrase", SearchParameters.AdvancedFreeTextSearchPhrase, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("OrderBy", SearchParameters.OrderByClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("Namespace", SearchOptions.Namespace, DataParameterType.NVarChar, 1024));
            cmd.Parameters.Add(new DataParameter("Classes", CommerceHelper.ConvertToString(SearchOptions.Classes, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("StartingRec", SearchOptions.StartingRecord, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("NumRecords", SearchOptions.RecordsToRetrieve, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("JoinType", SearchParameters.JoinType, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("SourceTableName", SearchParameters.JoinSourceTable, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("TargetQuery", SearchParameters.JoinTargetQuery, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("SourceJoinKey", SearchParameters.JoinSourceTableKey, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("TargetJoinKey", SearchParameters.JoinTargetTableKey, DataParameterType.NVarChar));

            DataParameter param = new DataParameter("RecordCount", DataParameterType.Int);
            param.Direction = ParameterDirection.InputOutput;
            param.Value = 0;
            cmd.Parameters.Add(param);
            cmd.Parameters.Add(new DataParameter("ReturnTotalCount", SearchOptions.ReturnTotalCount, DataParameterType.Bit));
            //cmd.Parameters.Add(new DataParameter("ReturnNodeFilter", Enum.GetName(typeof(CatalogNodeFilter), SearchOptions.NodeFilter), DataParameterType.NVarChar, 50));

            DataService.ExecuteNonExec(cmd);

            return Int32.Parse(cmd.Parameters["RecordCount"].Value.ToString());
        }

        /// <summary>
        /// Searches the nodes.
        /// </summary>
        /// <param name="searchGuid">The search GUID.</param>
        /// <returns></returns>
        public int SearchNodes(Guid searchGuid)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNodeSearch");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Language", SearchParameters.Language, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("Catalogs", CommerceHelper.ConvertToString(SearchParameters.CatalogNames, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("CatalogNodes", CommerceHelper.ConvertToString(SearchParameters.CatalogNodes, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("SQLClause", SearchParameters.SqlWhereClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("MetaSQLClause", SearchParameters.SqlMetaWhereClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("FTSPhrase", SearchParameters.FreeTextSearchPhrase, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("AdvancedFTSPhrase", SearchParameters.AdvancedFreeTextSearchPhrase, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("OrderBy", SearchParameters.OrderByClause, DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("Namespace", SearchOptions.Namespace, DataParameterType.NVarChar, 1024));
            cmd.Parameters.Add(new DataParameter("Classes", CommerceHelper.ConvertToString(SearchOptions.Classes, ","), DataParameterType.NVarChar));
            cmd.Parameters.Add(new DataParameter("StartingRec", SearchOptions.StartingRecord, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("NumRecords", SearchOptions.RecordsToRetrieve, DataParameterType.Int));
            DataParameter param = new DataParameter("RecordCount", DataParameterType.Int);
            param.Direction = ParameterDirection.InputOutput;
            param.Value = 0;
            cmd.Parameters.Add(param);

            DataService.ExecuteNonExec(cmd);

            return Int32.Parse(cmd.Parameters["RecordCount"].Value.ToString());
        }

        /// <summary>
        /// Gets or sets the search options.
        /// </summary>
        /// <value>The search options.</value>
        public CatalogSearchOptions SearchOptions
        {
            get
            {
                return this._catalogSearchOptions;
            }
            set
            {
                this._catalogSearchOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        public CatalogSearchParameters SearchParameters
        {
            get 
            { 
                return _catalogSearchParameters; 
            }
            set 
            { 
                _catalogSearchParameters = value; 
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public string CacheKey
        {
            get
            {
                return this.SearchOptions.CacheKey + this.SearchParameters.CacheKey;
            }
        }
    }
}
