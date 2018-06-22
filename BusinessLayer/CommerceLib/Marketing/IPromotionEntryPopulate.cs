using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog.Objects;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Promotion entry populate interface. This interface will be used to populate promotion entry object with attributes from the line item or other object.
    /// </summary>
    public interface IPromotionEntryPopulate
    {
        /// <summary>
        /// Populates the specified promotion entry with attribute values from the object.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="val">The val.</param>
        void Populate(ref PromotionEntry promoEntry, object item);
    }
}
