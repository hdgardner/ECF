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
    /// Contains all the functions needed to perform administration on the Promotion
    /// </summary>
    public class PromotionAdmin
    {
        private PromotionDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public PromotionDto CurrentDto
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
                return DataHelper.MapTables("Promotion", "PromotionCondition", "PromotionLanguage", "PromotionPolicy");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionAdmin"/> class.
        /// </summary>
        internal PromotionAdmin()
        {
            _DataSet = new PromotionDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal PromotionAdmin(PromotionDto dto)
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
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "Promotion", "PromotionPolicy", "PromotionCondition", "PromotionLanguage");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads all the promotions.
        /// </summary>
        internal void LoadAll()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Promotion");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PromotionId", DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified Promotion id.
        /// </summary>
        /// <param name="PromotionId">The Promotion id.</param>
        internal void Load(int PromotionId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Promotion");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PromotionId", PromotionId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the promotion using the specified datetime.
        /// </summary>
        /// <param name="datetime">The datetime in UTC format.</param>
        internal void Load(DateTime datetime)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_PromotionByDate");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("DateTime", datetime, DataParameterType.DateTime));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}