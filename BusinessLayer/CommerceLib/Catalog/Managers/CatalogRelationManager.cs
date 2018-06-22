using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Events;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Implements operations for the catalog relation manager.
    /// </summary>
    public static class CatalogRelationManager
    {
        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CatalogRelationDto GetCatalogRelationDto(int catalogId, int catalogNodeId, int catalogEntryId, string groupName, CatalogRelationResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalog-relation", responseGroup.CacheKey, catalogId.ToString(), catalogNodeId.ToString(), catalogEntryId.ToString(), groupName);

            CatalogRelationDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogRelationDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogRelationAdmin admin = new CatalogRelationAdmin();
                admin.Load(catalogId, catalogNodeId, catalogEntryId, groupName, responseGroup.ResponseGroups.GetHashCode());
                dto = admin.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog nodes dto.
        /// </summary>
        /// <param name="assetKey">The asset key.</param>
        /// <returns></returns>
        internal static CatalogRelationDto GetCatalogRelationDto(string assetKey)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalog-relation-asset", assetKey);

            CatalogRelationDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogRelationDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogRelationAdmin admin = new CatalogRelationAdmin();
                admin.LoadAsset(assetKey);
                dto = admin.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }


        /// <summary>
        /// Saves the catalog relation.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal static void SaveCatalogRelation(CatalogRelationDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogRelationDto can not be null"));

            CatalogRelationAdmin admin = new CatalogRelationAdmin(dto);
            EventContext.Instance.RaiseRelationUpdatingEvent(dto, new RelationEventArgs("updating"));
            admin.Save();
            EventContext.Instance.RaiseRelationUpdatedEvent(dto, new RelationEventArgs("updated"));

			string cacheKey = CatalogCache.CreateCacheKey("catalog-relation");

			// remove cached items
			if(dto.CatalogEntryRelation.Rows.Count>0)
			{
				int catalogEntryId = ((CatalogRelationDto.CatalogEntryRelationRow)dto.CatalogEntryRelation.Rows[0]).ParentEntryId;
				CatalogCache.RemoveByPattern(String.Format(@"{0}-(\w*)-(\d+)-(\d+)-{1}-.*", cacheKey, catalogEntryId.ToString()));
			}

			if (dto.CatalogNodeRelation.Rows.Count > 0)
			{
				int catalogId = ((CatalogRelationDto.CatalogNodeRelationRow)dto.CatalogNodeRelation.Rows[0]).CatalogId;
				int catalogNodeId = ((CatalogRelationDto.CatalogNodeRelationRow)dto.CatalogNodeRelation.Rows[0]).ChildNodeId;
				CatalogCache.RemoveByPattern(String.Format(@"{0}-(\w*)-{1}-{2}-(\d+)-.*", cacheKey, catalogId.ToString(), catalogNodeId.ToString()));
			}

			// TODO: remove cached items in case if CatalogRelations are deleted
        }
    }
}
