using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Order form discount class.
    /// </summary>
    [Serializable]
    public class OrderFormDiscount : Discount
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
        /// Sets the parent OrderForm.
        /// </summary>
        /// <param name="orderForm">The order form.</param>
        public void SetParent(OrderForm orderForm)
        {
            this._Parent = orderForm;
        }

        /// <summary>
        /// Gets or sets the order form discount id.
        /// </summary>
        /// <value>The order form discount id.</value>
        public int OrderFormDiscountId
        {
            get
            {
                return base.GetInt("OrderFormDiscountId");
            }
            set
            {
                base["OrderFormDiscountId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the order form id.
        /// </summary>
        /// <value>The order form id.</value>
        public int OrderFormId
        {
            get
            {
                return base.GetInt("OrderFormId");
            }
            set
            {
                base["OrderFormId"] = value;
            }
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void CreateParameters(DataCommand command)
        {
            command.Parameters.Add(new DataParameter("OrderFormDiscountId", OrderFormDiscountId, DataParameterType.Int));
            command.Parameters.Add(new DataParameter("OrderFormId", OrderFormId, DataParameterType.Int));
            base.CreateParameters(command);
        }

        /// <summary>
        /// Called when [saved].
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void OnSaved(DataResult result)
        {
            if (this.OrderFormDiscountId == 0)
            {
                this.OrderFormDiscountId = (int)result.Parameters[0].Value;
            }
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (this.ObjectState != MetaObjectState.Deleted)
            {
                OrderFormId = Parent.OrderFormId;
                OrderGroupId = Parent.OrderGroupId;
            }

            base.AcceptChanges();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFormDiscount"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrderFormDiscount(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFormDiscount"/> class.
        /// </summary>
        public OrderFormDiscount()
        {
        }

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            OrderFormDiscountId = 0;
            base.MarkNew();
        }
    }
}
