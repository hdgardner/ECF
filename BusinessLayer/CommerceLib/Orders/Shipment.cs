using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Shipment contains information about the particular shipment. Line Items will reference the shipment object they belong to.
    /// </summary>
    [Serializable]
    public class Shipment : OrderStorageBase, ISerializable
    {
        private OrderForm _Parent;
        private ShipmentDiscountCollection _Discounts;

        /// <summary>
        /// Gets the discounts.
        /// </summary>
        /// <value>The discounts.</value>
        public ShipmentDiscountCollection Discounts
        {
            get
            {
                return _Discounts;
            }
        }

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

        #region Public Field Properties
        /// <summary>
        /// Gets the shipment id.
        /// </summary>
        /// <value>The shipment id.</value>
        public int ShipmentId
        {
            get
            {
                if (this["ShipmentId"] == null)
                    return 0;

                return GetInt32("ShipmentId");
            }
        }

        /// <summary>
        /// Gets or sets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                if (this["OrderGroupId"] == null)
                    return 0;

                return GetInt32("OrderGroupId");
            }
            set
            {
                this["OrderGroupId"] = value;
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
                return GetInt32("OrderFormId");
            }
            set
            {
                this["OrderFormId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping method id.
        /// </summary>
        /// <value>The shipping method id.</value>
        public Guid ShippingMethodId
        {
            get
            {
                return GetGuid("ShippingMethodId");
            }
            set
            {
                this["ShippingMethodId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the shipping method.
        /// </summary>
        /// <value>The name of the shipping method.</value>
        public string ShippingMethodName
        {
            get
            {
                return GetString("ShippingMethodName");
            }
            set
            {
                this["ShippingMethodName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping address name.
        /// </summary>
        /// <value>The shipping address id.</value>
        public string ShippingAddressId
        {
            get
            {
                return GetString("ShippingAddressId");
            }
            set
            {
                this["ShippingAddressId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipment tracking number.
        /// </summary>
        /// <value>The shipment tracking number.</value>
        public string ShipmentTrackingNumber
        {
            get
            {
                return GetString("ShipmentTrackingNumber");
            }
            set
            {
                this["ShipmentTrackingNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipment total. This price does not include any discounts that might have
        /// been applied to the shipment.
        /// </summary>
        /// <value>The shipment total.</value>
        public decimal ShipmentTotal
        {
            get
            {
                return GetDecimal("ShipmentTotal");
            }
            set
            {
                this["ShipmentTotal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping discount amount.
        /// </summary>
        /// <value>The shipping discount amount.</value>
        public decimal ShippingDiscountAmount
        {
            get
            {
                return GetDecimal("ShippingDiscountAmount");
            }
            set
            {
                this["ShippingDiscountAmount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status
        {
            get
            {
                return GetString("Status");
            }
            set
            {
                this["Status"] = value;
            }
        }

        private StringCollection _LineItemIds = null;
        /// <summary>
        /// Gets the line item indexes. This is a string collection of LineItem indexes in the LineItemCollection.
        /// </summary>
        /// <value>The line item ids.</value>
        public StringCollection LineItemIndexes
        {
            get
            {
                if (_LineItemIds == null)
                {
                    string lineItemsString = GetString("LineItemIds");
                    
                    if (String.IsNullOrEmpty(lineItemsString))
                    {
                        _LineItemIds = new StringCollection();
                        return _LineItemIds;
                    }

                    // Parse the comma separated string
                    _LineItemIds = new StringCollection();
                    foreach(string itemId in lineItemsString.Split(new char[] {','} ))
                    {
                        _LineItemIds.Add(itemId);
                    }
                }

                return _LineItemIds;
            }
        }

        /// <summary>
        /// Gets the shipment line items.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
		public static List<LineItem> GetShipmentLineItems(Shipment shipment)
		{
            /* OLD METHOD USED WHEN LineItemId(s) were stored
			List<LineItem> items = new List<LineItem>();

			if (shipment != null && shipment.Parent != null)
				foreach (string key in shipment.LineItemIds)
				{
					foreach (LineItem item in shipment.Parent.LineItems)
					{
						if (String.Compare(item.LineItemId.ToString(), key, true) == 0)
							items.Add(item);
					}
				}
            return items;
             * */

            List<LineItem> items = new List<LineItem>();

            if (shipment != null && shipment.Parent != null)
            {
                foreach (string key in shipment.LineItemIndexes)
                {
                    LineItem item = shipment.Parent.LineItems[Int32.Parse(key)];
                    if (item != null)
                        items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Returns string of properties specified by <paramref name="type"/> parameter separated by <paramref name="separator"/>.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <param name="type">The type.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        /// <remarks>Type can be one of the following: 1 - LineItemIds, 2 - CatalogEntryIds, 3 - DisplayName</remarks>
		public static string GetShipmentLineItemsString(Shipment shipment, int type, string separator)
		{
			string retVal = String.Empty;

			List<LineItem> items = Shipment.GetShipmentLineItems(shipment);

			int count = items.Count;

			if (items.Count > 0)
			{
				if (type == 1)
				{
                    for (int i = 0; i < count - 1; i++)
                        retVal += String.Concat(items[i].LineItemId.ToString(), separator);
					retVal = String.Concat(retVal, items[count - 1].LineItemId.ToString());
				}
				else if (type == 2)
				{
					for (int i = 0; i < count - 1; i++)
						retVal += String.Concat(items[i].CatalogEntryId, separator);
					retVal = String.Concat(retVal, items[count - 1].CatalogEntryId);
				}
				else if (type == 3)
				{
					for (int i = 0; i < count - 1; i++)
						retVal += String.Concat(items[i].DisplayName, separator);
					retVal = String.Concat(retVal, items[count - 1].DisplayName);
				}
			}
			return retVal;
		}

        /*
        public MetaStringDictionary LineItemIds
        {
            get
            {
                MetaStringDictionary dic = this.GetStringDictionary("LineItemIds");
                if (dic == null)
                    dic = new MetaStringDictionary();

                base["LineItemIds"] = dic;

                return dic;
            }
        }
         * */
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Shipment"/> class.
        /// </summary>
        public Shipment() : base(OrderContext.Current.ShipmentMetaClass)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _Discounts = new ShipmentDiscountCollection(this);
			base["ShipmentId"] = 0;
            this.ShipmentTrackingNumber = String.Empty;
            this.ShippingAddressId = String.Empty;
            this.ShippingMethodId = Guid.Empty;
            this.ShippingMethodName = String.Empty;
            this.ShipmentTotal = 0m;
            this.ShippingDiscountAmount = 0m;
            this.Status = String.Empty;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="Parent">The parent.</param>
        public override void SetParent(object Parent)
        {
            _Parent = (OrderForm)Parent;
            Discounts.SetParent(this);
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (_Parent == null)
                throw new NoNullAllowedException("Parent");

            if (_Parent.ObjectState == MetaObjectState.Added)
            {
                throw new MetaException("Must save parent object");
            }

			if (ObjectState != MetaObjectState.Deleted)
			{
				this.OrderGroupId = _Parent.OrderGroupId;
				this.OrderFormId = _Parent.OrderFormId;
			
				// Convert LineItemIds to the string
				string lineItemIdsString = String.Empty;
                StringCollection lineItemIndexes = LineItemIndexes;
                if (lineItemIndexes != null && lineItemIndexes.Count > 0)
				{
                    foreach (string lineItemId in lineItemIndexes)
					{
						if (String.IsNullOrEmpty(lineItemIdsString))
							lineItemIdsString = lineItemId.Trim();
						else
							lineItemIdsString += "," + lineItemId.Trim();
					}
				}

				this["LineItemIds"] = lineItemIdsString;
			}

            // Save record
            using (TransactionScope scope = new TransactionScope())
            {
                base.AcceptChanges();

                if (this.ObjectState != MetaObjectState.Deleted)
                {
                    // Save discounts
                    Discounts.AcceptChanges();
                }

                scope.Complete();
            }
        }

        /// <summary>
        /// Populates the collections internal.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        internal void PopulateCollectionsInternal(DataTableCollection tables, string filter)
        {
            PopulateCollections(tables, filter);
        }

        #region Populate Collections
        /// <summary>
        /// Populates collections within table. The tables used will be removed from
        /// the table collection.
        /// Override this method to populate your custom collection objects.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        protected override void PopulateCollections(DataTableCollection tables, string filter)
        {
            filter = String.Format("ShipmentId = '{0}'", this.ShipmentId.ToString());
            base.PopulateCollections(tables, filter);

            // Populate object collections               
            DataView view = DataHelper.CreateDataView(tables["ShipmentDiscount"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                ShipmentDiscount discount = new ShipmentDiscount();
                discount.Load(row);
                Discounts.Add(discount);
            }
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Initializes a new instance of the <see cref="Shipment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Shipment(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            _Discounts = (ShipmentDiscountCollection)info.GetValue("Discounts", typeof(ShipmentDiscountCollection));

            //_Discounts.SetParent(this);
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);

            info.AddValue("Discounts", this.Discounts, typeof(ShipmentDiscountCollection));
        }
        #endregion
    }
}
