using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Status for the promotion usage.
    /// </summary>
    public enum PromotionUsageStatus
    {
        /// <summary>
        /// The promotion has expired.
        /// </summary>
        Expired = 0,
        /// <summary>
        /// The promotion is currently reserved.
        /// </summary>
        Reserved = 1,
        /// <summary>
        /// The promotion has been used.
        /// </summary>
        Used = 2
    }
}
