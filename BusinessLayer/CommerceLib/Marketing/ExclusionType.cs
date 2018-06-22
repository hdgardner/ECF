using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Represents the marketing exclusion types.
    /// </summary>
    public static class ExclusionType
    {
        /// <summary>
        /// Public string literal for the non-exclusive type.
        /// </summary>
        public const string None = "none";
        /// <summary>
        /// Public string literal for the group level exclusion type.
        /// </summary>
        public const string GroupLevel = "group";
        /// <summary>
        /// Public string literal for the global level exclusion type.
        /// </summary>
        public const string GlobalLevel = "global";
    }
}
