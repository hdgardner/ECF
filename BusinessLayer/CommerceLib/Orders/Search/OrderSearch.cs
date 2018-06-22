using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Orders;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Orders.Search
{
    /// <summary>
    /// Order Search object. Contains information about search parameters.
    /// </summary>
    public class OrderSearch
    {
        private OrderSearchOptions _orderSearchOptions = null;
        private OrderSearchParameters _orderSearchParameters = null;
        private OrderContext _Context = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSearch"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal OrderSearch(OrderContext context)
        {
            _Context = context;
        }

        /// <summary>
        /// Searches the specified search GUID.
        /// </summary>
        /// <param name="searchGuid">The search GUID.</param>
        /// <returns></returns>
        public int Search(Guid searchGuid)
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_OrderSearch");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
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
        public OrderSearchOptions SearchOptions
        {
            get
            {
                return this._orderSearchOptions;
            }
            set
            {
                this._orderSearchOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        public OrderSearchParameters SearchParameters
        {
            get 
            { 
                return _orderSearchParameters; 
            }
            set 
            { 
                _orderSearchParameters = value; 
            }
        }

    }
}
