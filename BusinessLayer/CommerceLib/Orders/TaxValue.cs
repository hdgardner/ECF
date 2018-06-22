using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Commerce.Orders
{
	/// <summary>
	/// Tax type.
	/// </summary>
    public enum TaxType
    {
        /// <summary>
        /// Sales tax
        /// </summary>
        SalesTax = 1,
        /// <summary>
        /// Shipping tax
        /// </summary>
        ShippingTax = 2
    }

    /// <summary>
    /// Contains Tax Value
    /// </summary>
    public partial class TaxValue
    {
        private double _Percentage = 0d;

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>The percentage.</value>
        public double Percentage
        {
            get { return _Percentage; }
            set { _Percentage = value; }
        }
        private string _Name = String.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _DisplayName = String.Empty;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
        private TaxType _TaxType = TaxType.SalesTax;

        /// <summary>
        /// Gets or sets the type of the tax.
        /// </summary>
        /// <value>The type of the tax.</value>
        public TaxType TaxType
        {
            get { return _TaxType; }
            set { _TaxType = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxValue"/> class.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="name">The name.</param>
        /// <param name="displayname">The displayname.</param>
        /// <param name="type">The type.</param>
        public TaxValue(double percentage, string name, string displayname, TaxType type)
        {
            _Percentage = percentage;
            _Name = name;
            _DisplayName = displayname;
            _TaxType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxValue"/> class.
        /// </summary>
        public TaxValue()
        {
        }
    }
}
