using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Cms.Data;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Cms.Data
{
    public class SiteAdmin
    {
        private SiteDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        internal SiteDto CurrentDto
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
        private System.Data.Common.DataTableMapping[] Mapping
        {
            get
            {
				return DataHelper.MapTables("Site", "main_GlobalVariables", "SiteLanguage");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteAdmin"/> class.
        /// </summary>
        internal SiteAdmin()
        {
            _DataSet = new SiteDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal SiteAdmin(SiteDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the by application.
        /// </summary>
        /// <param name="appId">The app id.</param>
		/// <param name="returnInactive"></param>
        internal void LoadByApplication(Guid appId, bool returnInactive)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("cms_Site");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", appId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataResult results = DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified site id.
        /// </summary>
        /// <param name="siteId">The site id.</param>
		/// <param name="returnInactive"></param>
		internal void Load(Guid siteId, bool returnInactive)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("cms_Site");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataResult results = DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Updates the catalog.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand();
            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(null, cmd, CurrentDto, "Site", "main_GlobalVariables", "SiteLanguage");
                scope.Complete();
            }
        }
    }
}