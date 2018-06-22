using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the catalog object price.
    /// </summary>
    [DataContract]
    [Serializable]
    public partial class Price
    {
        /// <summary>
        /// Represents the price amount.
        /// </summary>
        private decimal _Amount;

        public decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        /// <summary>
        /// Represents the price currency code.
        /// </summary>
        private string _CurrencyCode;

        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set { _CurrencyCode = value; }
        }

        /// <summary>
        /// Represents the formatted price.
        /// </summary>
        private string _FormattedPrice;

        public string FormattedPrice
        {
            get { return _FormattedPrice; }
            set { _FormattedPrice = value; }
        }
    }
}
