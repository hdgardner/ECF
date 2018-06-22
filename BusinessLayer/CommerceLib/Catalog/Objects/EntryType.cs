using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the catalog entry types.
    /// </summary>
    public class EntryType
    {
        /// <summary>
        /// Public string literal for the product entry type.
        /// </summary>
        public const string Product = "Product";
        /// <summary>
        /// Public string literal for the variation entry type.
        /// </summary>
        public const string Variation = "Variation";
        /// <summary>
        /// Public string literal for the package entry type.
        /// </summary>
        public const string Package = "Package";
        /// <summary>
        /// Public string literal for the bundle entry type.
        /// </summary>
        public const string Bundle = "Bundle";
        /// <summary>
        /// Public string literal for the dynamic package entry type.
        /// </summary>
        public const string DynamicPackage = "DynamicPackage";
    }
}