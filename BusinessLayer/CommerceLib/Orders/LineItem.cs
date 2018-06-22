using System;
using System.Runtime.Serialization;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Line Item in the system. The actual item that is bought.
    /// </summary>
    [Serializable]
    public class LineItem : OrderStorageBase, ISerializable
    {
        private OrderForm _Parent;
        private LineItemDiscountCollection _Discounts;

        /// <summary>
        /// Gets the list of discounts that were applied to that particular line item.
        /// </summary>
        /// <value>The discounts.</value>
        public LineItemDiscountCollection Discounts
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


        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        public LineItem() : base(OrderContext.Current.LineItemMetaClass) 
        {
            Initialize();   
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
			Catalog = String.Empty;
			CatalogNode = String.Empty;
			CatalogEntryId = String.Empty;
            Quantity = 0;
            PlacedPrice = 0;
            ListPrice = 0;
            LineItemDiscountAmount = 0;
            OrderLevelDiscountAmount = 0;
            ShippingAddressId = String.Empty;
			ShippingMethodId = Guid.Empty;
			AllowBackordersAndPreorders = false;
            ExtendedPrice = 0;
            InStockQuantity = 0;
            PreorderQuantity = 0;
            BackorderQuantity = 0;
			InventoryStatus = 0;
			MinQuantity = 1;
			MaxQuantity = 100;
            LineItemOrdering = DateTime.UtcNow;
            this["LineItemId"] = 0;

            _Discounts = new LineItemDiscountCollection(this);
        }

        #region Public Properties
        /// <summary>
        /// Gets the line item id.
        /// </summary>
        /// <value>The line item id.</value>
        public int LineItemId
        {
            get
            {
                return base.GetInt("LineItemId");
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
        /// Gets or sets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                return base.GetInt("OrderGroupId");
            }
            set
            {
                base["OrderGroupId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        /// <value>The catalog.</value>
        public string Catalog
        {
            get
            {
                return base.GetString("Catalog");
            }
            set
            {
                base["Catalog"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the catalog node.
        /// </summary>
        /// <value>The catalog node.</value>
        public string CatalogNode
        {
            get
            {
                return base.GetString("CatalogNode");
            }
            set
            {
                base["CatalogNode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent catalog entry id. Typically for Product/Sku(Variation) types of products, this will be a product code.
        /// </summary>
        /// <value>The parent catalog entry id.</value>
        public string ParentCatalogEntryId
        {
            get
            {
                return base.GetString("ParentCatalogEntryId");
            }
            set
            {
                base["ParentCatalogEntryId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the catalog entry code.
        /// </summary>
        /// <value>The catalog entry id.</value>
        public string CatalogEntryId
        {
            get
            {
                return base.GetString("CatalogEntryId");
            }
            set
            {
                base["CatalogEntryId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public decimal Quantity
        {
            get
            {
                return base.GetDecimal("Quantity");
            }
            set
            {
                base["Quantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the min quantity.
        /// </summary>
        /// <value>The min quantity.</value>
        public decimal MinQuantity
        {
            get
            {
                return base.GetDecimal("MinQuantity");
            }
            set
            {
                base["MinQuantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the max quantity.
        /// </summary>
        /// <value>The max quantity.</value>
        public decimal MaxQuantity
        {
            get
            {
                return base.GetDecimal("MaxQuantity");
            }
            set
            {
                base["MaxQuantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the placed price.
        /// </summary>
        /// <value>The placed price.</value>
        public decimal PlacedPrice
        {
            get
            {
                return base.GetDecimal("PlacedPrice");
            }
            set
            {
                base["PlacedPrice"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the list price. The price that the item is listed in the catalog.
        /// </summary>
        /// <value>The list price.</value>
        public decimal ListPrice
        {
            get
            {
                return base.GetDecimal("ListPrice");
            }
            set
            {
                base["ListPrice"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the line item discount amount.
        /// </summary>
        /// <value>The line item discount amount.</value>
        public decimal LineItemDiscountAmount
        {
            get
            {
                return base.GetDecimal("LineItemDiscountAmount");
            }
            set
            {
                base["LineItemDiscountAmount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the order level discount amount.
        /// </summary>
        /// <value>The order level discount amount.</value>
        public decimal OrderLevelDiscountAmount
        {
            get
            {
                return base.GetDecimal("OrderLevelDiscountAmount");
            }
            set
            {
                base["OrderLevelDiscountAmount"] = value;
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
                return base.GetString("ShippingAddressId");
            }
            set
            {
                base["ShippingAddressId"] = value;
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
                return base.GetString("ShippingMethodName");
            }
            set
            {
                base["ShippingMethodName"] = value;
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
                return base.GetGuid("ShippingMethodId");
            }
            set
            {
                base["ShippingMethodId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the extended price.
        /// </summary>
        /// <value>The extended price.</value>
        public decimal ExtendedPrice
        {
            get
            {
                return base.GetDecimal("ExtendedPrice");
            }
            set
            {
                base["ExtendedPrice"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return base.GetString("Description");
            }
            set
            {
                base["Description"] = value;
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
                return base.GetString("Status");
            }
            set
            {
                base["Status"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the display.
        /// </summary>
        /// <value>The name of the display.</value>
        public string DisplayName
        {
            get
            {
                return base.GetString("DisplayName");
            }
            set
            {
                base["DisplayName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow backorders and preorders].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow backorders and preorders]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowBackordersAndPreorders
        {
            get
            {
                return base.GetBool("AllowBackordersAndPreorders");
            }
            set
            {
                base["AllowBackordersAndPreorders"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the in stock quantity.
        /// </summary>
        /// <value>The in stock quantity.</value>
        public decimal InStockQuantity
        {
            get
            {
                return base.GetDecimal("InStockQuantity");
            }
            set
            {
                base["InStockQuantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the preorder quantity.
        /// </summary>
        /// <value>The preorder quantity.</value>
        public decimal PreorderQuantity
        {
            get
            {
                return base.GetDecimal("PreorderQuantity");
            }
            set
            {
                base["PreorderQuantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the backorder quantity.
        /// </summary>
        /// <value>The backorder quantity.</value>
        public decimal BackorderQuantity
        {
            get
            {
                return base.GetDecimal("BackorderQuantity");
            }
            set
            {
                base["BackorderQuantity"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the inventory status.
        /// </summary>
        /// <value>The inventory status.</value>
        public int InventoryStatus
        {
            get
            {
                return base.GetInt("InventoryStatus");
            }
            set
            {
                base["InventoryStatus"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the line item ordering.
        /// </summary>
        /// <value>The line item ordering.</value>
        public DateTime LineItemOrdering
        {
            get
            {
                return base.GetDateTime("LineItemOrdering");
            }
            set
            {
                base["LineItemOrdering"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the configuration id. The external component configuration id, used by bundle, kits and other
        /// combination products.
        /// </summary>
        /// <value>The configuration id.</value>
        public string ConfigurationId
        {
            get
            {
                return base.GetString("ConfigurationId");
            }
            set
            {
                base["ConfigurationId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the provider id. Used to identify the line item in the extrnal system.
        /// </summary>
        /// <value>The provider id.</value>
        public string ProviderId
        {
            get
            {
                return GetString("ProviderId");
            }
            set
            {
                this["ProviderId"] = value;
            }
        }
        #endregion

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
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            base.MarkNew();
            Discounts.MarkNew();
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if(_Parent == null)
                throw new NoNullAllowedException("Parent");

            if (_Parent.ObjectState == MetaObjectState.Added)
            {
                throw new MetaException("Must save parent object");
            }

            if (ObjectState != MetaObjectState.Deleted)
            {
                this.OrderGroupId = _Parent.OrderGroupId;
                this.OrderFormId = _Parent.OrderFormId;
            }

            using (TransactionScope scope = new TransactionScope())
            {
                base.AcceptChanges();

                if (this.ObjectState != MetaObjectState.Deleted)
                {
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
            filter = String.Format("LineItemId = '{0}'", this.LineItemId.ToString());
            base.PopulateCollections(tables, filter);

            // Populate object collections               
            DataView view = DataHelper.CreateDataView(tables["LineItemDiscount"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                LineItemDiscount discount = new LineItemDiscount();
                discount.Load(row);
                Discounts.Add(discount);
            }
        }
        #endregion

        #region ISerializable Members

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LineItem(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            _Discounts = (LineItemDiscountCollection)info.GetValue("Discounts", typeof(LineItemDiscountCollection));
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

            info.AddValue("Discounts", this.Discounts, typeof(LineItemDiscountCollection));
        }

        #endregion
    }
}