using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing.Managers
{
    /// <summary>
    /// Policy manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class PolicyManager
    {
        #region Policy Functions
        /// <summary>
        /// Gets the policy dto.
        /// </summary>
        /// <returns></returns>
        public static PolicyDto GetPolicyDto()
        {
            return GetPolicyDto(0);
        }

        /// <summary>
        /// Gets the Policy dto, checks permissions and caches results.
        /// </summary>
        /// <param name="policyId">The policy id.</param>
        /// <returns></returns>
        public static PolicyDto GetPolicyDto(int policyId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            //string cacheKey = MarketingCache.CreateCacheKey("Policy", PolicyId.ToString());

            PolicyDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
            //    dto = (PolicyDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                PolicyAdmin Policy = new PolicyAdmin();
                Policy.Load(policyId);
                dto = Policy.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.PolicyCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Policy Functions
        /// <summary>
        /// Saves the policy.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SavePolicy(PolicyDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("PolicyDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (PolicyDto.PolicyRow row in dto.Policy.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.PolicySecurity, String.Format("PolicyId = -1 or PolicyId = {0}", row.PolicyId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(PolicyScope.Policy, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            PolicyAdmin admin = new PolicyAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
