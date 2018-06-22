using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Core.Managers
{
    /// <summary>
    /// Implements operations for the application manager.
    /// </summary>
    public static class AppManager
    {
        #region Application Functions
        /// <summary>
        /// Gets the application dto.
        /// </summary>
        /// <returns></returns>
        public static AppDto GetApplicationDto()
        {
            return GetApplicationDto(Guid.Empty);
        }

        /// <summary>
        /// Gets the application dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static AppDto GetApplicationDto(string name)
        {
            AppDto dto = null;

            // Load the object
            if (dto == null)
            {
                AppAdmin admin = new AppAdmin();
                admin.LoadByApplication(name);
                dto = admin.CurrentDto;
            }

            dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Gets the application dto.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <returns></returns>
        public static AppDto GetApplicationDto(Guid appId)
        {
            // Assign new cache key, specific for site guid and response groups requested
            //string cacheKey = CoreCache.CreateCacheKey("Expression", ExpressionId.ToString());

            AppDto dto = null;

            // check cache first
            //object cachedObject = MarketingCache.Get(cacheKey);

            //if (cachedObject != null)
              //  dto = (ExpressionDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                AppAdmin admin = new AppAdmin();
                admin.LoadByApplication(appId);
                dto = admin.CurrentDto;

                // Insert to the cache collection
                //MarketingCache.Insert(cacheKey, dto, MarketingConfiguration.CacheConfig.ExpressionCollectionTimeout);
            }

            dto.AcceptChanges();

            return dto;
        }
        #endregion

        #region Edit Application Functions
        /// <summary>
        /// Saves the application.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveApplication(AppDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("AppDto can not be null"));

            /*
            // Checks roles first
            if (!ProfileConfiguration.Instance.EnablePermissions)
            {
                if (!SecurityManager.CheckPermission(new string[] { AppRoles.AdminRole }))
                    return;
            }
             * */

            AppAdmin admin = new AppAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
