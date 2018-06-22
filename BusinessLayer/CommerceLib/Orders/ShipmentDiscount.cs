using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Shipment Discount, contains information about specific shipment discount.
    /// </summary>
    [Serializable]
    public class ShipmentDiscount : Discount
    {
        private Shipment _Parent;

        /// <summary>
        /// Gets the Shipment discount belongs to.
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
        /// Sets the parent shipment.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        public void SetParent(Shipment shipment)
        {
            this._Parent = shipment;
        }

        /// <summary>
        /// Gets or sets the shipment discount id.
        /// </summary>
        /// <value>The shipment discount id.</value>
        public int ShipmentDiscountId
        {
            get
            {
                return base.GetInt("ShipmentDiscountId");
            }
            set
            {
                base["ShipmentDiscountId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipment id.
        /// </summary>
        /// <value>The shipment id.</value>
        public int ShipmentId
        {
            get
            {
                return base.GetInt("ShipmentId");
            }
            set
            {
                base["ShipmentId"] = value;
            }
        }


        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void CreateParameters(DataCommand command)
        {
            command.Parameters.Add(new DataParameter("ShipmentDiscountId", ShipmentDiscountId, DataParameterType.Int));
            command.Parameters.Add(new DataParameter("ShipmentId", ShipmentId, DataParameterType.Int));
            base.CreateParameters(command);
        }

        /// <summary>
        /// Called when [saved].
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void OnSaved(DataResult result)
        {
            if (this.ShipmentDiscountId == 0)
            {
                this.ShipmentDiscountId = (int)result.Parameters[0].Value;
            }
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (this.ObjectState != MetaObjectState.Deleted)
            {
                ShipmentId = Parent.ShipmentId;
                OrderGroupId = Parent.OrderGroupId;
            }

            base.AcceptChanges();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentDiscount"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ShipmentDiscount(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentDiscount"/> class.
        /// </summary>
        public ShipmentDiscount() : base()
        {
        }

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            ShipmentDiscountId = 0;
            base.MarkNew();
        }
    }
}
