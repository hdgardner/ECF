using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Security;
using System.Data;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Search;
using System.Web.Security;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Catalog.Events;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Catalog manage acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class CatalogManager
    {
        #region Catalog Functions
        /// <summary>
        /// Returns the collection of catalogs with data populated that is specified in the responseGroup parameter.
        /// The data is cached according to the configuration.
        /// </summary>
        /// <param name="siteGuid">The site GUID.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public static SiteCatalogs GetCatalogs(Guid siteGuid, CatalogResponseGroup responseGroup)
        {
            // Retrieve dto
            CatalogDto dto = GetCatalogDto(siteGuid, responseGroup);

            // Continue with security checks and other operations
            SiteCatalogs nodes = new SiteCatalogs();
            List<SiteCatalog> nodeArray = new List<SiteCatalog>();

            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                nodeArray.Add(DtoToObjectMapper.CreateSiteCatalog(row));
            }

            nodes.Catalog = nodeArray.ToArray();
            return nodes;
        }

        /// <summary>
        /// Gets the catalog dto, checks permissions and caches results.
        /// </summary>
        /// <param name="siteGuid">The site GUID.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public static CatalogDto GetCatalogDto(Guid siteGuid, CatalogResponseGroup responseGroup)
        {
            /*
            // Checks roles first
            if (!SecurityManager.CheckPermission(new string[] { CatalogRoles.CatalogAdminRole, CatalogRoles.CatalogManagerRole, CatalogRoles.CatalogViewerRole }))
                return new CatalogDto();
             * */

            // Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogs", responseGroup != null ? responseGroup.CacheKey : "", siteGuid.ToString());

            CatalogDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogAdmin catalog = new CatalogAdmin();
                catalog.Load(siteGuid);
                dto = catalog.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            // Continue with security checks and other operations
			/*
            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.CatalogSecurity, String.Format("CatalogId = -1 or CatalogId = {0}", row.CatalogId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(SecurityScope.Catalog.ToString(), Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
                
            }
			 * */

            //dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog dto, checks permissions and caches results.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        public static CatalogDto GetCatalogDto(int catalogId, CatalogResponseGroup responseGroup)
        {
            /*
            // Checks roles first
            if (!SecurityManager.CheckPermission(new string[] { CatalogRoles.CatalogAdminRole, CatalogRoles.CatalogManagerRole, CatalogRoles.CatalogViewerRole }))
                return new CatalogDto();
             * */

            // Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalog", responseGroup != null ? responseGroup.CacheKey : "", catalogId.ToString());

            CatalogDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogAdmin catalog = new CatalogAdmin();
                catalog.Load(catalogId);
                dto = catalog.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            // Continue with security checks and other operations
            /*
            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.CatalogSecurity, String.Format("CatalogId = -1 or CatalogId = {0}", row.CatalogId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(SecurityScope.Catalog.ToString(), Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */

            //dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Catalog Functions
        /// <summary>
        /// Saves the catalog.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveCatalog(CatalogDto dto)
        {
            /*
            // Checks roles first
            if (!ProfileConfiguration.Instance.EnablePermissions)
            {
                if (!SecurityManager.CheckPermission(new string[] { CatalogRoles.CatalogAdminRole, CatalogRoles.CatalogAdminRole }))
                    return;
            }
             * */

            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogDto can not be null"));

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
            
            CatalogAdmin admin = new CatalogAdmin(dto);
            EventContext.Instance.RaiseCatalogUpdatingEvent(dto, new CatalogEventArgs("updating"));
            admin.Save();
            EventContext.Instance.RaiseCatalogUpdatedEvent(dto, new CatalogEventArgs("updated"));
        }
        #endregion

        /// <summary>
        /// Deletes the catalog.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        public static void DeleteCatalog(int catalogId)
        {
            CatalogDto dto = GetCatalogDto(catalogId, new CatalogResponseGroup(CatalogResponseGroup.ResponseGroup.CatalogFull));
            if (dto.Catalog.Count == 0)
                return;

            //Delete CatalogItemAsset rows by CatalogId
            CatalogNodeDto catalogNodeDto = CatalogNodeManager.GetCatalogNodesDto(catalogId, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.Assets));
            if (catalogNodeDto.CatalogNode.Count > 0)
            {
                for (int i1 = 0; i1 < catalogNodeDto.CatalogItemAsset.Count; i1++)
                {
                    catalogNodeDto.CatalogItemAsset[i1].Delete();
                }

                CatalogNodeManager.SaveCatalogNode(catalogNodeDto);
            }

            // delete relations
            CatalogRelationDto catalogRelationDto = CatalogRelationManager.GetCatalogRelationDto(catalogId, 0, 0, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry | CatalogRelationResponseGroup.ResponseGroup.CatalogEntry | CatalogRelationResponseGroup.ResponseGroup.CatalogNode));

            foreach (CatalogRelationDto.NodeEntryRelationRow row in catalogRelationDto.NodeEntryRelation.Rows)
                row.Delete();

            foreach (CatalogRelationDto.CatalogEntryRelationRow row in catalogRelationDto.CatalogEntryRelation.Rows)
                row.Delete();

            foreach (CatalogRelationDto.CatalogNodeRelationRow row in catalogRelationDto.CatalogNodeRelation.Rows)
                row.Delete();

            if (catalogRelationDto.HasChanges())
                CatalogRelationManager.SaveCatalogRelation(catalogRelationDto);

            //Delete CatalogItemSeo rows by CatalogNodeId and CatalogNode rows
            catalogNodeDto = CatalogNodeManager.GetCatalogNodesDto(catalogId, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeFull));
            if (catalogNodeDto.CatalogNode.Count > 0)
            {
                for (int i1 = 0; i1 < catalogNodeDto.CatalogItemSeo.Count; i1++)
                {
                    catalogNodeDto.CatalogItemSeo[i1].Delete();
                }

                for (int i1 = 0; i1 < catalogNodeDto.CatalogNode.Count; i1++)
                {
                    catalogNodeDto.CatalogNode[i1].Delete();
                }

                CatalogNodeManager.SaveCatalogNode(catalogNodeDto);
            }

            //Delete entries
            while (true)
            {
                CatalogSearchParameters pars = new CatalogSearchParameters();
                CatalogSearchOptions options = new CatalogSearchOptions();

                options.Namespace = String.Empty;
                options.RecordsToRetrieve = 100;
                options.StartingRecord = 0;
                pars.CatalogNames.Add(dto.Catalog[0].Name);

                int totalCount = 0;
                CatalogEntryDto catalogEntryDto = CatalogContext.Current.FindItemsDto(pars, options, ref totalCount, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

                //Delete CatalogEntryAssociation rows
                foreach (CatalogEntryDto.CatalogAssociationRow catalogAssociationRow in catalogEntryDto.CatalogAssociation)
                {
                    CatalogAssociationDto catalogAssociationDto = FrameworkContext.Current.CatalogSystem.GetCatalogAssociationDto(catalogAssociationRow.CatalogAssociationId);
                    foreach (CatalogAssociationDto.CatalogEntryAssociationRow itemCatalogEntryAssociation in catalogAssociationDto.CatalogEntryAssociation)
                    {
                        itemCatalogEntryAssociation.Delete();
                    }

                    if (catalogAssociationDto.HasChanges())
                        CatalogContext.Current.SaveCatalogAssociation(catalogAssociationDto);
                }

                //Delete CatalogEntry rows
                foreach (CatalogEntryDto.CatalogEntryRow catalogEntryRow in catalogEntryDto.CatalogEntry)
                {
                    if (catalogEntryRow.InventoryRow != null)
                        catalogEntryRow.InventoryRow.Delete();
                    catalogEntryRow.Delete();
                }

                CatalogContext.Current.SaveCatalogEntry(catalogEntryDto);

                // Break the loop if we retrieved all the record
                if (totalCount < options.RecordsToRetrieve)
                    break;
            }

            // Delete root entries
            CatalogEntryDto rootCatalogEntries = CatalogEntryManager.GetCatalogEntriesDto(catalogId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

            foreach (CatalogEntryDto.CatalogEntryRow catalogEntryRow in rootCatalogEntries.CatalogEntry)
            {
                if (catalogEntryRow.InventoryRow != null)
                    catalogEntryRow.InventoryRow.Delete();
                catalogEntryRow.Delete();
            }

            CatalogEntryManager.SaveCatalogEntry(rootCatalogEntries);

            //Delete Catalog row by id
            dto.Catalog[0].Delete();
            SaveCatalog(dto);
        }

        /*
        public static Entries FindItems(CatalogSearch search)
        {
            CatalogEntryAdmin admin = new CatalogEntryAdmin();
            CatalogSearchParameters pars = search.SearchParameters;
            CatalogSearchOptions opt = search.SearchOptions;
            int recordsCount = 0;
            
            admin.Search(
                pars.CatalogNames.ToString(), 
                pars.Language, 
                opt.ResponseGroupsToReturn.ToString(), 
                pars.SqlWhereClause, pars.CategoriesClause, 
                pars.FreeTextSearchPhrase, 
                pars.AdvancedFreeTextSearchPhrase,
                String.Empty,
                opt.ClassesToReturn.ToString(),
                opt.StartingRecord, 
                opt.RecordsToRetrieve, 
                !opt.SortDescending,
                ref recordsCount, 
                false, 
                false, 
                false, 
                false, 
                pars.Recursive, 
                true);
            Entries items = new Entries();
            //items.TotalResults = recordsCount;
            //items.TotalPages = (int)(recordsCount / opt.RecordsToRetrieve);

            return items;
        }
        */
    }
}
