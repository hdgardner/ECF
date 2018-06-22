using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// A collection of addresses for the particular order.
    /// </summary>
    [Serializable]
    public class OrderAddressCollection : MetaStorageCollectionBase<OrderAddress>
    {
		private int _AutoIncrementInternal;
        private OrderGroup _Parent;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public OrderGroup Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAddressCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
		public OrderAddressCollection(OrderGroup parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(OrderAddress value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="orderGroup">The order group.</param>
        internal void SetParent(OrderGroup orderGroup)
        {
            foreach (OrderAddress address in this)
            {
                address.SetParent(orderGroup);
            }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
		public OrderAddress AddNew()
		{
			OrderAddress orderAddress = new OrderAddress();

			// assign id to the new orderAddress
			orderAddress["OrderGroupAddressId"] = --_AutoIncrementInternal;
			orderAddress.OrderGroupId = Parent != null ? Parent.OrderGroupId : 0;
			this.Add(orderAddress);
			return orderAddress;
		}
    }
}
