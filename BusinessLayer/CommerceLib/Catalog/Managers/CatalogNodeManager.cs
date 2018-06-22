using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Events;
using System.Runtime.Serialization;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Contains internal functions to perform operations related to catalog nodes.
    /// </summary>
    public static class CatalogNodeManager
    {
        /// <summary>
        /// Loads the node.
        /// </summary>
        /// <param name="admin">The admin.</param>
        /// <param name="row">The row.</param>
        /// <param name="responseGroup">The response group.</param>
        private static void LoadNode(CatalogNodeAdmin admin, CatalogNodeDto.CatalogNodeRow row, CatalogNodeResponseGroup responseGroup)
        {
            if (responseGroup.ContainsGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull) || responseGroup.ContainsGroup(CatalogNodeResponseGroup.ResponseGroup.Assets))
            {
                // Load Associations
                admin.LoadAssets(row.CatalogNodeId);
            }
        }

        #region CatalogNode Search functionality
        /// <summary>
        /// Finds the nodes dto2.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        private static CatalogNodeDto FindNodesDto2(CatalogSearch search, ref int recordsCount, CatalogNodeResponseGroup responseGroup)
        {
            CatalogNodeDto dto = null;

            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            recordsCount = search.SearchNodes(searchGuid);

            CatalogNodeAdmin admin = new CatalogNodeAdmin();

            // Load results and return them back
            admin.LoadSearchResults(searchGuid);

            dto = admin.CurrentDto;

            if (dto.CatalogNode.Count > 0)
            {
                foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                    LoadNode(admin, row, responseGroup);
            }

            /*
            if (admin.CurrentDto.CatalogNode.Count > 0)
            {
                MetaDataContext.Current = CatalogContext.MetaDataContext;
                foreach (CatalogNodeDto.CatalogNodeRow row in admin.CurrentDto.CatalogNode.Rows)
                    MetaHelper.FillMetaData(row, row.MetaClassId, row.CatalogNodeId, true);
            }
             * */

            return admin.CurrentDto;
        }

        /// <summary>
        /// Finds the nodes dto.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto FindNodesDto(CatalogSearch search, ref int recordsCount, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            // Only cache results if specified
            if (search.SearchOptions.CacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("catalog-nodesdto", responseGroup.CacheKey, search.CacheKey);

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                    return (CatalogNodeDto)cachedObject;

            }

            CatalogNodeDto dto = FindNodesDto2(search, ref recordsCount, responseGroup);

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, search.SearchOptions.CacheTimeout);
            }

            return dto;
        }        
        #endregion

        /// <summary>
        /// Retrieves table containing nodes and entries from the specified catalog and catagory.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>
        /// DataTable with the following fields:
        /// ID, Name, Type, Code, StartDate, EndDate, IsActive, SortOrder, RowNumber.
        /// If returnTotalCount is true, [RecordCount] is returned in the last column of the output table.
        /// </returns>
        internal static DataTable GetCatalogItemsTable(ItemSearchParameters parameters, CatalogNodeResponseGroup responseGroup)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogitemslist", responseGroup.CacheKey, parameters.CatalogId.ToString(), 
				parameters.ParentNodeId.ToString(), parameters.OrderByClause, parameters.RecordsToRetrieve.ToString(),
				parameters.StartingRecord.ToString(), parameters.ReturnTotalCount.ToString(), parameters.ReturnInactive.ToString());

			DataTable table = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				table = (DataTable)cachedObject;

			// Load the object
			if (table == null)
			{
				DataCommand cmd = CatalogDataHelper.CreateDataCommand();
				cmd.CommandText = String.Format("[ecf_CatalogNodesList]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("CatalogId", parameters.CatalogId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("CatalogNodeId", parameters.ParentNodeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("OrderClause", parameters.OrderByClause, DataParameterType.NVarChar, 100));
				cmd.Parameters.Add(new DataParameter("StartingRec", parameters.StartingRecord, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NumRecords", parameters.RecordsToRetrieve, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("ReturnInactive", parameters.ReturnInactive, DataParameterType.Bit));
				cmd.Parameters.Add(new DataParameter("ReturnTotalCount", parameters.ReturnTotalCount, DataParameterType.Bit));

				table = DataService.LoadTable(cmd);

				if (table != null)
				{
					// Insert to the cache collection
					CatalogCache.Insert(cacheKey, table, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
				}
			}

			return table;
		}

        /// <summary>
        /// Gets the catalog dto, checks permissions and caches results.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
		internal static CatalogNodeDto GetCatalogNodesDto(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes", responseGroup.CacheKey, catalogId.ToString());

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.LoadByCatalogId(catalogId);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodesDto(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes-name", responseGroup.CacheKey, catalogName);

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.LoadByCatalogName(catalogName);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodesDto(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes", responseGroup.CacheKey, catalogId.ToString(), parentNodeId.ToString());

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.LoadByParentNodeId(catalogId, parentNodeId);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodesDto(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes", responseGroup.CacheKey, catalogName.ToString(), parentNodeCode.ToString());

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.LoadByParentNodeCode(catalogName, parentNodeCode);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodeDto(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode", responseGroup.CacheKey, catalogNodeId.ToString());

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.Load(catalogNodeId);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodeDto(string code, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode-code", responseGroup.CacheKey, code.ToString());

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.Load(code);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog node dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodeDto GetCatalogNodeDto(string uri, string languageCode, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode-uri", responseGroup.CacheKey, languageCode.ToString(), uri);

            CatalogNodeDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogNodeDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogNodeAdmin catalog = new CatalogNodeAdmin();
                catalog.LoadByUri(uri, languageCode);
                dto = catalog.CurrentDto;

                if (dto.CatalogNode.Count > 0)
                {
                    foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode.Rows)
                        LoadNode(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Saves the catalog node.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal static void SaveCatalogNode(CatalogNodeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogNodeDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.CatalogSecurity, String.Format("CatalogId = -1 or CatalogId = {0}", row.CatalogId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(CatalogScope.Catalog, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            CatalogNodeAdmin admin = new CatalogNodeAdmin(dto);
            EventContext.Instance.RaiseNodeUpdatingEvent(dto, new NodeEventArgs("updating"));
            admin.Save();
            EventContext.Instance.RaiseNodeUpdatedEvent(dto, new NodeEventArgs("updated"));
        }

        /// <summary>
        /// Deletes the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogId">The catalog id.</param>
        internal static void DeleteCatalogNode(int catalogNodeId, int catalogId)
        {
            CatalogNodeDto catalogNodeDto = GetCatalogNodesDto(catalogId, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull));
            CatalogRelationDto catalogRelationDto = CatalogRelationManager.GetCatalogRelationDto(0, 0, 0, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogNode | CatalogRelationResponseGroup.ResponseGroup.NodeEntry));

            DeleteNodeRecursive(catalogNodeId, catalogId, ref catalogNodeDto, ref catalogRelationDto);

            if (catalogRelationDto.HasChanges())
                CatalogRelationManager.SaveCatalogRelation(catalogRelationDto);

            if (catalogNodeDto.HasChanges())
                SaveCatalogNode(catalogNodeDto);
        }

        /// <summary>
        /// Deletes the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeDto">The catalog node Dto.</param>
        /// <param name="catalogDto">The catalog Dto.</param>
        internal static void DeleteNodeRecursive(int catalogNodeId, int catalogId, ref CatalogNodeDto catalogNodeDto, ref CatalogRelationDto catalogRelationDto)
        {
            CatalogNodeDto.CatalogNodeRow row = catalogNodeDto.CatalogNode.FindByCatalogNodeId(catalogNodeId);
            if (row != null)
            {
                DataRow[] catalogRelations = catalogRelationDto.CatalogNodeRelation.Select(String.Format("CatalogId = {0} AND ParentNodeId = {1}", catalogId, catalogNodeId));
                foreach (DataRow catalogRelation in catalogRelations)
                {
                    catalogRelation.Delete();
                }

                DataRow[] nodeEntryRelations = catalogRelationDto.NodeEntryRelation.Select(String.Format("CatalogId = {0} AND CatalogNodeId = {1}", catalogId, catalogNodeId));
                foreach (DataRow nodeEntryRelation in nodeEntryRelations)
                {
                    nodeEntryRelation.Delete();
                }

                row.Delete();

                DataRow[] catalogNodes = catalogNodeDto.CatalogNode.Select(String.Format("ParentNodeId = {0}", catalogNodeId));
                if (catalogNodes.Length > 0)
                {
                    foreach (DataRow catalogNode in catalogNodes)
                    {
                        DeleteNodeRecursive((int)catalogNode["CatalogNodeId"], catalogId, ref catalogNodeDto, ref catalogRelationDto);
                    }
                }
            }
        }

        #region Object Methods

        /// <summary>
        /// Gets the catalog node. Results are cached.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNode GetCatalogNode(string code, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode-objects-code", responseGroup.CacheKey, code.ToString());

            CatalogNode node = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                node = (CatalogNode)cachedObject;

            // Load the object
            if (node == null)
            {
                CatalogNodeDto dto = GetCatalogNodeDto(code, responseGroup);

                // Load node
                if (dto.CatalogNode.Count > 0)
                    node = LoadNode(dto.CatalogNode[0], false, responseGroup);
                else
                    node = new CatalogNode();


                CatalogCache.Insert(cacheKey, node, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return node;
        }

        /// <summary>
        /// Gets the catalog node. Results are cached.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNode GetCatalogNode(int catalogNodeId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode-objects-id", responseGroup.CacheKey, catalogNodeId.ToString());

            CatalogNode node = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                node = (CatalogNode)cachedObject;

            // Load the object
            if (node == null)
            {
                CatalogNodeDto dto = GetCatalogNodeDto(catalogNodeId, responseGroup);

                // Load node
                if (dto.CatalogNode.Count > 0)
                    node = LoadNode(dto.CatalogNode[0], false, responseGroup);
                else
                    node = new CatalogNode();

                CatalogCache.Insert(cacheKey, node, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return node;
        }

        /// <summary>
        /// Gets the catalog node. Results are cached.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNode GetCatalogNode(string uri, string languageCode, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognode-objects-uri", responseGroup.CacheKey, uri, languageCode);

            CatalogNode node = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                node = (CatalogNode)cachedObject;

            // Load the object
            if (node == null)
            {
                CatalogNodeDto dto = GetCatalogNodeDto(uri, languageCode, responseGroup);

                // Load node
                if (dto.CatalogNode.Count > 0)
                    node = LoadNode(dto.CatalogNode[0], false, responseGroup);
                else
                    node = new CatalogNode();

                CatalogCache.Insert(cacheKey, node, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return node;
        }

        /// <summary>
        /// Gets the catalog nodes. Results are cached.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodes GetCatalogNodes(int catalogId, int parentNodeId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes-parentid", responseGroup.CacheKey, catalogId.ToString(), parentNodeId.ToString());

            CatalogNodes nodes = new CatalogNodes();

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
            {
                nodes = (CatalogNodes)cachedObject;
            }
            else
            {
                CatalogNodeDto dto = GetCatalogNodesDto(catalogId, parentNodeId, responseGroup);

                // Load Node
                if (dto.CatalogNode.Count > 0)
                    nodes = LoadNodes(dto, null, false, responseGroup);

                CatalogCache.Insert(cacheKey, nodes, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return nodes;
        }

        /// <summary>
        /// Gets the catalog nodes. Results are cached.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodes GetCatalogNodes(int catalogId, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes-catalogid", responseGroup.CacheKey, catalogId.ToString());

            CatalogNodes nodes = new CatalogNodes();

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
            {
                nodes = (CatalogNodes)cachedObject;
            }
            else
            {
                CatalogNodeDto dto = GetCatalogNodesDto(catalogId, responseGroup);

                // Load Node
                if (dto.CatalogNode.Count > 0)
                    nodes = LoadNodes(dto, null, false, responseGroup);

                CatalogCache.Insert(cacheKey, nodes, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return nodes;
        }

        /// <summary>
        /// Gets the catalog nodes. Results are cached.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodes GetCatalogNodes(string catalogName, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes-catalogname", responseGroup.CacheKey, catalogName);

            CatalogNodes nodes = new CatalogNodes();

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
            {
                nodes = (CatalogNodes)cachedObject;
            }
            else
            {
                CatalogNodeDto dto = GetCatalogNodesDto(catalogName, responseGroup);

                // Load Node
                if (dto.CatalogNode.Count > 0)
                    nodes = LoadNodes(dto, null, false, responseGroup);

                CatalogCache.Insert(cacheKey, nodes, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return nodes;
        }

        /// <summary>
        /// Gets the catalog nodes. Results are cached.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodes GetCatalogNodes(string catalogName, string parentNodeCode, CatalogNodeResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalognodes-catalogname-code", responseGroup.CacheKey, catalogName, parentNodeCode);

            CatalogNodes nodes = new CatalogNodes();

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
            {
                nodes = (CatalogNodes)cachedObject;
            }
            else
            {
                CatalogNodeDto dto = GetCatalogNodesDto(catalogName, parentNodeCode, responseGroup);

                // Load Node
                if (dto.CatalogNode.Count > 0)
                    nodes = LoadNodes(dto, null, false, responseGroup);

                CatalogCache.Insert(cacheKey, nodes, CatalogConfiguration.Instance.Cache.CatalogNodeTimeout);
            }

            return nodes;
        }

        /// <summary>
        /// Loads the node.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNode LoadNode(/*CatalogNodeDto dto,*/ CatalogNodeDto.CatalogNodeRow row, bool recursive, CatalogNodeResponseGroup responseGroup)
        {
            CatalogNode node = null;

            // Load node
            if (row != null)
            {
                node = new CatalogNode(row);
            }
            else
                node = new CatalogNode();

            // Populate children
            if (responseGroup.ContainsGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull) || responseGroup.ContainsGroup(CatalogNodeResponseGroup.ResponseGroup.Children))
            {
                CatalogNodeDto childrenDto = GetCatalogNodesDto(row.CatalogId, row.CatalogNodeId, responseGroup);
                CatalogNodes nodes = LoadNodes(childrenDto, node, recursive, responseGroup);
                node.Children = nodes;
            }

            return node;
        }

        /// <summary>
        /// Loads the nodes.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogNodes LoadNodes(CatalogNodeDto dto, CatalogNode parent, bool recursive, CatalogNodeResponseGroup responseGroup)
        {
            List<CatalogNode> nodes = new List<CatalogNode>();

            foreach (CatalogNodeDto.CatalogNodeRow childRow in dto.CatalogNode)
            {
                CatalogNode childNode = LoadNode(childRow, recursive, responseGroup);
                childNode.ParentNode = parent;
                nodes.Add(childNode);
            }

            CatalogNodes n = new CatalogNodes();
            n.CatalogNode = nodes.ToArray();
            return n;
        }
        #endregion
    }

    /// <summary>
    /// Implements operations for and represents item search parameters.
    /// </summary>
	[DataContract]
	public class ItemSearchParameters
	{
		private int _catalogId;
		private int _parentNodeId;
		private string _orderByClause;
		private int _startingRecord;
		private int _recordsToRetrieve;
		private bool _returnInactive;
		bool _returnTotalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSearchParameters"/> class.
        /// </summary>
        public ItemSearchParameters()
		{
			_startingRecord = 1;
			_recordsToRetrieve = 20;
			_returnTotalCount = true;
			_orderByClause = String.Empty;
		}

        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        /// <value>The catalog id.</value>
		public int CatalogId
		{
			get { return _catalogId; }
			set { _catalogId = value; }
		}

        /// <summary>
        /// Gets or sets the parent node id.
        /// </summary>
        /// <value>The parent node id.</value>
		public int ParentNodeId
		{
			get { return _parentNodeId; }
			set { _parentNodeId = value; }
		}

        /// <summary>
        /// Gets or sets the order by clause.
        /// </summary>
        /// <value>The order by clause.</value>
		public string OrderByClause
		{
			get { return _orderByClause; }
			set { _orderByClause = value; }
		}

        /// <summary>
        /// Starting number of record to return. Should be pageSize*(pageNumber-1)+1.
        /// </summary>
        /// <value>The starting record.</value>
        /// <remarks>
        /// Records numbering starts with 1.
        /// </remarks>
		public int StartingRecord
		{
			get { return _startingRecord; }
			set { _startingRecord = value; }
		}

        /// <summary>
        /// Number of records to be retrieved.
        /// </summary>
        /// <value>The records to retrieve.</value>
		public int RecordsToRetrieve
		{
			get { return _recordsToRetrieve; }
			set { _recordsToRetrieve = value; }
		}

        /// <summary>
        /// If true, total records count will be returned
        /// </summary>
        /// <value><c>true</c> if [return total count]; otherwise, <c>false</c>.</value>
		public bool ReturnTotalCount
		{
			get { return _returnTotalCount; }
			set { _returnTotalCount = value; }
		}

        /// <summary>
        /// If true, inactive nodes/entries will be returned.
        /// </summary>
        /// <value><c>true</c> if [return inactive]; otherwise, <c>false</c>.</value>
		public bool ReturnInactive
		{
			get { return _returnInactive; }
			set { _returnInactive = value; }
		}
	}
}