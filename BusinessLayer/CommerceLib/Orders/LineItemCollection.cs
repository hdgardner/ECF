using Mediachase.Commerce.Storage;
using System.Collections;
using System;

namespace Mediachase.Commerce.Orders {

    /// <summary>
    /// A collection of line items in the system.
    /// </summary>
    [Serializable]
    public class LineItemCollection : MetaStorageCollectionBase<LineItem>
    {
		private int _AutoIncrementInternal;
        private OrderForm _Parent;

        /// <summary>
        /// Gets the parent.
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
        /// Sets the parent Order Form.
        /// </summary>
        /// <param name="form">The form.</param>
        internal void SetParent(OrderForm form)
        {
            foreach (LineItem lineItem in this)
                lineItem.SetParent(form);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public LineItemCollection(OrderForm parent)
        {
            _Parent = parent;
		}

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(LineItem value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="lineItemRollup">if set to <c>true</c> [line item rollup].</param>
        /// <returns></returns>
        public virtual int Add(LineItem value, bool lineItemRollup)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            LineItem existingItem = null;

            foreach (LineItem item in this)
            {
                if (object.ReferenceEquals(value, item))
                {
                    return IndexOf(value);
                }

                if (item.CatalogEntryId == value.CatalogEntryId)
                {
                    existingItem = item;
                    break;
                }
            }

            if (lineItemRollup && (existingItem != null))
            {
                existingItem.Quantity += value.Quantity;
                return IndexOf(existingItem);
            }
            else
            {
                return Add(value);
            }
        }

        /// <summary>
        /// Adds a new LineItem to the collection and assigns it unique LineItemId
        /// </summary>
        /// <returns></returns>
		public LineItem AddNew()
		{
			LineItem lineItem = new LineItem();

			// assign id to the new lineItem
			lineItem["LineItemId"] = --_AutoIncrementInternal;
			this.Add(lineItem);
			return lineItem;
		}

		/// <summary>
		/// Finds LineItem with specified LineItemId in the collection.
		/// </summary>
		/// <param name="lineItemId"></param>
		/// <returns></returns>
		public LineItem FindItem(int lineItemId)
		{
			LineItem item = null;
			foreach(LineItem li in this)
				if (li.LineItemId == lineItemId)
				{
					item = li;
					break;
				}
			return item;
		}

		/// <summary>
		/// Returns string of properties specified by <paramref name="type"/> parameter separated by <paramref name="separator"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		/// <remarks>Type can be one of the following: 1 - LineItemIds, 2 - CatalogEntryIds, 3 - DisplayName</remarks>
		public string GetFormattedString(int type, string separator)
		{
			string retVal = String.Empty;

			int count = this.Count;

			if (type == 1)
			{
				for (int i = 0; i < count; i++)
					retVal += i < count - 1 ? String.Concat(this[i].LineItemId.ToString(), separator) : this[i].LineItemId.ToString();
			}
			else if (type == 2)
			{
				for (int i = 0; i < count; i++)
					retVal += i < count - 1 ? String.Concat(this[i].CatalogEntryId, separator) : this[i].CatalogEntryId;
			}
			else if (type == 3)
			{
				for (int i = 0; i < count; i++)
					retVal += i < count - 1 ? String.Concat(this[i].DisplayName, separator) : this[i].DisplayName;
			}

			return retVal;
		}

		/// <summary>
		/// Returns comma-separated list of LineItemIds.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string retVal = String.Empty;

			int count = this.Count;

			for (int i = 0; i < count; i++)
				retVal += i < count - 1 ? String.Concat(this[i].LineItemId.ToString(), ",") : this[i].LineItemId.ToString();

			return retVal;
		}
	}
}