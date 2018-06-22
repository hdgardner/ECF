using System.Collections;
using Mediachase.Commerce.Storage;
using System;
namespace Mediachase.Commerce.Orders 
{
    /// <summary>
    /// Collection of line item discounts.
    /// </summary>
    [Serializable]
    public class LineItemDiscountCollection : SimpleObjectCollectionBase<LineItemDiscount>
    {
        private LineItem _Parent;

        /// <summary>
        /// Gets the parent line item collection belongs to.
        /// </summary>
        /// <value>The parent.</value>
        public LineItem Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemDiscountCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LineItemDiscountCollection(LineItem parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(LineItemDiscount value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent line item.
        /// </summary>
        /// <param name="lineitem">The lineitem.</param>
        internal void SetParent(LineItem lineitem)
        {
            foreach (LineItemDiscount discount in this)
            {
                discount.SetParent(lineitem);
            }
        }
	}
}