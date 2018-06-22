using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing.Managers
{
    /// <summary>
    /// Segment manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class SegmentManager
    {
        #region Segment Functions
        /// <summary>
        /// Gets the segment dto.
        /// </summary>
        /// <returns></returns>
        public static SegmentDto GetSegmentDto()
        {
            return GetSegmentDto(0);
        }

        /// <summary>
        /// Gets the Segment dto, checks permissions and caches results.
        /// </summary>
        /// <param name="segmentId">The segment id.</param>
        /// <returns></returns>
        public static SegmentDto GetSegmentDto(int segmentId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            //string cacheKey = MarketingCache.CreateCacheKey("Segment", SegmentId.ToString());

            SegmentDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
            //    dto = (SegmentDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                SegmentAdmin Segment = new SegmentAdmin();
                Segment.Load(segmentId);
                dto = Segment.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.SegmentCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Segment Functions
        /// <summary>
        /// Saves the segment.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveSegment(SegmentDto dto)
        {
            if (dto == null)
				throw new ArgumentNullException("dto", String.Format("SegmentDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (SegmentDto.SegmentRow row in dto.Segment.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.SegmentSecurity, String.Format("SegmentId = -1 or SegmentId = {0}", row.SegmentId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(SegmentScope.Segment, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            SegmentAdmin admin = new SegmentAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
