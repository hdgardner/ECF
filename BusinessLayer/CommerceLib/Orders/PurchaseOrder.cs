using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using System;
using Mediachase.Data.Provider;
using System.Data;
using System.Collections;
using System.Data.Common;
using System.Collections.Generic;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Orders.Search;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders {
    /// <summary>
    /// Purchase Order is the actual recorded sale.
    /// </summary>
	public class PurchaseOrder : OrderGroup, ISerializable
    {
        #region Internal Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrder"/> class.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="reader">The reader.</param>
        internal PurchaseOrder(Guid CustomerId, IDataReader reader)
            : base(CustomerId, OrderContext.Current.PurchaseOrderMetaClass, reader) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrder"/> class.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        internal PurchaseOrder(Guid CustomerId)
            : base(CustomerId, OrderContext.Current.PurchaseOrderMetaClass) 
        {
            Initialize();
        }

        /// <summary>
        /// Internal constructor required by collection implementation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public PurchaseOrder(IDataReader reader)
            : base(OrderContext.Current.PurchaseOrderMetaClass, reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrder"/> class.
        /// </summary>
        protected PurchaseOrder()
            : base(Guid.Empty, OrderContext.Current.PurchaseOrderMetaClass)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            ParentOrderGroupId = 0;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the parent order group id. This can be used to relate purchase orders to a payment plan or other order.
        /// </summary>
        /// <value>The parent order group instance id.</value>
        public int ParentOrderGroupId
        {
            get
            {
                return GetInt("ParentOrderGroupId");
            }
            set
            {
                this["ParentOrderGroupId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tracking number. Automatically generated or can be generated using provided function.
        /// </summary>
        /// <value>The tracking number.</value>
        public string TrackingNumber
        {
            get
            {
                return GetString("TrackingNumber");
            }
            set
            {
                this["TrackingNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration date. Expiration date can be used for subscription type of orders.
        /// </summary>
        /// <value>The expiration date.</value>
        public DateTime ExpirationDate
        {
            get
            {
                return base.GetDateTime("ExpirationDate");
            }
            set
            {
                this["ExpirationDate"] = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads the by customer and order group id.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="OrderGroupId">The order group id.</param>
        /// <returns></returns>
        public static PurchaseOrder LoadByCustomerAndOrderGroupId(Guid CustomerId, int OrderGroupId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_CustomerAndOrderGroupId", OrderContext.Current.PurchaseOrderMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CustomerId", CustomerId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("OrderGroupId", OrderGroupId, DataParameterType.Int));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<PurchaseOrder> orders = LoadSearchResults(searchGuid);

            if (orders.Count > 0)
                return orders[0];

            return null;
        }

        /// <summary>
        /// Loads the purchase order by customer.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<PurchaseOrder> LoadByCustomer(Guid CustomerId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_Customer", OrderContext.Current.PurchaseOrderMetaClass.Name);
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
        /// Searches for Purchase Order.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<PurchaseOrder> Search(OrderSearch search, out int totalRecords)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            totalRecords = search.Search(searchGuid);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
        }

        /// <summary>
        /// Populates from cart.
        /// </summary>
        /// <param name="cart">The cart.</param>
        public virtual void PopulateFromCart(Cart cart)
        {
			this.Id = cart.OrderGroupId;
        }
        #endregion

        #region Private Data Help Methods
        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="SearchGuid">The search GUID.</param>
        /// <returns></returns>
        private static MetaStorageCollectionBase<PurchaseOrder> LoadSearchResults(Guid SearchGuid)
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}", OrderContext.Current.PurchaseOrderMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SearchSetId", SearchGuid, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataResult result = DataService.LoadDataSet(cmd);

            MetaStorageCollectionBase<PurchaseOrder> orders = new MetaStorageCollectionBase<PurchaseOrder>();

            PopulateCollection<PurchaseOrder>(OrderContext.Current.PurchaseOrderClassInfo, orders, result.DataSet);
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