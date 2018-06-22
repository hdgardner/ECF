using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus.Configurator;
using System.Data;
using System;
using Mediachase.Data.Provider;
using System.Data.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Orders.Search;
using System.Xml.Serialization;

namespace Mediachase.Commerce.Orders {
    
    /// <summary>
    /// Represents the Shopping Cart in the system. User can have unlimited number of carts which can be used for such features
    /// as Wish List, Wedding Registry and so on. Each of them will need to have a unique card name. The Cart with a DefaultName specified
    /// by the DefaultName property will be considered primary cart.
    /// </summary>
    [Serializable()]
	public class Cart : OrderGroup, ISerializable {

        static string _DefaultName = "Default";
        private CreateOrderNumber _OrderNumberMethod;

        /// <summary>
        /// Delegate for generating custom order tracking number. Please look at OrderNumberMethod property for details on how it works.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public delegate string CreateOrderNumber(Cart cart);

        #region Public Properties
        /// <summary>
        /// Gets the default name for the cart.
        /// </summary>
        /// <value>default name for the cart.</value>
        public static string DefaultName
        {
            get { return _DefaultName; }
        }


        /// <summary>
        /// Gets or sets the order number method that is used to generate the tracking PO sequence. If none set, the framework will use
        /// built in function GenerateOrderNumber:
        /// 
        /// this.OrderNumberMethod = new CreateOrderNumber(GenerateOrderNumber);
        /// 
        /// private string GenerateOrderNumber(Cart cart)
        /// {
        ///     string num = new Random().Next(100, 999).ToString();
        ///     return String.Format("PO{0}{1}", cart.OrderGroupId, num);
        /// }
        /// </summary>
        /// <value>The order number method.</value>
        [XmlIgnore]
        public CreateOrderNumber OrderNumberMethod
        {
            get
            {
                return _OrderNumberMethod;
            }
            set
            {
                _OrderNumberMethod = value;
            }
        }
        #endregion

        #region Internal Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="CustomerId">The customer id.</param>
        internal Cart(string Name, Guid CustomerId)
            : base(CustomerId, OrderContext.Current.ShoppingCartMetaClass) 
        {
            this.Name = Name;
        }

        /// <summary>
        /// Internal constructor required by collection implementation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public Cart(IDataReader reader)
            : base(OrderContext.Current.ShoppingCartMetaClass, reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class.
        /// </summary>
        protected Cart() : base(Guid.Empty, OrderContext.Current.ShoppingCartMetaClass)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Cart(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Saves the cart as purchase order, typically the last step in the checkout.
        /// The original cart will need to be deleted and is not handled by this script.
        /// </summary>
        /// <returns></returns>
        public virtual PurchaseOrder SaveAsPurchaseOrder()
        {
            Guid customerId = this.CustomerId;
            int orderGroupId = this.OrderGroupId;

            PurchaseOrder purchaseOrder = null;

            using (TransactionScope scope = new TransactionScope())
            {
                // Clone the object
                // need to set meta data context before cloning
                MetaDataContext.DefaultCurrent = OrderContext.MetaDataContext;
                Cart cart = (Cart)this.Clone();
                cart.SetParent(cart);

                // Second: create new purchase order
                purchaseOrder = (PurchaseOrder)OrderContext.Current.PurchaseOrderClassInfo.CreateInstance();

                // Third: migrate order group
                purchaseOrder.Initialize((OrderGroup)cart);

                // Set tracking number
                if (this.OrderNumberMethod == null)
                {
                    this.OrderNumberMethod = new CreateOrderNumber(GenerateOrderNumber);
                }

                purchaseOrder.TrackingNumber = OrderNumberMethod(this);
                purchaseOrder.Status = OrderConfiguration.Instance.NewOrderStatus;

                // Save the new purchase order
                purchaseOrder.AcceptChanges();

                orderGroupId = purchaseOrder.OrderGroupId;

                scope.Complete();
            }

            return purchaseOrder;// OrderContext.Current.GetPurchaseOrder(customerId, orderGroupId);
        }

        /// <summary>
        /// Saves the cart as purchase order, typically the last step in the checkout.
        /// </summary>
        /// <returns></returns>
        public virtual PaymentPlan SaveAsPaymentPlan()
        {
            Guid customerId = this.CustomerId;
            int orderGroupId = this.OrderGroupId;

            PaymentPlan plan = null;

            using (TransactionScope scope = new TransactionScope())
            {
                // Clone the object
                // need to set meta data context before cloning
                MetaDataContext.DefaultCurrent = OrderContext.MetaDataContext;
                Cart cart = (Cart)this.Clone();
                cart.SetParent(cart);

                // Second: create new purchase order
                plan = (PaymentPlan)OrderContext.Current.PaymentPlanClassInfo.CreateInstance();

                // Third: migrate order group
                plan.Initialize((OrderGroup)cart);

                // Set tracking number
                if (this.OrderNumberMethod == null)
                {
                    this.OrderNumberMethod = new CreateOrderNumber(GenerateOrderNumber);
                }
                plan.Status = OrderConfiguration.Instance.NewOrderStatus;

                // Save the new purchase order
                plan.AcceptChanges();

                orderGroupId = plan.OrderGroupId;

                scope.Complete();
            }

            return plan; // OrderContext.Current.GetPaymentPlan(customerId, orderGroupId);
        }

        /// <summary>
        /// Generates the order number.
        /// </summary>
        /// <param name="cart">The cart.</param>
        /// <returns></returns>
        private string GenerateOrderNumber(Cart cart)
        {
            string num = new Random().Next(100, 999).ToString();
            return String.Format("PO{0}{1}", cart.OrderGroupId, num);
        }

        /// <summary>
        /// Loads cart by customer.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<Cart> LoadByCustomer(Guid CustomerId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_Customer", OrderContext.Current.ShoppingCartMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CustomerId", CustomerId, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
        }

        /// <summary>
        /// Loads cat by name and customer id.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public static Cart LoadByCustomerAndName(Guid CustomerId, string Name)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_CustomerAndName", OrderContext.Current.ShoppingCartMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CustomerId", CustomerId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Name", Name, DataParameterType.NVarChar));
            cmd.Parameters[2].Size = 64;

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<Cart> carts = LoadSearchResults(searchGuid);

            if (carts.Count > 0)
                return carts[0];

            return null;
        }

        /// <summary>
        /// Loads cart by customer and order group id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="oderGroupId">The oder group id.</param>
        /// <returns></returns>
        public static Cart LoadByCustomerAndOrderGroupId(Guid customerId, int oderGroupId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_CustomerAndOrderGroupId", OrderContext.Current.ShoppingCartMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CustomerId", customerId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("OrderGroupId", oderGroupId, DataParameterType.Int));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<Cart> orders = LoadSearchResults(searchGuid);

            if (orders.Count > 0)
                return orders[0];

            return null;
        }

        /// <summary>
        /// Adds the specified order group to the existing cart.
        /// </summary>
        /// <param name="orderGroup">The order group.</param>
        public void Add(OrderGroup orderGroup)
        {
            Add(orderGroup, false);
        }

        /// <summary>
        /// Adds the specified order group to the existing cart.
        /// </summary>
        /// <param name="orderGroup">The order group.</param>
        /// <param name="lineItemRollup">if set to <c>true</c> [line item rollup].</param>
        public void Add(OrderGroup orderGroup, bool lineItemRollup)
        {
            if (orderGroup == null)
            {
                throw new ArgumentNullException("orderGroup");
            }

            if ((orderGroup.OrderForms != null) && (orderGroup.OrderForms.Count != 0))
            {
                OrderContext orderSystem = OrderContext.Current;
                ClassInfo orderFormInfo = orderSystem.OrderFormClassInfo;

                // need to set meta data context before cloning
                MetaDataContext.DefaultCurrent = OrderContext.MetaDataContext;
                foreach (OrderForm form in orderGroup.OrderForms)
                {
                    OrderForm orderForm = base.OrderForms[form.Name];
                    if (orderForm == null)
                    {
                        orderForm = (OrderForm)orderFormInfo.CreateInstance();
                        orderForm.Name = form.Name;
                        base.OrderForms.Add(orderForm);
                    }

                    int count = form.LineItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        LineItem newLineItem = (LineItem)form.LineItems[i].Clone();
                        newLineItem.SetParent(orderForm);
                        orderForm.LineItems.Add(newLineItem, lineItemRollup);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Searches for the cart.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<Cart> Search(OrderSearch search, out int totalRecords)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            totalRecords = search.Search(searchGuid);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
        }

        #region Private Data Help Methods
        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="SearchGuid">The search GUID.</param>
        /// <returns></returns>
        private static MetaStorageCollectionBase<Cart> LoadSearchResults(Guid SearchGuid)
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}", OrderContext.Current.ShoppingCartMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SearchSetId", SearchGuid, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataResult result = DataService.LoadDataSet(cmd);

            MetaStorageCollectionBase<Cart> orders = new MetaStorageCollectionBase<Cart>();

            PopulateCollection<Cart>(OrderContext.Current.ShoppingCartClassInfo, orders, result.DataSet);
            return orders;
        }
        #endregion

        #region ISerializable Members
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
        }

        #endregion
    }
}