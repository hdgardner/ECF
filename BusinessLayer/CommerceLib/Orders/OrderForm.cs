using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Contains the order information.
    /// </summary>
	[Serializable]
    public class OrderForm : OrderStorageBase, ISerializable
    {
        #region Private Fields
        private OrderGroup _Parent;
        private ShipmentCollection _Shipments;
        private PaymentCollection _Payments;
        private LineItemCollection _LineItems;
        private OrderFormDiscountCollection _Discounts;
        #endregion

        /// <summary>
        /// Gets the parent Order Group.
        /// </summary>
        /// <value>The parent.</value>
        public OrderGroup Parent
        {
            get
            {
                return _Parent;
            }
        }

        #region Public Collections
        /// <summary>
        /// Gets the shipments.
        /// </summary>
        /// <value>The shipments.</value>
        public ShipmentCollection Shipments
        {
            get
            {
                return _Shipments;
            }
        }

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <value>The payments.</value>
        public PaymentCollection Payments
        {
            get
            {
                return _Payments;
            }
        }

        /// <summary>
        /// Gets the line items.
        /// </summary>
        /// <value>The line items.</value>
        public LineItemCollection LineItems
        {
            get
            {
                return _LineItems;
            }
        }

        /// <summary>
        /// Gets the discounts.
        /// </summary>
        /// <value>The discounts.</value>
        public OrderFormDiscountCollection Discounts
        {
            get
            {
                return _Discounts;
            }
        }
        #endregion

        #region Public Field Properties
        /// <summary>
        /// Gets the order form id.
        /// </summary>
        /// <value>The order form id.</value>
        public int OrderFormId
        {
            get
            {
                if (this["OrderFormId"] == null)
                    return 0;

                return GetInt32("OrderFormId");
            }
        }

        /// <summary>
        /// Gets the order group id.
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
            /*
            protected set
            {
                this["OrderGroupId"] = value;
            }
             * */
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return GetString("Name");
            }
            set
            {
                this["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the billing address id. The id is the name of the address in OrderAddress collection.
        /// </summary>
        /// <value>The billing address id.</value>
        public string BillingAddressId
        {
            get
            {
                return GetString("BillingAddressId");
            }
            set
            {
                this["BillingAddressId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping total, does not includes discounts that have been applied to each shipment.
        /// </summary>
        /// <value>The shipping total.</value>
        public decimal ShippingTotal
        {
            get
            {
                return GetDecimal("ShippingTotal");
            }
            set
            {
                this["ShippingTotal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the handling total.
        /// </summary>
        /// <value>The handling total.</value>
        public decimal HandlingTotal
        {
            get
            {
                return GetDecimal("HandlingTotal");
            }
            set
            {
                this["HandlingTotal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        /// <value>The tax total.</value>
        public decimal TaxTotal
        {
            get
            {
                return GetDecimal("TaxTotal");
            }
            set
            {
                this["TaxTotal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the discount amount. This amount includes shipping discounts as well.
        /// </summary>
        /// <value>The discount amount.</value>
        public decimal DiscountAmount
        {
            get
            {
                return GetDecimal("DiscountAmount");
            }
            set
            {
                this["DiscountAmount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>The sub total.</value>
        public decimal SubTotal
        {
            get
            {
                return GetDecimal("SubTotal");
            }
            set
            {
                this["SubTotal"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>The total.</value>
        public decimal Total
        {
            get
            {
                return GetDecimal("Total");
            }
            set
            {
                this["Total"] = value;
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

        /// <summary>
        /// Gets or sets the provider id.
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

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderForm"/> class.
        /// </summary>
        public OrderForm()
            : base(OrderContext.Current.OrderFormMetaClass)
        {
            Initialize();
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderForm"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public OrderForm(IDataReader reader)
            : base(OrderContext.Current.OrderFormMetaClass, reader)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _Shipments = new ShipmentCollection(this);
            _Payments = new PaymentCollection(this);
            _LineItems = new LineItemCollection(this);
            _Discounts = new OrderFormDiscountCollection(this);
            HandlingTotal = 0;
            ShippingTotal = 0;
            SubTotal = 0;
            TaxTotal = 0;
            Total = 0;
            DiscountAmount = 0;
            this["OrderGroupId"] = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the parent order group.
        /// </summary>
        /// <param name="parentGroup">The parent group.</param>
        public override void SetParent(object parentGroup)
        {
            _Parent = (OrderGroup)parentGroup;
            this.Shipments.SetParent(this);
            this.Payments.SetParent(this);
            this.LineItems.SetParent(this);
            this.Discounts.SetParent(this);
        }

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            base.MarkNew();
            Shipments.MarkNew();
            Payments.MarkNew();
            LineItems.MarkNew();
            Discounts.MarkNew();
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            // Save shipments
            foreach (Shipment shipment in Shipments)
            {
                shipment.Delete();
            }

            // Save payments
            foreach (Payment payment in Payments)
            {
                payment.Delete();
            }

            // Save payments
            foreach (LineItem lineitem in LineItems)
            {
                lineitem.Delete();
            }

            base.Delete();
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (_Parent == null)
            {
                throw new NoNullAllowedException("Parent");
            }

            if (_Parent.ObjectState == MetaObjectState.Added)
            {
                throw new MetaException("Must save parent object");
            }

            if(this.ObjectState != MetaObjectState.Deleted)
                this["OrderGroupId"] = _Parent.OrderGroupId;

            using (TransactionScope scope = new TransactionScope())
            {
                base.AcceptChanges();

                if (this.ObjectState != MetaObjectState.Deleted)
                {
                    // Save shipments
                    Shipments.AcceptChanges();

                    // Save payments
                    Payments.AcceptChanges();

                    // Save payments
                    LineItems.AcceptChanges();

                    // Save discounts
                    Discounts.AcceptChanges();
                }

                scope.Complete();
            }
        }
        #endregion

        #region Populate Collections
        /// <summary>
        /// Populates the collections internal.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        internal void PopulateCollectionsInternal(DataTableCollection tables, string filter)
        {
            PopulateCollections(tables, filter);
        }

        /// <summary>
        /// Populates collections within table. The tables used will be removed from
        /// the table collection.
        /// Override this method to populate your custom collection objects.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        protected override void PopulateCollections(DataTableCollection tables, string filter)
        {
            filter = String.Format("OrderFormId = '{0}'", this.OrderFormId.ToString());

            base.PopulateCollections(tables, filter);

            // Populate object collections               
            DataView view = DataHelper.CreateDataView(tables["Shipment"], filter);

            // Read until we are done, since this is a collection
            foreach(DataRowView row in view)
            {
                Shipment orderShipment = (Shipment)OrderContext.Current.ShipmentClassInfo.CreateInstance();
                orderShipment.Load(row);
                orderShipment.PopulateCollectionsInternal(tables, filter);
                Shipments.Add(orderShipment);
            }

            view = DataHelper.CreateDataView(tables["LineItem"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                LineItem lineItem = (LineItem)OrderContext.Current.LineItemClassInfo.CreateInstance();
                lineItem.Load(row);
                lineItem.PopulateCollectionsInternal(tables, filter);
                LineItems.Add(lineItem);
            }

            // Populate object collections               
            // Populates payments collection
            LoadPayments(tables, filter);

            // Load discounts
            view = DataHelper.CreateDataView(tables["OrderFormDiscount"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                OrderFormDiscount discount = new OrderFormDiscount();
                discount.Load(row);
                Discounts.Add(discount);
            }
        }

        /// <summary>
        /// Loads the payments from the returned dataset and adds them to the payments collection.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="filter">The filter.</param>
        protected void LoadPayments(DataTableCollection tables, string filter)
        {
            LoadPayment(DataHelper.CreateDataView(tables["OrderFormPayment_CreditCard"], filter));
            LoadPayment(DataHelper.CreateDataView(tables["OrderFormPayment_CashCard"], filter));
            LoadPayment(DataHelper.CreateDataView(tables["OrderFormPayment_GiftCard"], filter));
            LoadPayment(DataHelper.CreateDataView(tables["OrderFormPayment_Invoice"], filter));
            LoadPayment(DataHelper.CreateDataView(tables["OrderFormPayment_Other"], filter));
        }

        /// <summary>
        /// Loads the payment from the reader object and adds it to the payments collection.
        /// </summary>
        /// <param name="view">The view.</param>
        protected void LoadPayment(DataView view)
        {
            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                string type = row["ImplementationClass"].ToString();
                ClassInfo classInfo = new ClassInfo(type);
                Payment orderPayment = (Payment)classInfo.CreateInstance();
                orderPayment.Load(row);
                Payments.Add(orderPayment);
            }
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderForm"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrderForm(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            _Discounts = (OrderFormDiscountCollection)info.GetValue("Discounts", typeof(OrderFormDiscountCollection));
            _LineItems = (LineItemCollection)info.GetValue("LineItems", typeof(LineItemCollection));
            _Shipments = (ShipmentCollection)info.GetValue("Shipments", typeof(ShipmentCollection));
            _Payments = (PaymentCollection)info.GetValue("Payments", typeof(PaymentCollection));

            /*
            _Discounts.SetParent(this);
            _LineItems.SetParent(this);
            _Shipments.SetParent(this);
            _Payments.SetParent(this);
             * */
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

            info.AddValue("LineItems", this.LineItems, typeof(LineItemCollection));
            info.AddValue("Discounts", this.Discounts, typeof(OrderFormDiscountCollection));
            info.AddValue("Shipments", this.Shipments, typeof(ShipmentCollection));
            info.AddValue("Payments", this.Payments, typeof(PaymentCollection));
        }
        #endregion
    }
}