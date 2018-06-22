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
    /// Contains all the functions needed to perform administration on the Campaign
    /// </summary>
    public class CampaignAdmin
    {
        private CampaignDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CampaignDto CurrentDto
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
                return DataHelper.MapTables("Campaign", "CampaignSegment");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignAdmin"/> class.
        /// </summary>
        internal CampaignAdmin()
        {
            _DataSet = new CampaignDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CampaignAdmin(CampaignDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the Campaign.
        /// </summary>
        internal void Save()
        {
            // TODO: Check if user is allowed to perform this operation
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "Campaign", "CampaignSegment");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified site GUID.
        /// </summary>
        internal void LoadAll()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Campaign");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CampaignId", DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified Campaign id.
        /// </summary>
        /// <param name="CampaignId">The Campaign id.</param>
        internal void Load(int CampaignId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Campaign");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CampaignId", CampaignId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}