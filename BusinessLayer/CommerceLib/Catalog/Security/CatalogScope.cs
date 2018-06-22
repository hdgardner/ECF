using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Security
{
    /// <summary>
    /// Represents the catalog security scopes.
    /// </summary>
    public static class SecurityScope
    {
        /// <summary>
        /// Public string literal for the site scope.
        /// </summary>
        public const string Site = "Site";
        /// <summary>
        /// Public string literal for the catalog scope.
        /// </summary>
        public const string Catalog = "Catalog";
        /// <summary>
        /// Public string literal for the order scope.
        /// </summary>
        public const string Order = "Order";
        /// <summary>
        /// Public string literal for the catalog node scope.
        /// </summary>
        public const string CatalogNode = "CatalogNode";
        /// <summary>
        /// Public string literal for the catalog entry scope.
        /// </summary>
        public const string CatalogEntry = "CatalogEntry";
    }
}