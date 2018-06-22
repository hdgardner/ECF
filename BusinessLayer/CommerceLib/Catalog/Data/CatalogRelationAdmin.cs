using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Exceptions;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrative functions for the catalog relation
    /// </summary>
    public class CatalogRelationAdmin
    {
        private CatalogRelationDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CatalogRelationDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogRelationAdmin"/> class.
        /// </summary>
        internal CatalogRelationAdmin()
        {
            _DataSet = new CatalogRelationDto();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogRelationAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogRelationAdmin(CatalogRelationDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the specified catalog id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="responseGroup">The response group.</param>
        internal void Load(int catalogId, int catalogNodeId, int catalogEntryId, string groupName, int responseGroup)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogRelation");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("CatalogNodeId", catalogNodeId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("GroupName", groupName, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ResponseGroup", responseGroup, DataParameterType.Int));
            cmd.DataSet = CurrentDto;
            
            cmd.TableMapping = DataHelper.MapTables("CatalogNodeRelation", "CatalogEntryRelation", "NodeEntryRelation");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the asset.
        /// </summary>
        /// <param name="key">The asset key.</param>
        internal void LoadAsset(string key)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_AssetKey");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("AssetKey", key, DataParameterType.NVarChar, 254));
            cmd.TableMapping = DataHelper.MapTables("CatalogItemAsset");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Updates the catalog node.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "CatalogNodeRelation", "CatalogEntryRelation", "NodeEntryRelation", "CatalogItemAsset");
                scope.Complete();
            }
        }
    }
}