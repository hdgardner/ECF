using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Data;

namespace Mediachase.Commerce.Catalog.Managers
{
    public static class CatalogLogManager
    {
        /// <summary>
        /// Gets the catalog log.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="created">The created.</param>
        /// <param name="startingRecord">The starting record.</param>
        /// <param name="numberOfRecords">The number of records.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static CatalogLogDto GetCatalogLog(string operation, string objectType, DateTime created, int startingRecord, int numberOfRecords, ref int totalRecords)
        {
            CatalogLogDto dto = null;
            CatalogLogAdmin admin = new CatalogLogAdmin();
            admin.Load(operation, objectType, created, startingRecord, numberOfRecords, ref totalRecords);
            dto = admin.CurrentDto;
            return dto;
        }

        /// <summary>
        /// Saves the log.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveLog(CatalogLogDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CatalogLogDto can not be null"));

            CatalogLogAdmin admin = new CatalogLogAdmin(dto);
            admin.Save();
        }
    }
}
