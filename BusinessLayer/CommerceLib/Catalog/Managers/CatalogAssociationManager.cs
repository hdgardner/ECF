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

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Implements operations for the catalog association manager.
    /// </summary>
    public static class CatalogAssociationManager
    {
        /// <summary>
        /// Finds the items dto.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        /// <returns></returns>
        internal static CatalogAssociationDto GetCatalogAssociationDto(int catalogAssociationId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalogassociation", catalogAssociationId.ToString());

            CatalogAssociationDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CatalogAssociationDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
                admin.Load(catalogAssociationId);
                dto = admin.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the catalog association dto.
        /// </summary>
        /// <param name="catalogAssociationName">Name of the catalog association.</param>
        /// <returns></returns>
        internal static CatalogAssociationDto GetCatalogAssociationDto(string catalogAssociationName)
        {
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogassociation-name-", catalogAssociationName);

			CatalogAssociationDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogAssociationDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
				admin.Load(catalogAssociationName);
				dto = admin.CurrentDto;

				// Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
			}

			dto.AcceptChanges();

			return dto;
        }

        /// <summary>
        /// Gets the catalog association dto by entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
		internal static CatalogAssociationDto GetCatalogAssociationDtoByEntryId(int catalogEntryId)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogassociation-catalogEntryId-", catalogEntryId.ToString());

			CatalogAssociationDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogAssociationDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
				admin.LoadByCatalogEntryId(catalogEntryId);
				dto = admin.CurrentDto;

				// Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
			}

			dto.AcceptChanges();

			return dto;
		}

        /// <summary>
        /// Gets the catalog association dto by entry code.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <returns></returns>
		internal static CatalogAssociationDto GetCatalogAssociationDtoByEntryCode(int catalogId, string catalogEntryCode)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogassociation-catalogId-catalogEntryCode-", catalogId.ToString(), catalogEntryCode);

			CatalogAssociationDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogAssociationDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
				admin.LoadByCatalogEntryCode(catalogId, catalogEntryCode);
				dto = admin.CurrentDto;

				// Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
			}

			dto.AcceptChanges();

			return dto;
		}

		/// <summary>
		/// Gets the catalog association dto by catalog id.
		/// </summary>
		/// <param name="catalogId">The catalog id.</param>
		/// <returns></returns>
		internal static CatalogAssociationDto GetCatalogAssociationDtoByCatalogId(int catalogId)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalogassociation-catalogId-", catalogId.ToString());

			CatalogAssociationDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogAssociationDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
				admin.LoadByCatalogId(catalogId);
				dto = admin.CurrentDto;

				// Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogCollectionTimeout);
			}

			dto.AcceptChanges();

			return dto;
		}

        /// <summary>
        /// Saves the catalog association.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal static void SaveCatalogAssociation(CatalogAssociationDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogAssociationDto can not be null"));

            CatalogAssociationAdmin admin = new CatalogAssociationAdmin(dto);
            EventContext.Instance.RaiseAssociationUpdatingEvent(dto, new AssociationEventArgs("updating"));
            admin.Save();
            EventContext.Instance.RaiseAssociationUpdatedEvent(dto, new AssociationEventArgs("updated"));
        }

		/// <summary>
		/// Saves the catalog association type.
		/// </summary>
		/// <param name="dto">The dto.</param>
		internal static void SaveAssociationType(CatalogAssociationDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("CatalogAssociationDto can not be null"));

			CatalogAssociationAdmin admin = new CatalogAssociationAdmin(dto);
			admin.SaveAssociationType();
		}
	}
}