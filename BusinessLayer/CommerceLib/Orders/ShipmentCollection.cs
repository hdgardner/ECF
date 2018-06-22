using System;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Orders 
{
    /// <summary>
    /// Collection of shipments.
    /// </summary>
    [Serializable]
    public class ShipmentCollection : MetaStorageCollectionBase<Shipment>
    {
		private int _AutoIncrementInternal = 0;
        private OrderForm _Parent;

        /// <summary>
        /// Gets the parent Order Form.
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
        /// Initializes a new instance of the <see cref="ShipmentCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ShipmentCollection(OrderForm parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Sets the parent Order Form.
        /// </summary>
        /// <param name="form">The form.</param>
        internal void SetParent(OrderForm form)
        {
            foreach (Shipment shipment in this)
                shipment.SetParent(form);
        }

        /// <summary>
        /// Adds the specified Shipment.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(Shipment value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Adds the new shipment.
        /// </summary>
        /// <returns></returns>
		public Shipment AddNew()
		{
			Shipment shipment = new Shipment();

			// assign id to the new shipment
			shipment["ShipmentId"] = --_AutoIncrementInternal;
			shipment.OrderGroupId = Parent != null ? Parent.OrderGroupId : 0;
			this.Add(shipment);
			return shipment;
		}
	}
}