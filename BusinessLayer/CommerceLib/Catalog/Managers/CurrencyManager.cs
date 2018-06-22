using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Implements operations for the currency manager.
    /// </summary>
    public static class CurrencyManager
    {
        /// <summary>
        /// Gets the currency dto.
        /// </summary>
        /// <returns></returns>
        public static CurrencyDto GetCurrencyDto()
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CatalogCache.CreateCacheKey("catalog-currency", CatalogConfiguration.Instance.ApplicationId.ToString());

            CurrencyDto dto = null;

            // check cache first
            object cachedObject = CatalogCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (CurrencyDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CurrencyAdmin admin = new CurrencyAdmin();
                admin.Load();
                dto = admin.CurrentDto;

                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogSchemaTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }

		/// <summary>
		/// Gets the currency dto by currencyId.
		/// </summary>
		/// <returns></returns>
		public static CurrencyDto GetCurrencyByCurrencyId(int currencyId)
		{
			CurrencyAdmin admin = new CurrencyAdmin();
			admin.LoadByCurrencyId(currencyId);
			return admin.CurrentDto;
		}

		/// <summary>
		/// Gets the currency dto by currencyCode.
		/// </summary>
		/// <returns></returns>
		public static CurrencyDto GetCurrencyByCurrencyCode(string currencyCode)
		{
			CurrencyAdmin admin = new CurrencyAdmin();
			admin.LoadByCurrencyCode(currencyCode);
			return admin.CurrentDto;
		}

        /// <summary>
        /// Saves the currency.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveCurrency(CurrencyDto dto)
        {
            if (dto == null)
				throw new ArgumentNullException("dto", String.Format("CurrencyDto can not be null"));

            CurrencyAdmin admin = new CurrencyAdmin(dto);
            admin.Save();
        }
    }
}