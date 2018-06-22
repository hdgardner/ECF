using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing.Managers
{
    /// <summary>
    /// Promotion manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class PromotionManager
    {
        #region Promotion Functions
        /// <summary>
        /// Loads all promotions.
        /// </summary>
        /// <returns></returns>
        public static PromotionDto GetPromotionDto()
        {
            return GetPromotionDto(0);
        }

        /// <summary>
        /// Gets promotion dto based on the date passed. Only promotions that are active for the period specified (plus 1 week) will be returned.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static PromotionDto GetPromotionDto(DateTime dateTime)
        {
            PromotionAdmin admin = new PromotionAdmin();
            admin.Load(dateTime);
            return admin.CurrentDto;
        }

        /// <summary>
        /// Gets the promotion usage dto. Results are not cached.
        /// </summary>
        /// <param name="promotionId">The promotion id.</param>
        /// <param name="customerId">The customer id.</param>
        /// <param name="orderGroupId">The order group id.</param>
        /// <returns></returns>
        public static PromotionUsageDto GetPromotionUsageDto(int promotionId, Guid customerId, int orderGroupId)
        {
            PromotionUsageDto dto = null;

            // Load the object
            if (dto == null)
            {
                PromotionUsageAdmin admin = new PromotionUsageAdmin();
                admin.Load(promotionId, customerId, orderGroupId);
                dto = admin.CurrentDto;
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the promotion usage statistics.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetPromotionUsageStatistics()
        {
            return GetPromotionUsageStatistics(Guid.Empty);
        }

        /// <summary>
        /// Gets the promotion usage statistics.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static DataTable GetPromotionUsageStatistics(Guid customerId)
        {
            PromotionUsageAdmin admin = new PromotionUsageAdmin();
            return admin.LoadStatistics(customerId);
        }

        /// <summary>
        /// Gets the Promotion dto without caching the results.
        /// </summary>
        /// <param name="promotionId">The promotion id.</param>
        /// <returns></returns>
        public static PromotionDto GetPromotionDto(int promotionId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            //string cacheKey = MarketingCache.CreateCacheKey("Promotion", PromotionId.ToString());

            PromotionDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
            //    dto = (PromotionDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                PromotionAdmin admin = new PromotionAdmin();
                admin.Load(promotionId);
                dto = admin.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.PromotionCollectionTimeout);
            }

            return dto;
        }
        #endregion

        #region Edit Promotion Functions
        /// <summary>
        /// Saves the promotion.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SavePromotion(PromotionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("PromotionDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (PromotionDto.PromotionRow row in dto.Promotion.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.PromotionSecurity, String.Format("PromotionId = -1 or PromotionId = {0}", row.PromotionId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(PromotionScope.Promotion, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            PromotionAdmin admin = new PromotionAdmin(dto);
            admin.Save();
        }

        /// <summary>
        /// Saves the promotion usage.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SavePromotionUsage(PromotionUsageDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("PromotionUsageDto can not be null"));

            PromotionUsageAdmin admin = new PromotionUsageAdmin(dto);
            admin.Save();
        }

        #endregion
    }
}
