using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// The ItemType enumeration defines the catalog object item types.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Represents the product type.
        /// </summary>
        Product,
        /// <summary>
        /// Represents the variation type.
        /// </summary>
        Variation,
        /// <summary>
        /// Represents the package type.
        /// </summary>
        Package,
        /// <summary>
        /// Represents the bundle type.
        /// </summary>
        Bundle,
        /// <summary>
        /// Represents the dynamic package type.
        /// </summary>
        DynamicPackage,
        /// <summary>
        /// Represents the user-defined type Custom1.
        /// </summary>
        Custom1,
        /// <summary>
        /// Represents the user-defined type Custom2.
        /// </summary>
        Custom2,
        /// <summary>
        /// Represents the user-defined type Custom3.
        /// </summary>
        Custom3
    }
}
