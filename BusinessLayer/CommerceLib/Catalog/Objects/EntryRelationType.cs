using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the catalog entry relation types.
    /// </summary>
    public class EntryRelationType
    {
        /// <summary>
        /// Public string literal for the product variation relation type.
        /// </summary>
        public const string ProductVariation = "ProductVariation";
        /// <summary>
        /// Public string literal for the package entry relation type.
        /// </summary>
        public const string PackageEntry = "PackageEntry";
        /// <summary>
        /// Public string literal for the bundle entry relation type.
        /// </summary>
        public const string BundleEntry = "BundleEntry";
    }
}