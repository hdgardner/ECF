using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Events;
using Mediachase.Commerce.Catalog.Exceptions;
using System.Collections.Specialized;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Contains functions that provide methods to manipulate catalog entries.
    /// </summary>
    public static class CatalogEntryManager
    {
        #region DTO Methods

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(int[] catalogEntries, Boolean cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            if (catalogEntries == null || catalogEntries.Length == 0)
                return null;

            // Only cache results if specified
            if (cacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("catalog-entriesdto", responseGroup.CacheKey, CreateCacheKey(catalogEntries));

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                    return (CatalogEntryDto)cachedObject;

            }

            Guid searchGuid = Guid.NewGuid();
            CatalogEntryAdmin catalog = new CatalogEntryAdmin();
            catalog.InsertSearchResults(searchGuid, catalogEntries);
            catalog.LoadSearchResults(searchGuid);
            CatalogEntryDto dto = catalog.CurrentDto;

            if (dto.CatalogEntry.Count > 0)
            {
                LoadEntry(catalog, dto.CatalogEntry[0], responseGroup);
            }

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, cacheTimeout);
            }

            return dto;
        }

        /// <summary>
        /// Creates the cache key.
        /// </summary>
        /// <param name="intArray">The int array.</param>
        /// <returns></returns>
        private static string CreateCacheKey(int[] intArray)
        {
            string cacheKey = String.Empty;
            int count = 0;
            foreach (int val in intArray)
            {
                count++;
                cacheKey += val.ToString() + "-";
                if (count > 50)
                {
                    break;
                }
            }

            cacheKey += count.ToString() + "-" + intArray.Length;
            return cacheKey;
        }

        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        private static CatalogEntryDto FindItemsDto2(CatalogSearch search, ref int recordsCount, CatalogEntryResponseGroup responseGroup)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            recordsCount = search.SearchEntries(searchGuid);

            CatalogEntryAdmin admin = new CatalogEntryAdmin();

            // Load results and return them back
            admin.LoadSearchResults(searchGuid);

            return admin.CurrentDto;
        }

        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto FindItemsDto(CatalogSearch search, ref int recordsCount, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            // Only cache results if specified
            if (search.SearchOptions.CacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("catalog-entriesdto", responseGroup.CacheKey, search.CacheKey);

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                    return (CatalogEntryDto)cachedObject;

            }

            CatalogEntryDto dto = FindItemsDto2(search, ref recordsCount, responseGroup);

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, search.SearchOptions.CacheTimeout);
            }

            return dto;
        }

        #region Load Associated Entries
        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetAssociatedCatalogEntriesDto(int parentEntryId, string associationName, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentries", responseGroup.CacheKey, parentEntryId.ToString(), associationName);

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadAssociated(parentEntryId, associationName);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the associated catalog entries dto.
        /// </summary>
        /// <param name="parentEntryCode">The parent entry code.</param>
        /// <param name="associationName">Name of the association.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetAssociatedCatalogEntriesDto(string parentEntryCode, string associationName, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentries", responseGroup.CacheKey, parentEntryCode, associationName);

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadAssociated(parentEntryCode, associationName);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }
        #endregion

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntryDto(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry", responseGroup.CacheKey, catalogEntryId.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.Load(catalogEntryId);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    LoadEntry(catalog, dto.CatalogEntry[0], responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntryDto(string catalogEntryCode, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry-code-", responseGroup.CacheKey, catalogEntryCode.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.Load(catalogEntryCode);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    LoadEntry(catalog, dto.CatalogEntry[0], responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entry by URI dto.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntryByUriDto(string uri, string languageCode, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry-uri-", responseGroup.CacheKey, uri, languageCode);

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByUri(uri, languageCode);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    LoadEntry(catalog, dto.CatalogEntry[0], responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="parentEntryId">The parent entry id.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="relationType">Type of the relation.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(int parentEntryId, string entryType, string relationType, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogrelatedentries", responseGroup.CacheKey, parentEntryId.ToString(), entryType.ToString(), relationType.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadChildren(parentEntryId, entryType, relationType);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach(CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryType">Type of the entry.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(string name, string entryType, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogrelatedentries", responseGroup.CacheKey, name.ToString(), entryType.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByName(name, entryType);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Saves the catalog entry.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal static void SaveCatalogEntry(CatalogEntryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogEntryDto can not be null"));

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


            CatalogEntryAdmin admin = new CatalogEntryAdmin(dto);
            EventContext.Instance.RaiseEntryUpdatingEvent(dto, new EntryEventArgs("updating"));
            admin.Save();
            EventContext.Instance.RaiseEntryUpdatedEvent(dto, new EntryEventArgs("updated"));
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        internal static void DeleteCatalogEntry(int entryId, bool recursive)
        {
            CatalogEntryDto catalogEntryDto = GetCatalogEntryDto(entryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

            if (catalogEntryDto.CatalogEntry.Count > 0)
            {
                if (recursive)
                {
                    //Delete child entry rows
                    CatalogEntryDto childrenDto = GetCatalogEntriesDto(entryId, String.Empty, String.Empty, new CatalogEntryResponseGroup());
                    foreach (CatalogEntryDto.CatalogEntryRow row in childrenDto.CatalogEntry)
                        DeleteCatalogEntry(row.CatalogEntryId, recursive);
                }

                CatalogRelationDto catalogRelationDto = CatalogRelationManager.GetCatalogRelationDto(0, 0, entryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry | CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));

                //Delete NodeEntryRelation rows
                foreach (CatalogRelationDto.NodeEntryRelationRow row in catalogRelationDto.NodeEntryRelation.Rows)
                    row.Delete();

                //Delete CatalogEntryRelation rows
                foreach (CatalogRelationDto.CatalogEntryRelationRow row in catalogRelationDto.CatalogEntryRelation.Rows)
                    row.Delete();

                if (catalogRelationDto.HasChanges())
                    CatalogRelationManager.SaveCatalogRelation(catalogRelationDto);

                //Delete CatalogEntryAssociation rows
                foreach (CatalogEntryDto.CatalogAssociationRow catalogAssociationRow in catalogEntryDto.CatalogAssociation)
                {
                    CatalogAssociationDto catalogAssociationDto = CatalogAssociationManager.GetCatalogAssociationDto(catalogAssociationRow.CatalogAssociationId);
                    foreach (CatalogAssociationDto.CatalogEntryAssociationRow itemCatalogEntryAssociation in catalogAssociationDto.CatalogEntryAssociation)
                    {
                        itemCatalogEntryAssociation.Delete();
                    }

                    if (catalogAssociationDto.HasChanges())
                        CatalogAssociationManager.SaveCatalogAssociation(catalogAssociationDto);
                }

                CatalogEntryDto.CatalogEntryRow entryRow = catalogEntryDto.CatalogEntry[0];

                // Delete inventory if on exists
                if (entryRow.InventoryRow != null)
                    entryRow.InventoryRow.Delete();

                //Delete entry row
                entryRow.Delete();
                SaveCatalogEntry(catalogEntryDto);
            }
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(int catalogId, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry", responseGroup.CacheKey, catalogId.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByCatalogId(catalogId);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogEntryTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(string catalogName, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry-catalogname", responseGroup.CacheKey, catalogName.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByCatalogName(catalogName);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(int catalogId, int parentNodeId, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry", responseGroup.CacheKey, catalogId.ToString(), parentNodeId.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByCatalogNodeId(catalogId, parentNodeId);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeId">The parent node id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesDto(string catalogName, int parentNodeId, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentry-name", responseGroup.CacheKey, catalogName.ToString(), parentNodeId.ToString());

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByCatalogNodeId(catalogName, parentNodeId);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog entries by catalog dto.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="parentNodeCode">The parent node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogEntryDto GetCatalogEntriesByCatalogDto(string catalogName, string parentNodeCode, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogentries-catalogname", responseGroup.CacheKey, catalogName, parentNodeCode);

            CatalogEntryDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogEntryDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogEntryAdmin catalog = new CatalogEntryAdmin();
                catalog.LoadByCatalogNodeCode(catalogName, parentNodeCode);
                dto = catalog.CurrentDto;

                if (dto.CatalogEntry.Count > 0)
                {
                    foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry.Rows)
                        LoadEntry(catalog, row, responseGroup);
                }

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        /// <param name="admin">The admin.</param>
        /// <param name="row">The row.</param>
        /// <param name="responseGroup">The response group.</param>
        private static void LoadEntry(CatalogEntryAdmin admin, CatalogEntryDto.CatalogEntryRow row, CatalogEntryResponseGroup responseGroup)
        {
            if (responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull))
            {
                // Load Variations
                admin.LoadVariation(row.CatalogEntryId);

                // Load inventory
                admin.LoadInventory(row.CatalogEntryId);
            }

            if (responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull) || responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.Associations))
            {
                // Load Associations
                admin.LoadAssociation(row.CatalogEntryId);
            }

            if (responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull) || responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.Assets))
            {
                // Load Associations
                admin.LoadAssets(row.CatalogEntryId);
            }
        }

		/// <summary>
		/// Gets the catalog entries by catalog dto.
		/// </summary>
		/// <returns></returns>
		internal static CatalogEntryDto GetMerchants()
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogentry-merchants");

			CatalogEntryDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogEntryDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				CatalogEntryAdmin catalog = new CatalogEntryAdmin();
				catalog.LoadMerchants();
				dto = catalog.CurrentDto;

				// Insert to the cache collection
				CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
			}

			//dto.AcceptChanges();

			return dto;
		}

		/// <summary>
		/// Gets the merchant id by merchant name. If there are more tham 1 merchant with the specified name, it returns the first one.
		/// </summary>
		/// <returns>Returns id if merchant is found, otherwise </returns>
		internal static Guid GetMerchantIdByName(string name)
		{
			CatalogEntryDto dto = GetMerchants();

			Guid merchantId = Guid.Empty;

			if (dto != null)
			{
				CatalogEntryDto.MerchantRow[] merchantRows = (CatalogEntryDto.MerchantRow[])dto.Merchant.Select(String.Format("Name='{0}'", name));
				if (merchantRows != null && merchantRows.Length > 0)
					merchantId = merchantRows[0].MerchantId;
			}

			return merchantId;
		}
        #endregion

        #region Object Methods

        /// <summary>
        /// Finds the items. Results can be cached. Caching parameters are specified in CatalogSearch variable.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entries FindItems(CatalogSearch search, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            // Only cache results if specified
            if (search.SearchOptions.CacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("catalog-entries", responseGroup.CacheKey, search.CacheKey);

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                    return (Entries)cachedObject;

            }

            int recordsCount = 0;

            CatalogSearchOptions opt = search.SearchOptions;
            CatalogEntryDto dto = FindItemsDto2(search, ref recordsCount, responseGroup);

            Entries items = new Entries();

            if (dto.CatalogEntry.Count > 0)
                items = LoadEntries(dto, null, true, responseGroup);

            items.TotalResults = recordsCount;
            items.TotalPages = (int)Math.Ceiling((decimal)recordsCount / opt.RecordsToRetrieve);

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, items, search.SearchOptions.CacheTimeout);
            }

            return items;
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogEntries">The catalog entries.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entries GetCatalogEntries(int[] catalogEntries, bool cacheResults, TimeSpan cacheTimeout, CatalogEntryResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            if (catalogEntries == null || catalogEntries.Length == 0)
                return new Entries();

            // Only cache results if specified
            if (cacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("catalog-entries", responseGroup.CacheKey, CreateCacheKey(catalogEntries));

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                    return (Entries)cachedObject;
            }

            CatalogEntryDto dto = GetCatalogEntriesDto(catalogEntries, false, new TimeSpan(), responseGroup);

            Entries items = new Entries();

            if (dto.CatalogEntry.Count > 0)
                items = LoadEntries(dto, null, true, responseGroup);

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, items, cacheTimeout);
            }

            return items;
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entry GetCatalogEntry(int catalogEntryId, CatalogEntryResponseGroup responseGroup)
        {
            CatalogEntryDto dto = GetCatalogEntryDto(catalogEntryId, responseGroup);

            Entry entry = null;

            // Load entry
            if (dto.CatalogEntry.Count > 0)
                entry = LoadEntry(dto.CatalogEntry[0], true, responseGroup);
/*
            else
                entry = new Entry();
 * */

            return entry;
        }

        /// <summary>
        /// Gets the catalog entry by URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entry GetCatalogEntryByUri(string uri, string languageCode, CatalogEntryResponseGroup responseGroup)
        {
            CatalogEntryDto dto = GetCatalogEntryByUriDto(uri, languageCode, responseGroup);

            Entry entry = null;

            // Load entry
            if (dto.CatalogEntry.Count > 0)
                entry = LoadEntry(dto.CatalogEntry[0], true, responseGroup);
/*
            else
                entry = new Entry();
 * */

            return entry;
        }

        /// <summary>
        /// Gets the catalog entries.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entries GetCatalogEntries(string catalogName, string catalogNodeCode, CatalogEntryResponseGroup responseGroup)
        {
            CatalogEntryDto dto = GetCatalogEntriesByCatalogDto(catalogName, catalogNodeCode, responseGroup);

            Entries entries = new Entries();

            // Load entry
            if (dto.CatalogEntry.Count > 0)
                entries = LoadEntries(dto, null, true, responseGroup);

            return entries;
        }

        /// <summary>
        /// Gets the catalog entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entry GetCatalogEntry(string code, CatalogEntryResponseGroup responseGroup)
        {
            CatalogEntryDto dto = GetCatalogEntryDto(code, responseGroup);

            Entry entry = null;

            // Load entry
            if (dto.CatalogEntry.Count > 0)
            {
                entry = LoadEntry(dto.CatalogEntry[0], true, responseGroup);
            }
/*
            else
                entry = new Entry();
 * */

            return entry;
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entry LoadEntry(CatalogEntryDto.CatalogEntryRow row, bool recursive, CatalogEntryResponseGroup responseGroup)
        {
            StringCollection entryList = new StringCollection();
            return LoadEntry(row, recursive, responseGroup, ref entryList);
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="entryList">The entry list.</param>
        /// <returns></returns>
        internal static Entry LoadEntry(CatalogEntryDto.CatalogEntryRow row, bool recursive, CatalogEntryResponseGroup responseGroup, ref StringCollection entryList)
        {
            Entry entry = null;

           // Load entry
            if (row != null)
            {
                // Track entries added, to avoid circular dependencies
                entryList.Add(row.Code);

                entry = new Entry(row);

                // Populate association detailed info
                if (recursive && (responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull) || responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.Associations)))
                {
                    if (entry.Associations != null)
                    {
                        CatalogAssociationDto associationDto = CatalogAssociationManager.GetCatalogAssociationDtoByEntryId(row.CatalogEntryId);

                        // If associations do not contain any entries, then we do not need to go through the rest
                        if (associationDto.CatalogEntryAssociation.Count > 0)
                        {
                            foreach (Association association in entry.Associations)
                            {
                                int associationId = 0;
                                // Find out association id
                                foreach(CatalogAssociationDto.CatalogAssociationRow associationRow in associationDto.CatalogAssociation)
                                {
                                    if(associationRow.AssociationName.Equals(association.Name))
                                    {
                                        associationId = associationRow.CatalogAssociationId;
                                        break;
                                    }
                                }

                                // Load association entries
                                List<EntryAssociation> entryAssociationList = new List<EntryAssociation>();
                                CatalogEntryDto associatedEntries = GetAssociatedCatalogEntriesDto(row.CatalogEntryId, association.Name, responseGroup);
                                foreach (CatalogEntryDto.CatalogEntryRow childRow in associatedEntries.CatalogEntry)
                                {
                                    EntryAssociation entryAssociation = new EntryAssociation();    
                                    // Find appropriate row
                                    CatalogAssociationDto.CatalogEntryAssociationRow entryAssociationRow = associationDto.CatalogEntryAssociation.FindByCatalogAssociationIdCatalogEntryId(associationId, childRow.CatalogEntryId);
                                    if (entryAssociationRow != null)
                                    {
                                        entryAssociation.SortOrder = entryAssociationRow.SortOrder;
                                        entryAssociation.AssociationType = entryAssociationRow.AssociationTypeId;
                                        entryAssociation.AssociationDesc = entryAssociationRow.AssociationTypeRow.Description;
                                    }

                                    // Check for circular dependencies here
                                    /*
                                    if (row.CatalogEntryId == childRow.CatalogEntryId)
                                        throw new CircularDependencyException(String.Format("Circular dependency detected. Entry association \"{0}\" for \"{1}[{2}]\" contains reference to itself.", entryAssociation.AssociationDesc, childRow.Name, childRow.CatalogEntryId));
                                     * */

                                    bool loadRecursive = recursive;
                                    
                                    // do not load recursive if there is potential circular dependency
                                    if (entryList.Contains(row.Code))
                                        loadRecursive = false;

                                    Entry childEntry = LoadEntry(childRow, loadRecursive, responseGroup, ref entryList);
                                    childEntry.ParentEntry = null;
                                    entryAssociation.Entry = childEntry;

                                    entryAssociationList.Add(entryAssociation);
                                }

                                association.EntryAssociations = new EntryAssociations();
                                association.EntryAssociations.Association = entryAssociationList.ToArray();
                            }
                        }
                    }
                }

                // Populate children
                if (recursive && (responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull) || responseGroup.ContainsGroup(CatalogEntryResponseGroup.ResponseGroup.Children)))
                {
                    bool loadRecursive = recursive;

                    // do not load recursive if there is potential circular dependency
                    if (entryList.Contains(row.Code))
                        loadRecursive = false;

                    CatalogEntryDto childrenDto = GetCatalogEntriesDto(row.CatalogEntryId, String.Empty, String.Empty, responseGroup);
                    Entries entries = LoadEntries(childrenDto, entry, loadRecursive, responseGroup, ref entryList);
                    entry.Entries = entries;
                }
            }
            /*
            else
                entry = new Entry();
             * */

            return entry;
        }

        /// <summary>
        /// Loads the entries.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static Entries LoadEntries(CatalogEntryDto dto, Entry parent, bool recursive, CatalogEntryResponseGroup responseGroup)
        {
            StringCollection entryList = new StringCollection();
            return LoadEntries(dto, parent, recursive, responseGroup, ref entryList);
        }

        /// <summary>
        /// Loads the entries.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="entryList">The entry list.</param>
        /// <returns></returns>
        internal static Entries LoadEntries(CatalogEntryDto dto, Entry parent, bool recursive, CatalogEntryResponseGroup responseGroup, ref StringCollection entryList)
        {
            List<Entry> entries = new List<Entry>();

            foreach (CatalogEntryDto.CatalogEntryRow childRow in dto.CatalogEntry)
            {
                Entry childEntry = LoadEntry(childRow, recursive, responseGroup, ref entryList);
                if (childEntry != null)
                {
                    childEntry.ParentEntry = parent;
                    entries.Add(childEntry);
                }
            }

            Entries en = new Entries();
            en.Entry = entries.ToArray();
            en.TotalResults = dto.CatalogEntry.Count;
            en.TotalPages = 1;
            return en;
        }
        #endregion
    }
}