using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Data;
using Mediachase.Cms.Objects;
using System.IO;
using System.Xml;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Managers
{
    public static class SiteManager
    {
        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <param name="appGuid">The app GUID.</param>
        /// <returns></returns>
        public static SiteDto GetSites(Guid appGuid)
        {
			return GetSites(appGuid, false);
        }

		/// <summary>
		/// Gets the sites.
		/// </summary>
		/// <param name="appGuid">The app GUID.</param>
		/// <returns></returns>
		public static SiteDto GetSites(Guid appGuid, bool returnInactive)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CmsCache.CreateCacheKey("sites", appGuid.ToString(), returnInactive.ToString());

			SiteDto dto = null;

			// check cache first
			object cachedObject = CmsCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (SiteDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				SiteAdmin admin = new SiteAdmin();
				admin.LoadByApplication(appGuid, returnInactive);
				dto = admin.CurrentDto;

				// Insert to the cache collection
				CmsCache.Insert(cacheKey, dto, CmsConfiguration.Instance.Cache.SitesCollectionTimeout);
			}

			// Continue with security checks and other operations
			/*
			foreach (SiteDto.SiteRow row in dto.Site.Rows)
			{
				// Check Security
				IDataReader reader = DataHelper.CreateDataReader(dto.SiteSecurity, String.Format("SiteId = '{0}' or SiteId = '{1}'", Guid.Empty, row.SiteId));
				PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
				if (!PermissionManager.CheckPermission(SecurityScope.Site.ToString(), Permission.Read, recordSet))
				{
					row.Delete();
					continue;
				}
			}
			 * */

			if (dto.HasChanges())
				dto.AcceptChanges();

			return dto;
		}

		/// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
		public static SiteDto GetSite(Guid siteId)
		{
			return GetSite(siteId, false);
		}

        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="siteId">The site id.</param>
		/// <param name="returnInactive"></param>
        /// <returns></returns>
        public static SiteDto GetSite(Guid siteId, bool returnInactive)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CmsCache.CreateCacheKey("site", siteId.ToString());

            SiteDto dto = null;

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (SiteDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        dto = (SiteDto)cachedObject;
                    }
                    else
                    {
                        SiteAdmin admin = new SiteAdmin();
                        admin.Load(siteId, returnInactive);
                        dto = admin.CurrentDto;

                        // Insert to the cache collection
                        CmsCache.Insert(cacheKey, dto, CmsConfiguration.Instance.Cache.SitesCollectionTimeout);
                    }
                }
            }

            // Continue with security checks and other operations
            /*
            foreach (SiteDto.SiteRow row in dto.Site.Rows)
            {
                // Check Security
                IDataReader reader = DataHelper.CreateDataReader(dto.SiteSecurity, String.Format("SiteId = '{0}' or SiteId = '{1}'", Guid.Empty, row.SiteId));
                PermissionRecordSet recordSet = new PermissionRecordSet(PermissionHelper.ConvertReaderToRecords(reader));
                if (!PermissionManager.CheckPermission(SecurityScope.Site.ToString(), Permission.Read, recordSet))
                {
                    row.Delete();
                    continue;
                }
            }
             * */

            if(dto.HasChanges())
                dto.AcceptChanges();

            return dto;
        }

        /// <summary>
        /// Saves the site.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveSite(SiteDto dto)
        {
            /*
            if (!ProfileConfiguration.Instance.EnablePermissions)
            {
                if (!SecurityManager.CheckPermission(new string[] { CmsRoles.AdminRole }))
                    return;
            }
             * */

            if (dto == null)
                throw new NullReferenceException(String.Format("SiteDto can not be null"));

            SiteAdmin admin = new SiteAdmin(dto);
            admin.Save();
        }

        #region Object Methods
        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns></returns>
        internal static CmsSite GetSite(Guid siteId, SiteResponseGroup responseGroup)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = CmsCache.CreateCacheKey("site-object", siteId.ToString(), responseGroup.CacheKey);

            CmsSite site = null;

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
                site = (CmsSite)cachedObject;

            // Load the object
            if (site == null)
            {
                lock (CmsCache.GetLock(cacheKey))
                {
                    cachedObject = CmsCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        site = (CmsSite)cachedObject;
                    }
                    else
                    {
                        SiteDto siteDto = GetSite(siteId);

                        // Load main site parameters
                        site = ObjectHelper.CreateSite(siteDto.Site[0]);

                        // Load menus
                        if (responseGroup.ContainsGroup(SiteResponseGroup.ResponseGroup.Full) || responseGroup.ContainsGroup(SiteResponseGroup.ResponseGroup.Menus))
                        {
                            List<SiteMenu> menuList = new List<SiteMenu>();
                            MenuDto menuDto = MenuManager.GetMenuDto(siteId);

                            foreach (MenuDto.MenuRow row in menuDto.Menu)
                            {
                                menuList.Add(ObjectHelper.CreateMenu(menuDto, row));
                            }

                            site.Menus = menuList.ToArray();
                        }

                        // Insert to the cache collection
                        CmsCache.Insert(cacheKey, site, CmsConfiguration.Instance.Cache.SitesCollectionTimeout);
                    }
                }
            }

            return site;
        }
        #endregion
    }
}