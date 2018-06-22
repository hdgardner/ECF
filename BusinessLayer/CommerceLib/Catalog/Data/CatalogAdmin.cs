using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using System.Data;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Contains all the functions needed to perform administration on the catalog
    /// </summary>
    public class CatalogAdmin
    {
        private CatalogDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CatalogDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogAdmin"/> class.
        /// </summary>
        internal CatalogAdmin()
        {
            _DataSet = new CatalogDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogAdmin(CatalogDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Adds the site catalog relation.
        /// </summary>
        /// <param name="siteGuid">The site GUID.</param>
        /// <param name="catalogId">The catalog id.</param>
        internal static void AddSiteCatalogRelation(Guid siteGuid, int catalogId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = DataHelper.CreateInsertStoredProcedureName("SiteCatalog");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteGuid));
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId));
            DataService.ExecuteNonExec(cmd);
        }

        /// <summary>
        /// Updates the catalog.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.Catalog == null)
                return;

            if (CurrentDto.Catalog.Count == 0)
                return;

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "Catalog", "CatalogLanguage", "SiteCatalog");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified site GUID.
        /// </summary>
        /// <param name="siteGuid">The site GUID.</param>
        internal void Load(Guid siteGuid)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Catalog");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            
            if(siteGuid != Guid.Empty)
                cmd.Parameters.Add(new DataParameter("SiteId", siteGuid, DataParameterType.UniqueIdentifier));
            else
                cmd.Parameters.Add(new DataParameter("SiteId", DataParameterType.UniqueIdentifier));

            cmd.Parameters.Add(new DataParameter("CatalogId", DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("Catalog", "CatalogLanguage", "SiteCatalog");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified catalog id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        internal void Load(int catalogId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Catalog");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SiteId", DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("Catalog", "CatalogLanguage", "SiteCatalog");

            DataService.LoadDataSet(cmd);
        }
    }
}