using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Exceptions;
using System.Data.Common;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrative functions for the catalog entry
    /// </summary>
    public class CatalogEntryAdmin
    {
        private CatalogEntryDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        internal CatalogEntryDto CurrentDto
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
                return DataHelper.MapTables("CatalogEntry", "CatalogItemSeo");
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogEntryAdmin"/> class.
        /// </summary>
        internal CatalogEntryAdmin()
        {
            _DataSet = new CatalogEntryDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogEntryAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogEntryAdmin(CatalogEntryDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the specified catalog entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal void Load(int catalogEntryId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntry");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified catalog entry code.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        internal void Load(string catalogEntryCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntryByCode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogEntryCode", catalogEntryCode, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.CatalogEntry == null)
                return;

            if (CurrentDto.CatalogEntry.Count == 0)
                return;

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
				DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "CatalogEntry", "CatalogItemSeo", "CatalogItemAsset", "Variation", "Inventory", "Merchant", "CatalogAssociation", "SalePrice");
                scope.Complete();
            }
        }

		internal void SaveMerchants()
		{
			if (CurrentDto.Merchant == null)
				return;

			if (CurrentDto.Merchant.Count == 0)
				return;

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "Merchant");
				scope.Complete();
			}
		}


        /// <summary>
        /// Deletes the specified catalog entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal static void Delete(int catalogEntryId)
        {
            CatalogEntryAdmin admin = new CatalogEntryAdmin();
            admin.Load(catalogEntryId);

            if (admin.CurrentDto.CatalogEntry.Count == 0)
                throw new InvalidObjectException();

            int metaClassId = admin.CurrentDto.CatalogEntry[0].MetaClassId;

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = DataHelper.CreateDeleteStoredProcedureName("CatalogEntry");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId));

            using (TransactionScope scop = new TransactionScope())
            {
                DataService.ExecuteNonExec(cmd);
                // Make sure to remove meta data, if any
                if (metaClassId > 0)
                    MetaObject.Delete(CatalogContext.MetaDataContext, catalogEntryId, metaClassId);
            }
        }
		
        /// <summary>
        /// Loads the by catalog id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        internal void LoadByCatalogId(int catalogId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntry_CatalogId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by catalog node id.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        internal void LoadByCatalogNodeId(int catalogId, int catalogNodeId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntry_CatalogNodeId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("CatalogNodeId", catalogNodeId, DataParameterType.Int));
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
            cmd.CommandText = String.Format("ecf_CatalogEntry_CatalogName");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogName", catalogName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by catalog node id.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        internal void LoadByCatalogNodeId(string catalogName, int catalogNodeId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntry_CatalogNameCatalogNodeId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogName", catalogName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("CatalogNodeId", catalogNodeId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the entrie by catalog name and node code.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        internal void LoadByCatalogNodeCode(string catalogName, string catalogNodeCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogEntry_CatalogNameCatalogNodeCode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogName", catalogName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("CatalogNodeCode", catalogNodeCode, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Gets the catalog node by URI and language code.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        internal void LoadByUri(string uri, string languageCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_UriLanguage");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Uri", uri, DataParameterType.NVarChar, 255));
            cmd.Parameters.Add(new DataParameter("LanguageCode", languageCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the by parent entry id.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        internal void LoadChildren(int parentEntryId, string entryType, string relationType)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_ParentEntryId");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ParentEntryId", parentEntryId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("ClassTypeId", entryType.ToString(), DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("RelationTypeId", relationType.ToString(), DataParameterType.NVarChar, 50));           
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);

            // Mark columns with a do not persist attribute
            DataTable catalogEntryTable = cmd.DataSet.Tables["CatalogEntry"];
            catalogEntryTable.Columns["RelationTypeId"].ExtendedProperties.Add("Persist","false");
            catalogEntryTable.Columns["Quantity"].ExtendedProperties.Add("Persist", "false");
            catalogEntryTable.Columns["GroupName"].ExtendedProperties.Add("Persist","false");
            catalogEntryTable.Columns["SortOrder"].ExtendedProperties.Add("Persist","false");           
        }

        /// <summary>
        /// Loads the entry by name and type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        internal void LoadByName(string name, string entryType)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_Name");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("ClassTypeId", entryType.ToString(), DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the associated.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        internal void LoadAssociated(int parentEntryId, string associationName)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("[ecf_CatalogEntry_Associated]");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", parentEntryId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("AssociationName", associationName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the associated.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        internal void LoadAssociated(string parentEntryCode, string associationName)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("[ecf_CatalogEntry_AssociatedByCode]");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogEntryCode", parentEntryCode, DataParameterType.NVarChar, 100));
            cmd.Parameters.Add(new DataParameter("AssociationName", associationName, DataParameterType.NVarChar, 150));
            cmd.Parameters.Add(new DataParameter("ReturnInactive", true, DataParameterType.Bit));
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the association.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
		internal void LoadAssociation(int catalogEntryId)
		{
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();

			cmd.CommandText = String.Format("ecf_CatalogEntry_Association");
			cmd.Parameters = new DataParameters();
			cmd.DataSet = CurrentDto;
			cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
			cmd.TableMapping = DataHelper.MapTables("CatalogAssociation"); //, "CatalogEntryAssociation");

			DataService.LoadDataSet(cmd);
		}

        /// <summary>
        /// Loads the variation.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal void LoadVariation(int catalogEntryId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_Variation");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.TableMapping = DataHelper.MapTables("Variation", "SalePrice", "Merchant");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the assets.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal void LoadAssets(int catalogEntryId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_Asset");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.TableMapping = DataHelper.MapTables("CatalogItemAsset");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the inventory.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal void LoadInventory(int catalogEntryId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_CatalogEntry_Inventory");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.TableMapping = DataHelper.MapTables("Inventory");

            DataService.LoadDataSet(cmd);
        }

		/// <summary>
        /// Loads all the merchants for the current application.
        /// </summary>
        internal void LoadMerchants()
        {
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("select * from [Merchant] where [ApplicationId]='{0}';", CatalogConfiguration.Instance.ApplicationId);
			cmd.CommandType = CommandType.Text;
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("Merchant");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Inserts the entry ids into search results table.
        /// </summary>
        /// <param name="searchSetId">The search set id.</param>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <returns>true if any results found</returns>
        internal bool InsertSearchResults(Guid searchSetId, int[] catalogEntries)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            string list = String.Empty;

            foreach(int id in catalogEntries)
            {
                if(String.IsNullOrEmpty(list))
                    list = id.ToString();
                else
                    list = list + "," + id.ToString();
            }

            cmd.CommandText = String.Format("ecf_CatalogEntry_SearchInsertList");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchSetId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("List", list, DataParameterType.NVarChar));
            DataService.ExecuteNonExec(cmd);
            return true;
        }

        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="searchSetId">The search set id.</param>
        internal void LoadSearchResults(Guid searchSetId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            cmd.CommandText = String.Format("ecf_Search_CatalogEntry");
            cmd.Parameters = new DataParameters();
            cmd.DataSet = CurrentDto;
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchSetId, DataParameterType.UniqueIdentifier));
            cmd.TableMapping = DataHelper.MapTables("CatalogEntry", "CatalogItemSeo", "Variation", "Merchant", "Inventory", "SalePrice", "CatalogAssociation", "CatalogItemAsset");

            DataService.LoadDataSet(cmd);
        }
    }
}