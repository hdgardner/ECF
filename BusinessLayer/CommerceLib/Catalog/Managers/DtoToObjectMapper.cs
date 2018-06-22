using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Dto;
using System.Collections;
using System.Data;

namespace Mediachase.Commerce.Catalog.Managers
{
    /// <summary>
    /// Provides mapping between stateless objects and DTO objects
    /// </summary>
    public static class DtoToObjectMapper
    {
        /// <summary>
        /// Creates the site catalog object from the DTO CatalogRow.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>SiteCatalog instance</returns>
        public static SiteCatalog CreateSiteCatalog(CatalogDto.CatalogRow input)
        {
            SiteCatalog node = new SiteCatalog();

			if (input != null)
			{
				node.Name = input.Name;
				// Catalog Name is it's id
				node.CatalogId = input.Name;
			}
            return node;
        }

        /// <summary>
        /// Creates the catalog node.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        internal static CatalogNode CreateCatalogNode(CatalogNodeDto dto)
        {
            CatalogNode node = new CatalogNode();

            foreach (CatalogNodeDto.CatalogNodeRow row in dto.CatalogNode)
            {
                node.Name = row.Name;
                //node.NodeId = row.CatalogNodeId;
            }

            return node;
        }
    }
}