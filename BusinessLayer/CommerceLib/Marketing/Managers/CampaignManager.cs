using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing.Managers
{
    /// <summary>
    /// Campaign manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class CampaignManager
    {
        #region Campaign Functions
        /// <summary>
        /// Gets the campaign dto.
        /// </summary>
        /// <returns></returns>
        public static CampaignDto GetCampaignDto()
        {
            return GetCampaignDto(0);
        }

        /// <summary>
        /// Gets the Campaign dto, checks permissions and caches results.
        /// </summary>
        /// <param name="campaignId">The campaign id.</param>
        /// <returns></returns>
        public static CampaignDto GetCampaignDto(int campaignId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            // string cacheKey = MarketingCache.CreateCacheKey("Campaign", CampaignId.ToString());

            CampaignDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
                //dto = (CampaignDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                CampaignAdmin campaign = new CampaignAdmin();
                campaign.Load(campaignId);
                dto = campaign.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.CampaignCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Campaign Functions
        /// <summary>
        /// Saves the campaign.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveCampaign(CampaignDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CampaignDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (CampaignDto.CampaignRow row in dto.Campaign.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.CampaignSecurity, String.Format("CampaignId = -1 or CampaignId = {0}", row.CampaignId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(CampaignScope.Campaign, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            CampaignAdmin admin = new CampaignAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
