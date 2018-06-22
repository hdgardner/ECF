using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Data;

namespace Mediachase.Cms.Managers
{
    public static class DictionaryManager
    {
        #region Template Functions
        /// <summary>
        /// Gets the template dto.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <returns></returns>
        public static TemplateDto GetTemplateDto(int templateId)
        {
            // TODO: add caching
            TemplateAdmin admin = new TemplateAdmin();
            admin.Load(templateId);
            return admin.CurrentDto;
        }

        /// <summary>
        /// Gets the template path.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <returns></returns>
        public static string GetTemplatePath(int templateId)
        {
            TemplateDto dto = GetTemplateDto();
            foreach (TemplateDto.main_TemplatesRow row in dto.main_Templates)
            {
                if (row.TemplateId == templateId)
                {
                    return row.Path;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the template path.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="templateType">Type of the template.</param>
        /// <returns></returns>
        public static string GetTemplatePath(string name, string templateType)
        {
            TemplateDto dto = GetTemplateDto();
            foreach (TemplateDto.main_TemplatesRow row in dto.main_Templates)
            {
                if (row.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && row.TemplateType.Equals(templateType, StringComparison.OrdinalIgnoreCase))
                {
                    return row.Path;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Returns the full template dataset. Results are cached when not in design mode and not cached when in design mode.
        /// Use this method for all runtime calls.
        /// </summary>
        /// <returns></returns>
        public static TemplateDto GetTemplateDto()
        {
            bool useCache = !CMSContext.Current.IsDesignMode; // do not use template caching in design mode

            string cacheKey = CmsCache.CreateCacheKey("templates");

            // check cache first
            object cachedObject = null;

            if (useCache)
                cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
            {
                return (TemplateDto)cachedObject;
            }

            // TODO: add caching
            TemplateAdmin admin = new TemplateAdmin();
            admin.Load();

            // Insert to the cache collection
            if (useCache)
                CmsCache.Insert(cacheKey, admin.CurrentDto, CmsConfiguration.Instance.Cache.TemplateTimeout);

            return admin.CurrentDto;
        }

        /// <summary>
        /// Saves the template dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveTemplateDto(TemplateDto dto)
        {
            TemplateAdmin admin = new TemplateAdmin(dto);
            admin.Save();
        }
        #endregion
    }
}
