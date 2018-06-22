using System.Collections;
using Mediachase.Commerce.Storage;
using System;
namespace Mediachase.Commerce.Orders 
{
    /// <summary>
    /// Shipment Discount Collection.
    /// </summary>
    [Serializable]
    public class ShipmentDiscountCollection : SimpleObjectCollectionBase<ShipmentDiscount>
    {
        private Shipment _Parent;

        /// <summary>
        /// Gets the parent Shipment.
        /// </summary>
        /// <value>The parent.</value>
        public Shipment Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentDiscountCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
		public ShipmentDiscountCollection(Shipment parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(ShipmentDiscount value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        internal void SetParent(Shipment shipment)
        {
            foreach (ShipmentDiscount discount in this)
            {
                discount.SetParent(shipment);
            }
        }
	}
}