using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Line Item type of discount.
    /// </summary>
    [Serializable]
    public class LineItemDiscount : Discount
    {
        private LineItem _Parent;

        /// <summary>
        /// Gets the LineItem object discount belongs to.
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
        /// Sets the parent line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        public void SetParent(LineItem lineItem)
        {
            this._Parent = lineItem;
        }

        /// <summary>
        /// Gets or sets the line item discount id.
        /// </summary>
        /// <value>The line item discount id.</value>
        public int LineItemDiscountId
        {
            get
            {
                return base.GetInt("LineItemDiscountId");
            }
            set
            {
                base["LineItemDiscountId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the line item id.
        /// </summary>
        /// <value>The line item id.</value>
        public int LineItemId
        {
            get
            {
                return base.GetInt("LineItemId");
            }
            set
            {
                base["LineItemId"] = value;
            }
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void CreateParameters(DataCommand command)
        {
            command.Parameters.Add(new DataParameter("LineItemDiscountId", LineItemDiscountId, DataParameterType.Int));
            command.Parameters.Add(new DataParameter("LineItemId", LineItemId, DataParameterType.Int));
            base.CreateParameters(command);
        }

        /// <summary>
        /// Called when [saved].
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void OnSaved(DataResult result)
        {
            if (this.LineItemDiscountId == 0)
            {
                this.LineItemDiscountId = (int)result.Parameters[0].Value;
            }
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (this.ObjectState != MetaObjectState.Deleted)
            {
                LineItemId = Parent.LineItemId;
                OrderGroupId = Parent.OrderGroupId;
            }

            base.AcceptChanges();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemDiscount"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LineItemDiscount(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemDiscount"/> class.
        /// </summary>
        public LineItemDiscount() : base()
        {
        }

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            LineItemDiscountId = 0;
            base.MarkNew();
        }
    }
}