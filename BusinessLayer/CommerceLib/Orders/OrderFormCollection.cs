using System;
using System.Collections;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Collection of order forms.
    /// </summary>
    [Serializable]
    public class OrderFormCollection : MetaStorageCollectionBase<OrderForm>
    {

        private OrderGroup _Parent;

        /// <summary>
        /// Gets the parent order group.
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
        /// Initializes a new instance of the <see cref="OrderFormCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public OrderFormCollection(OrderGroup parent)
        {
            _Parent = parent;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(OrderForm value)
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
            foreach (OrderForm form in this)
            {
                form.SetParent(orderGroup);
            }
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Orders.OrderForm"/> with the specified name.
        /// </summary>
        /// <value></value>
        public OrderForm this[string name]
        {
            get
			{
                foreach (OrderForm form in this)
                {
					if (String.Compare(form.Name, name, true) == 0)
						return form;
                }

                return null;
            }
        }
    }
}