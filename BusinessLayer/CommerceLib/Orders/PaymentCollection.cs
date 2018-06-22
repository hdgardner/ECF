using System;
//using System.Collections.Generic;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Collection of payments.
    /// </summary>
    [Serializable]
    public class PaymentCollection : MetaStorageCollectionBase<Payment>
    {
		private int _AutoIncrementInternal = 0;
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
        /// Initializes a new instance of the <see cref="PaymentCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public PaymentCollection(OrderForm parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(Payment value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent Order Form.
        /// </summary>
        /// <param name="form">The form.</param>
        internal void SetParent(OrderForm form)
        {
            foreach (Payment payment in this)
                payment.SetParent(form);
        }

        /// <summary>
        /// Adds the new Payment.
        /// </summary>
        /// <param name="paymentType">Type of the payment.</param>
        /// <returns></returns>
		public Payment AddNew(Type paymentType)
		{
			Payment payment = (Payment)Activator.CreateInstance(paymentType);
			if (payment != null)
			{
				// assign id to the new payment
				payment["PaymentId"] = --_AutoIncrementInternal;
				//payment.OrderGroupId = Parent != null ? Parent.OrderGroupId : 0;
				this.Add(payment);
			}
			return payment;
		}
    }
}
