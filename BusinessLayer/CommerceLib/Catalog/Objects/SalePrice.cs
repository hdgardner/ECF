using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Implements operations for the sale price in catalog.
    /// </summary>
    [DataContract]
    public partial class SalePrice
    {
        string _SaleType;

        /// <summary>
        /// Gets or sets the type of the sale.
        /// </summary>
        /// <value>The type of the sale.</value>
        public string SaleType
        {
            get
            {
                return _SaleType;
            }
            set
            {
                _SaleType = value;
            }
        }

        string _SaleCode;

        /// <summary>
        /// Gets or sets the sale code.
        /// </summary>
        /// <value>The sale code.</value>
        public string SaleCode
        {
            get
            {
                return _SaleCode;
            }
            set
            {
                _SaleCode = value;
            }
        }

        DateTime _StartDate;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }

        DateTime _EndDate;

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
            }
        }

        decimal _MinQuantity;

        /// <summary>
        /// Gets or sets the min quantity.
        /// </summary>
        /// <value>The min quantity.</value>
        public decimal MinQuantity
        {
            get
            {
                return _MinQuantity;
            }
            set
            {
                _MinQuantity = value;
            }
        }

        Price _UnitPrice;

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        /// <value>The unit price.</value>
        public Price UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                _UnitPrice = value;
            }
        }
    }
}
