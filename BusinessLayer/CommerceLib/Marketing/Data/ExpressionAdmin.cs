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
    /// Contains all the functions needed to perform administration on the Expression
    /// </summary>
    public class ExpressionAdmin
    {
        private ExpressionDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public ExpressionDto CurrentDto
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
                return DataHelper.MapTables("Expression");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAdmin"/> class.
        /// </summary>
        internal ExpressionAdmin()
        {
            _DataSet = new ExpressionDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal ExpressionAdmin(ExpressionDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the Expression.
        /// </summary>
        internal void Save()
        {
            // TODO: Check if user is allowed to perform this operation
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "Expression");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified site GUID.
        /// </summary>
        internal void LoadAll()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Expression");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("ExpressionId", DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified Expression id.
        /// </summary>
        /// <param name="ExpressionId">The Expression id.</param>
        internal void Load(int ExpressionId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Expression");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("ExpressionId", ExpressionId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by category.
        /// </summary>
        /// <param name="category">The category.</param>
        internal void LoadByCategory(string category)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Expression_Category");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Category", category, DataParameterType.NVarChar, 50));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by segment.
        /// </summary>
        /// <param name="segmentId">The segment id.</param>
        internal void LoadBySegment(int segmentId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Expression_Segment");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SegmentId", segmentId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}