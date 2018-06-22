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
    /// Contains all the functions needed to perform administration on the Segment
    /// </summary>
    public class SegmentAdmin
    {
        private SegmentDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public SegmentDto CurrentDto
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
                return DataHelper.MapTables("Segment", "SegmentMember", "SegmentCondition");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentAdmin"/> class.
        /// </summary>
        internal SegmentAdmin()
        {
            _DataSet = new SegmentDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal SegmentAdmin(SegmentDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the Segment.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(MarketingContext.MetaDataContext, cmd, CurrentDto, "Segment", "SegmentMember", "SegmentCondition");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads all the segments.
        /// </summary>
        internal void LoadAll()
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Segment");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SegmentId", DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified Segment id.
        /// </summary>
        /// <param name="SegmentId">The Segment id.</param>
        internal void Load(int SegmentId)
        {
            DataCommand cmd = MarketingDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_mktg_Segment");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", MarketingConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SegmentId", SegmentId, DataParameterType.Int));
            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}