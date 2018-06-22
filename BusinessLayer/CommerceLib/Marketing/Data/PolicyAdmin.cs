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
    /// Contains all the functions needed to perform administration on the Policy
    /// </summary>
    public class PolicyAdmin
    {
        private PolicyDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public PolicyDto CurrentDto
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
                return DataHelper.MapTables("Policy", "GroupPolicy");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyAdmin"/> class.
        /// </summary>
        internal PolicyAdmin()
        {
            _DataSet = new PolicyDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal PolicyAdmin(PolicyDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the Policy.
        /// </summary>
        internal void Save()
        {
            // TODO: Check if user is allowed to perform this operation
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "Policy", "GroupPolicy");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified site GUID.
        /// </summary>
        internal void LoadAll()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Policy");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PolicyId", DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified Policy id.
        /// </summary>
        /// <param name="PolicyId">The Policy id.</param>
        internal void Load(int PolicyId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Policy");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PolicyId", PolicyId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}