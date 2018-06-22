using System;
using Mediachase.MetaDataPlus.Configurator;
using System.Reflection;
using Mediachase.Commerce.Storage;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using System.Workflow.Runtime;
using System.Threading;
using System.Workflow.Runtime.Hosting;
using Mediachase.Commerce.Engine;
using Mediachase.Commerce.Orders.Search;
using Mediachase.MetaDataPlus;
using System.Collections.Generic;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Orders 
{
    /// <summary>
    /// The Order Context class is used to access all the functions of the Order System.
    /// </summary>
	public class OrderContext
    {
        #region Private Fields
        private MetaClass _PurchaseOrderMetaClass;
        private MetaClass _ShoppingCartMetaClass;
        private MetaClass _OrderFormMetaClass;
        private MetaClass _LineItemMetaClass;
        private MetaClass _PaymentPlanMetaClass;
        private MetaClass _ShipmentMetaClass;
        private MetaClass _OrderAddressMetaClass;
        private ClassInfo _ShoppingCartClassInfo;
        private ClassInfo _PurchaseOrderClassInfo;
        private ClassInfo _OrderFormClassInfo;
        private ClassInfo _PaymentPlanClassInfo;
        private ClassInfo _OrderAddressClassInfo;
        private ClassInfo _ShipmentClassInfo;
        private ClassInfo _LineItemClassInfo;
        #endregion

        private static volatile OrderContext _Instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>The current.</value>
        public static OrderContext Current
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new OrderContext();
                        }
                    }
                }

                return _Instance;
            }
        }

        private static MetaDataContext _mdContext;
        /// <summary>
        /// Gets or sets the meta data context.
        /// </summary>
        /// <value>The meta data context.</value>
        public static MetaDataContext MetaDataContext
        {
            get
            {
                if (_mdContext == null)
                    _mdContext = new MetaDataContext(OrderConfiguration.Instance.Connections.TransactionAppDatabase);

                return _mdContext;
            }
            set
            {
                _mdContext = value;
            }
        }

        #region Workflow Management
		private static WorkflowRuntime _WorkflowRuntime;

        /// <summary>
        /// Occurs when [workflow terminated].
        /// </summary>
        public event EventHandler<WorkflowTerminatedEventArgs> WorkflowTerminated;
        /// <summary>
        /// Occurs when [workflow completed].
        /// </summary>
        public event EventHandler<WorkflowCompletedEventArgs> WorkflowCompleted;
        /// <summary>
        /// Occurs when [workflow idled].
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowIdled;
        /// <summary>
        /// Occurs when [workflow aborted].
        /// </summary>
        public event EventHandler<WorkflowEventArgs> WorkflowAborted;


        /// <summary>
        /// Gets the workflow runtime.
        /// </summary>
        /// <value>The workflow runtime.</value>
        public WorkflowRuntime WorkflowRuntime
        {
            get
            {
                //WorkflowRuntime workflowRuntime = null;

                //object runtime = Thread.GetData(Thread.GetNamedDataSlot("ECF.WorkflowRuntime"));
                //if (runtime != null)
                 //   _WorkflowRuntime = (WorkflowRuntime)runtime;

                if (_WorkflowRuntime == null)
                {
                    _WorkflowRuntime = new WorkflowRuntime();
                    //_WorkflowRuntime.AddService(new SynchronizationContextSchedulerService(true));
                    _WorkflowRuntime.AddService(new ManualWorkflowSchedulerService(true));
                    //Thread.SetData(Thread.GetNamedDataSlot("ECF.WorkflowRuntime"), _WorkflowRuntime);

                    //_WorkflowRuntime.AddService(new ManualWorkflowSchedulerService(true));
                    _WorkflowRuntime.StartRuntime();

                    // Initialize default handlers so we can control how workflow is executed, by default it is executed in sync
                    OrderContext.Current.WorkflowRuntime.WorkflowCompleted += new EventHandler<WorkflowCompletedEventArgs>(WorkflowRuntime_WorkflowCompleted);
                        //delegate(object sender, WorkflowCompletedEventArgs e) { _WorkflowWaitHandle.Set(); };
                    OrderContext.Current.WorkflowRuntime.WorkflowTerminated += new EventHandler<WorkflowTerminatedEventArgs>(WorkflowRuntime_WorkflowTerminated);
                    OrderContext.Current.WorkflowRuntime.WorkflowIdled += new EventHandler<WorkflowEventArgs>(WorkflowRuntime_WorkflowIdled);
                    OrderContext.Current.WorkflowRuntime.WorkflowAborted += new EventHandler<WorkflowEventArgs>(WorkflowRuntime_WorkflowAborted);
                }

                //_WorkflowRuntime = wok

                return _WorkflowRuntime;
            }
        }

        /// <summary>
        /// Handles the WorkflowAborted event of the WorkflowRuntime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
        void WorkflowRuntime_WorkflowAborted(object sender, WorkflowEventArgs e)
        {
            _WorkflowResults = WorkflowResults.CreateAbortedWorkflowResults(e);

            // Either pass the event to the custom handler or generate the exception
            if (WorkflowAborted != null)
                WorkflowAborted(sender, e);               
        }

        /// <summary>
        /// Handles the WorkflowIdled event of the WorkflowRuntime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
        void WorkflowRuntime_WorkflowIdled(object sender, WorkflowEventArgs e)
        {
            _WorkflowResults = WorkflowResults.CreateRunningWorkflowResults(e);

            // Either pass the event to the custom handler or generate the exception
            if (WorkflowIdled != null)
                WorkflowIdled(sender, e);               
        }

        WorkflowResults _WorkflowResults;
        /// <summary>
        /// Gets the workflow results.
        /// </summary>
        /// <value>The workflow results.</value>
        internal WorkflowResults WorkflowResults
        {
            get
            {
                return _WorkflowResults;
            }
        }

        /// <summary>
        /// Handles the WorkflowCompleted event of the WorkflowRuntime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowCompletedEventArgs"/> instance containing the event data.</param>
        void WorkflowRuntime_WorkflowCompleted(object sender, WorkflowCompletedEventArgs e)
        {
            //_WorkflowWaitHandle.Set();
            _WorkflowResults = WorkflowResults.CreateCompletedWorkflowResults(e);

            // Either pass the event to the custom handler or generate the exception
            if (WorkflowCompleted != null)
                WorkflowCompleted(sender, e);               
        }

        /// <summary>
        /// Handles the WorkflowTerminated event of the WorkflowRuntime control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Workflow.Runtime.WorkflowTerminatedEventArgs"/> instance containing the event data.</param>
        void WorkflowRuntime_WorkflowTerminated(object sender, WorkflowTerminatedEventArgs e)
        {
            //_WorkflowWaitHandle.Set();
            _WorkflowResults = WorkflowResults.CreateTerminatedWorkflowResults(e);

            // Either pass the event to the custom handler or generate the exception
            if (WorkflowTerminated != null)
                WorkflowTerminated(sender, e);               
        }

        /*
        public void WorkflowWait()
        {
            _WorkflowWaitHandle.WaitOne();
        }
         * */
        #endregion

        #region Tax Methods
        /// <summary>
        /// Gets the taxes.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="taxCategory">The tax category.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="stateProvinceCode">The state province code.</param>
        /// <param name="zipPostalCode">The zip postal code.</param>
        /// <param name="district">The district.</param>
        /// <param name="county">The county.</param>
        /// <param name="city">The city.</param>
        /// <returns></returns>
        public TaxValue[] GetTaxes(
            Guid siteId,
            string taxCategory,
            string languageCode,
            string countryCode,
            string stateProvinceCode,
            string zipPostalCode,
            string district,
            string county,
            string city)
        {
            List<TaxValue> taxes = new List<TaxValue>();

            DataTable taxTable = TaxManager.GetTaxes(siteId, taxCategory, languageCode, countryCode, stateProvinceCode, zipPostalCode, district, county, city);
            if (taxTable.Rows.Count > 0)
            {
                foreach (DataRow row in taxTable.Rows)
                {
                    TaxValue val = new TaxValue((double)row["Percentage"], (string)row["Name"], (string)row["DisplayName"], (TaxType)(int)row["TaxType"]);
                    taxes.Add(val);
                }
            }
            return taxes.ToArray();
        }

        #endregion

        #region Cart Methods
        /// <summary>
        /// Creates the cart.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        private Cart CreateCart(string name, Guid customerId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name parameter can't be null or empty", "name");
            }

            if (customerId == Guid.Empty)
            {
                throw new ArgumentException("CustomerId parameter can't be null or empty", "customerId");
            }

            Cart cart = (Cart)OrderContext.Current.ShoppingCartClassInfo.CreateInstance();
            cart.Name = name;
            cart.CustomerId = customerId;
            return cart;
        }

        /// <summary>
        /// Returns the cart by name and customer id. If non found creates a new cart.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public Cart GetCart(string name, Guid customerId)
        {
            // Check if one already exists
            Cart cart = Cart.LoadByCustomerAndName(customerId, name);
            
            if(cart == null)
            {
                // Create new one
                cart = CreateCart(name, customerId);
            }

            return cart;
        }

        /// <summary>
        /// Returns the cart by customer id and order group id.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="orderGroupId">The order group id.</param>
        /// <returns></returns>
        public Cart GetCart(Guid customerId, int orderGroupId)
        {
            Cart cart = Cart.LoadByCustomerAndOrderGroupId(customerId, orderGroupId);
            return cart;
        }

        /// <summary>
        /// Finds the carts.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Cart[] FindCarts(OrderSearchParameters parameters, OrderSearchOptions options)
        {
            int totalRecords = 0;
            Cart[] carts = FindCarts(parameters, options, out totalRecords);
            return carts;
        }

        /// <summary>
        /// Finds the carts.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public Cart[] FindCarts(OrderSearchParameters parameters, OrderSearchOptions options, out int totalRecords)
        {
            OrderSearch search = new OrderSearch(this);
            search.SearchOptions = options;
            search.SearchParameters = parameters;
            MetaStorageCollectionBase<Cart> orders = Cart.Search(search, out totalRecords);
            return orders.ToArray();
        }
        #endregion

        #region Purchase Order Methods
        /// <summary>
        /// Gets the purchase orders.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public PurchaseOrder[] GetPurchaseOrders(Guid customerId)
        {
            MetaStorageCollectionBase<PurchaseOrder> orders = PurchaseOrder.LoadByCustomer(customerId);
            return orders.ToArray();
        }

        /// <summary>
        /// Gets the purchase order.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="orderGroupId">The order group id.</param>
        /// <returns></returns>
        public PurchaseOrder GetPurchaseOrder(Guid customerId, int orderGroupId)
        {
            PurchaseOrder order = PurchaseOrder.LoadByCustomerAndOrderGroupId(customerId, orderGroupId);
            return order;
        }

        /// <summary>
        /// Finds the purchase orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public PurchaseOrder[] FindPurchaseOrders(OrderSearchParameters parameters, OrderSearchOptions options)
        {
            int totalRecords = 0;
            PurchaseOrder[] pos = FindPurchaseOrders(parameters, options, out totalRecords);
            return pos;
        }

        /// <summary>
        /// Finds the purchase orders.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public PurchaseOrder[] FindPurchaseOrders(OrderSearchParameters parameters, OrderSearchOptions options, out int totalRecords)
        {
            OrderSearch search = new OrderSearch(this);
            search.SearchOptions = options;
            search.SearchParameters = parameters;

            MetaStorageCollectionBase<PurchaseOrder> orders = PurchaseOrder.Search(search, out totalRecords);
            return orders.ToArray();
        }
        #endregion

        #region Payment Plan methods
        /// <summary>
        /// Gets the payment plans.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public PaymentPlan[] GetPaymentPlans(Guid customerId)
        {
            MetaStorageCollectionBase<PaymentPlan> orders = PaymentPlan.LoadByCustomer(customerId);
            return orders.ToArray();
        }

        /// <summary>
        /// Gets the payment plan.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="orderGroupId">The order group id.</param>
        /// <returns></returns>
        public PaymentPlan GetPaymentPlan(Guid customerId, int orderGroupId)
        {
            PaymentPlan p = PaymentPlan.LoadByCustomerAndOrderGroupId(customerId, orderGroupId);
            return p;
        }

        /// <summary>
        /// Finds the payment plans.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public PaymentPlan[] FindPaymentPlans(OrderSearchParameters parameters, OrderSearchOptions options)
        {
            int totalRecords = 0;
            PaymentPlan[] plans = FindPaymentPlans(parameters, options, out totalRecords);
            return plans;
        }

        /// <summary>
        /// Finds the payment plans.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public PaymentPlan[] FindPaymentPlans(OrderSearchParameters parameters, OrderSearchOptions options, out int totalRecords)
        {
            OrderSearch search = new OrderSearch(this);
            search.SearchOptions = options;
            search.SearchParameters = parameters;

            MetaStorageCollectionBase<PaymentPlan> orders = PaymentPlan.Search(search, out totalRecords);
            return orders.ToArray();
        }
        #endregion

        #region Serialization Helpers
        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="orderGroup">The order group.</param>
        public static void Serialize(XmlTextWriter writer, OrderGroup orderGroup)
        {
            XmlSerializer serializer = new XmlSerializer(orderGroup.GetType());
            serializer.Serialize(writer, orderGroup);
            serializer = null;
        }

        /// <summary>
        /// Deserializes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object Deserialize(XmlNode node, Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(new XmlNodeReader(node));
        }

        #endregion

        #region Public Properties

        #region Meta Classes
        /// <summary>
        /// Gets the purchase order meta class.
        /// </summary>
        /// <value>The purchase order meta class.</value>
        public MetaClass PurchaseOrderMetaClass
        {
            get
            {
                if (_PurchaseOrderMetaClass == null)
					_PurchaseOrderMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.PurchaseOrderClass.Name);

                return _PurchaseOrderMetaClass;
            }
        }
        /// <summary>
        /// Gets the shopping cart meta class.
        /// </summary>
        /// <value>The shopping cart meta class.</value>
        public MetaClass ShoppingCartMetaClass
        {
            get
            {
                if (_ShoppingCartMetaClass == null)
					_ShoppingCartMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.ShoppingCartClass.Name);

                return _ShoppingCartMetaClass;
            }
        }

        /// <summary>
        /// Gets the order form meta class.
        /// </summary>
        /// <value>The order form meta class.</value>
        public MetaClass OrderFormMetaClass
        {
            get
            {
                if (_OrderFormMetaClass == null)
                {
                    _OrderFormMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.OrderFormClass.Name);
                }

                return _OrderFormMetaClass;
            }
        }

        /// <summary>
        /// Gets the order item meta class.
        /// </summary>
        /// <value>The order item meta class.</value>
        public MetaClass OrderItemMetaClass
        {
            get
            {
                if (_LineItemMetaClass == null)
                {
                    _LineItemMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.LineItemClass.Name);
                }

                return _LineItemMetaClass;
            }
        }

        /// <summary>
        /// Gets the payment plan meta class.
        /// </summary>
        /// <value>The payment plan meta class.</value>
        public MetaClass PaymentPlanMetaClass
        {
            get
            {
                if (_PaymentPlanMetaClass == null)
                {
                    _PaymentPlanMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.PaymentPlanClass.Name);
                }

                return _PaymentPlanMetaClass;
            }
        }

        /// <summary>
        /// Gets the line item meta class.
        /// </summary>
        /// <value>The line item meta class.</value>
        public MetaClass LineItemMetaClass
        {
            get
            {
                if (_LineItemMetaClass == null)
                {
                    _LineItemMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.LineItemClass.Name);
                }

                return _LineItemMetaClass;
            }
        }

        /// <summary>
        /// Gets the shipment meta class.
        /// </summary>
        /// <value>The shipment meta class.</value>
        public MetaClass ShipmentMetaClass
        {
            get
            {
                if (_ShipmentMetaClass == null)
                {
                    _ShipmentMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.ShipmentClass.Name);
                }

                return _ShipmentMetaClass;
            }
        }

        /// <summary>
        /// Gets the order address meta class.
        /// </summary>
        /// <value>The order address meta class.</value>
        public MetaClass OrderAddressMetaClass
        {
            get
            {
                if (_OrderAddressMetaClass == null)
                {
                    _OrderAddressMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.OrderAddressClass.Name);
                }

                return _OrderAddressMetaClass;
            }
        }
        #endregion

        #endregion

        #region Class Definitions
        /// <summary>
        /// Gets the shopping cart class info.
        /// </summary>
        /// <value>The shopping cart class info.</value>
        internal ClassInfo ShoppingCartClassInfo
        {
            get
            {
                return _ShoppingCartClassInfo;
            }
        }

        /// <summary>
        /// Gets the purchase order class info.
        /// </summary>
        /// <value>The purchase order class info.</value>
        internal ClassInfo PurchaseOrderClassInfo
        {
            get
            {
                return _PurchaseOrderClassInfo;
            }
        }

        /// <summary>
        /// Gets the order form class info.
        /// </summary>
        /// <value>The order form class info.</value>
        internal ClassInfo OrderFormClassInfo
        {
            get
            {
                return _OrderFormClassInfo;
            }
        }

        /// <summary>
        /// Gets the payment plan class info.
        /// </summary>
        /// <value>The payment plan class info.</value>
        internal ClassInfo PaymentPlanClassInfo
        {
            get
            {
                return _PaymentPlanClassInfo;
            }
        }

        /// <summary>
        /// Gets the order address class info.
        /// </summary>
        /// <value>The order address class info.</value>
        internal ClassInfo OrderAddressClassInfo
        {
            get
            {
                return _OrderAddressClassInfo;
            }
        }

        /// <summary>
        /// Gets the shipment class info.
        /// </summary>
        /// <value>The shipment class info.</value>
        internal ClassInfo ShipmentClassInfo
        {
            get
            {
                return _ShipmentClassInfo;
            }
        }

        /// <summary>
        /// Gets the line item class info.
        /// </summary>
        /// <value>The line item class info.</value>
        internal ClassInfo LineItemClassInfo
        {
            get
            {
                return _LineItemClassInfo;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderContext"/> class.
        /// </summary>
        OrderContext()
        {
            // Initialize parameters here
            _PurchaseOrderMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.PurchaseOrderClass.Name);

            // Perform auto configuration
            if (OrderConfiguration.Instance.AutoConfigure && _PurchaseOrderMetaClass == null)
            {
                // Setup meta data
                OrderConfiguration.ConfigureMetaData();
            }

            // Initialize parameters here
            if (_PurchaseOrderMetaClass == null)
                _PurchaseOrderMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.PurchaseOrderClass.Name);

            _ShoppingCartMetaClass = MetaClass.Load(MetaDataContext, OrderConfiguration.Instance.MetaClasses.ShoppingCartClass.Name);

            // Load types
            _ShoppingCartClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.ShoppingCartType.Name);
            _PurchaseOrderClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.PurchaseOrderType.Name);
            _OrderFormClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.OrderFormType.Name);
            _PaymentPlanClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.PaymentPlanType.Name);
            _ShipmentClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.ShipmentType.Name);
            _OrderAddressClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.OrderGroupAddressType.Name);
            _LineItemClassInfo = new ClassInfo(OrderConfiguration.Instance.MappedTypes.LineItemType.Name);
        }

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetOrderSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = OrderDataHelper.CreateConfigDataCommand();
			command.CommandText = "GetOrderSchemaVersionNumber";
			DataResult result = DataService.LoadDataSet(command);
			if (result.DataSet != null)
			{
				if (result.DataSet.Tables.Count > 0 && result.DataSet.Tables[0].Rows.Count > 0)
				{
					DataRow row = result.DataSet.Tables[0].Rows[0];
					major = (int)row["Major"];
					minor = (int)row["Minor"];
					patch = (int)row["Patch"];
					installDate = (DateTime)row["InstallDate"];
				}
			}

			return retval;
		}

        /// <summary>
        /// Clears the cached meta data objects. This method should be called when meta data has been modified.
        /// </summary>
        public void ClearMetaCache()
        {
            _PurchaseOrderMetaClass = null;
            _ShoppingCartMetaClass = null;
            _OrderFormMetaClass = null;
            _LineItemMetaClass = null;
            _PaymentPlanMetaClass = null;
            _LineItemMetaClass = null;
            _ShipmentMetaClass = null;
            _OrderAddressMetaClass = null;
        }
	}
}