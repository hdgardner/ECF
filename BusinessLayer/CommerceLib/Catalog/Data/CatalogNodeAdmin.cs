using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Exceptions;
using System.Data.Common;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrative functions for the catalog node
    /// </summary>
    public class CatalogNodeAdmin
    {
        private CatalogNodeDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CatalogNodeDto CurrentDto
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
                return DataHelper.MapTables("CatalogNode", "CatalogItemSeo");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNodeAdmin"/> class.
        /// </summary>
        internal CatalogNodeAdmin()
        {
            _DataSet = new CatalogNodeDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogNodeAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogNodeAdmin(CatalogNodeDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Gets the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        internal void Load(int catalogNodeId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogNodeId", catalogNodeId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified code.
        /// </summary>
        /// <param name="code">The code.</param>
        internal void Load(string code)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_Code");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogNodeCode", code, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the assets.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        internal void LoadAssets(int catalogNodeId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogNode_Asset");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("CatalogNodeId", catalogNodeId, DataParameterType.Int));
            cmd.TableMapping = DataHelper.MapTables("CatalogItemAsset");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="searchSetId">The search set id.</param>
        internal void LoadSearchResults(Guid searchSetId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_Search_CatalogNode");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchSetId, DataParameterType.UniqueIdentifier));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Updates the catalog node.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.CatalogNode == null)
                return;

            if (CurrentDto.CatalogNode.Count == 0)
                return;

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "CatalogNode", "CatalogItemSeo", "CatalogItemAsset");

                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the by catalog id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        internal void LoadByCatalogId(int catalogId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_CatalogId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by catalog name.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        internal void LoadByCatalogName(string catalogName)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_CatalogName");
            cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogName", catalogName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by catalog id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        internal void LoadByParentNodeId(int catalogId, int parentNodeId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_CatalogParentNode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ParentNodeId", parentNodeId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by parent node code.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        internal void LoadByParentNodeCode(string catalogName, string parentNodeCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_CatalogParentNodeCode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogName", catalogName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("ParentNodeCode", parentNodeCode, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Gets the catalog node by URL na dlanguage code.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        internal void LoadByUri(string uri, string languageCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_UriLanguage");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Uri", uri, DataParameterType.NVarChar, 255));
            cmd.Parameters.Add(new DataParameter("LanguageCode", languageCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the root catalog nodes by site GUID.
        /// </summary>
        /// <param name="siteGuid">The site GUID.</param>
        internal void LoadRootBySite(Guid siteGuid)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogNode_SiteId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", siteGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }
    }
}