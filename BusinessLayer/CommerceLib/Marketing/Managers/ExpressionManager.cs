using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Data;

namespace Mediachase.Commerce.Marketing.Managers
{
    /// <summary>
    /// Expression manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class ExpressionManager
    {
        #region Expression Functions
        /// <summary>
        /// Gets the expression dto.
        /// </summary>
        /// <returns></returns>
        public static ExpressionDto GetExpressionDto()
        {
            return GetExpressionDto(0);
        }

        /// <summary>
        /// Gets the Expression dto, checks permissions and caches results.
        /// </summary>
        /// <param name="expressionId">The expression id.</param>
        /// <returns></returns>
        public static ExpressionDto GetExpressionDto(int expressionId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            //string cacheKey = MarketingCache.CreateCacheKey("Expression", ExpressionId.ToString());

            ExpressionDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
              //  dto = (ExpressionDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                ExpressionAdmin Expression = new ExpressionAdmin();
                Expression.Load(expressionId);
                dto = Expression.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.ExpressionCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the ExpressionDto by category. The supported categories now are "Promotion", "Segment", "Policy".
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public static ExpressionDto GetExpressionDto(string category)
        {
            ExpressionDto dto = null;

            // Load the object
            if (dto == null)
            {
                ExpressionAdmin Expression = new ExpressionAdmin();
                Expression.LoadByCategory(category);
                dto = Expression.CurrentDto;
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the expression dto.
        /// </summary>
        /// <param name="segmentId">The segment id.</param>
        /// <returns></returns>
        public static ExpressionDto GetExpressionBySegmentDto(int segmentId)
        {
            ExpressionDto dto = null;

            // Load the object
            if (dto == null)
            {
                ExpressionAdmin Expression = new ExpressionAdmin();
                Expression.LoadBySegment(segmentId);
                dto = Expression.CurrentDto;
            }

            dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Expression Functions
        /// <summary>
        /// Saves the expression.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveExpression(ExpressionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("ExpressionDto can not be null"));

            //TODO: check concurrency when updating the records

            //TODO: need to check security roles here, 
            // The procedure will be following:
            // 1. Retrieve the record from the database for each category that is about to be updated
            // 2. Check Write permissions (if failed generate the error and exit)
            // 3. Otherwise proceed to update
            // Continue with security checks and other operations
            /*
            foreach (ExpressionDto.ExpressionRow row in dto.Expression.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.ExpressionSecurity, String.Format("ExpressionId = -1 or ExpressionId = {0}", row.ExpressionId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(ExpressionScope.Expression, Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */


            ExpressionAdmin admin = new ExpressionAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
