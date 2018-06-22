using System.Collections;
using Mediachase.Commerce.Storage;
using System;
namespace Mediachase.Commerce.Orders 
{
    /// <summary>
    /// Collection of Order Forms.
    /// </summary>
    [Serializable]
    public class OrderFormDiscountCollection : SimpleObjectCollectionBase<OrderFormDiscount>
    {
        private OrderForm _Parent;

        /// <summary>
        /// Gets the parent OrderForm.
        /// </summary>
        /// <value>The parent.</value>
        public OrderForm Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFormDiscountCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
		public OrderFormDiscountCollection(OrderForm parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(OrderFormDiscount value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent Order Form.
        /// </summary>
        /// <param name="orderForm">The order form.</param>
        internal void SetParent(OrderForm orderForm)
        {
            foreach (OrderFormDiscount discount in this)
            {
                discount.SetParent(orderForm);
            }
        }
	}
}