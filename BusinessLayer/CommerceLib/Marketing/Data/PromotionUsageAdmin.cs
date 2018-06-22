using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using System.Data.Common;

namespace Mediachase.Commerce.Marketing.Data
{
    /// <summary>
    /// Contains all the functions needed to perform administration on the Promotion Usage Table
    /// </summary>
    public class PromotionUsageAdmin
    {
        private PromotionUsageDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public PromotionUsageDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        /// <value>The mapping.</value>
        private DataTableMapping[] Mapping
        {
            get
            {
                return DataHelper.MapTables("PromotionUsage");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionAdmin"/> class.
        /// </summary>
        internal PromotionUsageAdmin()
        {
            _DataSet = new PromotionUsageDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionUsageAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal PromotionUsageAdmin(PromotionUsageDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the Promotion.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "PromotionUsage");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified Promotion id.
        /// </summary>
        /// <param name="PromotionId">The Promotion id.</param>
        internal void Load(int PromotionId, Guid customerId, int orderGroupId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_PromotionUsage");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PromotionId", PromotionId, DataParameterType.Int));

            if (customerId == Guid.Empty)
                cmd.Parameters.Add(new DataParameter("CustomerId", DataParameterType.UniqueIdentifier));
            else
                cmd.Parameters.Add(new DataParameter("CustomerId", customerId, DataParameterType.UniqueIdentifier));

            cmd.Parameters.Add(new DataParameter("OrderGroupId", orderGroupId, DataParameterType.Int));
            
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the statistics table. Pass Guid.Empty for customer id to load non customer specific statistics (gloabl).
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        internal DataTable LoadStatistics(Guid customerId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_PromotionUsageStatistics");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));

            if (customerId == Guid.Empty)
                cmd.Parameters.Add(new DataParameter("CustomerId", DataParameterType.UniqueIdentifier));
            else
                cmd.Parameters.Add(new DataParameter("CustomerId", customerId, DataParameterType.UniqueIdentifier));

            return DataService.LoadTable(cmd);
        }

    }
}