using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Orders.Search
{
    /// <summary>
    /// Search Options.
    /// </summary>
    public class OrderSearchOptions : SearchOptions
    {
        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSearchOptions"/> class.
        /// </summary>
        public OrderSearchOptions() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSearchOptions"/> class.
        /// </summary>
        /// <param name="searchOptions">The search options.</param>
        public OrderSearchOptions(OrderSearchOptions searchOptions)
            : base(searchOptions)
        {
            Namespace = "Mediachase.Commerce.Orders";
        }
    }
}
