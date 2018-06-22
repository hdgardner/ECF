using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the collection of sale prices in the catalog.
    /// </summary>
    [DataContract]
    public partial class SalePrices
    {
        /// <summary>
        /// Represents the collection of sale prices.
        /// </summary>
        private SalePrice[] _SalePrice;

        public SalePrice[] SalePrice
        {
            get { return _SalePrice; }
            set { _SalePrice = value; }
        }
    }
}