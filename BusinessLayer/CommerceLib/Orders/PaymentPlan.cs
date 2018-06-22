using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Data.Provider;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Payment plan cycles.
    /// </summary>
    public enum PaymentPlanCycle
    {
        /// <summary>
        /// Unknown Plan Cycle
        /// </summary>
        None = 0,
        /// <summary>
        /// Daily Cycle
        /// </summary>
        Days = 1,
        /// <summary>
        /// Weekly Cycle
        /// </summary>
        Weeks,
        /// <summary>
        /// Monthly Cycle
        /// </summary>
        Months,
        /// <summary>
        /// Yearly Cycle
        /// </summary>
        Years,
        /// <summary>
        /// Custom1 Cycle, use your own time frame.
        /// </summary>
        Custom1,
        /// <summary>
        /// Custom2 Cycle, use your own time frame.
        /// </summary>
        Custom2
    }

    /// <summary>
    /// Payment Plan is a type of order that will be a recurring order.
    /// </summary>
    [Serializable()]
    public class PaymentPlan : OrderGroup, ISerializable
    {
        private CreateOrderNumber _OrderNumberMethod;
        
        /// <summary>
        /// Delegate for generating custom order tracking number. Please look at OrderNumberMethod property for details on how it works.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public delegate string CreateOrderNumber(PaymentPlan cart);

        #region Payment Plan Properties        
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

        /// <summary>
        /// Gets or sets the cycle mode. Cycle modes available are days, weeks, months and years as well as None, Custom1 and Custom2.
        /// </summary>
        /// <value>The cycle mode.</value>
        public virtual PaymentPlanCycle CycleMode
        {
            get
            {
                return (PaymentPlanCycle)GetInt("PlanCycleMode");
            }
            set
            {
                base["PlanCycleMode"] = value.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the length of the cycle. For example to charge customer monthly, set this value
        /// to 1 and CycleMode to Months.
        /// </summary>
        /// <value>The length of the cycle.</value>
        public virtual int CycleLength
        {
            get
            {
                return GetInt("PlanCycleLength");
            }
            set
            {
                base["PlanCycleLength"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the max cycles count. So for instance if you want to charge customer for a year each month,
        /// set CycleMode to month, CycleLength to 1 and MaxCyclesCount to 12.
        /// Leave at 0 if you never want this cycle to end.
        /// </summary>
        /// <value>The max cycles count.</value>
        public virtual int MaxCyclesCount
        {
            get
            {
                return GetInt("PlanMaxCyclesCount");
            }
            set
            {
                base["PlanMaxCyclesCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the completed cycles count. This property will be auto encreased on each new generated sale.
        /// </summary>
        /// <value>The completed cycles count.</value>
        public virtual int CompletedCyclesCount
        {
            get
            {
                return GetInt("PlanCompletedCyclesCount");
            }
            set
            {
                base["PlanCompletedCyclesCount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the start date for first payment.
        /// </summary>
        /// <value>The start date.</value>
        public virtual DateTime StartDate
        {
            get
            {
                return GetDateTime("PlanStartDate");
            }
            set
            {
                base["PlanStartDate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the stop date for the last payment.
        /// </summary>
        /// <value>The stop date.</value>
        public virtual DateTime EndDate
        {
            get
            {
                return GetDateTime("PlanEndDate");
            }
            set
            {
                base["PlanEndDate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the last transaction date.
        /// </summary>
        /// <value>The last transaction date.</value>
        public virtual DateTime LastTransactionDate
        {
            get
            {
                return GetDateTime("LastTransactionDate");
            }
            set
            {
                base["LastTransactionDate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Pasyment Plan is active.
        /// </summary>
        /// <value><c>true</c> if this plan is active; otherwise, <c>false</c>.</value>
        public virtual bool IsActive
        {
            get
            {
                return GetBool("PlanIsActive");
            }
            set
            {
                base["PlanIsActive"] = value;
            }
        }

        /// <summary>
        /// Returns true if payment plan payment is due
        /// </summary>
        /// <value><c>true</c> if this instance is due; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsDue
        {
            get
            {
                if (!this.IsActive || this.StartDate > DateTime.UtcNow || (this.MaxCyclesCount > 0 && (this.EndDate <= DateTime.UtcNow || this.EndDate == DateTime.MinValue)))
                    return false;

                if (this.MaxCyclesCount > 0 && this.MaxCyclesCount <= this.CompletedCyclesCount)
                    return false;

                // Check if next payment is due
                if (DateTime.UtcNow < this.NextTransactionDate)
                    return false;

                return true;
            }
        }

        /*
        /// <summary>
        /// Returns total amount generated by this payment plan
        /// </summary>
        public decimal TotalGross
        {
            get
            {
                return this.CompletedCyclesCount * this.Amount;
            }
        }
         * */

        /// <summary>
        /// Returns the date of the next transaction.
        /// </summary>
        /// <value>The next transaction date. In UTC Format.</value>
        /// <remarks>
        /// Gets the Date for the next transaction based on the type of
        /// Payment Plan
        /// </remarks>
        [XmlIgnore]
        public DateTime NextTransactionDate
        {
            get
            {
                if (!IsActive)
                    return DateTime.MinValue;

                if (LastTransactionDate == DateTime.MinValue)
                {
                    if (CompletedCyclesCount == 0)
                        return this.StartDate;
                    else
                        return DateTime.UtcNow;
                }

                switch (this.CycleMode)
                {
                    case PaymentPlanCycle.Days:
                        return this.LastTransactionDate.AddDays(this.CycleLength).Date;
                    case PaymentPlanCycle.Weeks:
                        return this.LastTransactionDate.Date.AddDays(7 * this.CycleLength).Date;
                    case PaymentPlanCycle.Months:
                        return this.LastTransactionDate.Date.AddMonths(this.CycleLength);
                    case PaymentPlanCycle.Years:
                        return this.LastTransactionDate.Date.AddYears(this.CycleLength);
                    default:
                        return DateTime.MinValue;
                }
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentPlan"/> class.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="CustomerId">The customer id.</param>
        internal PaymentPlan(string Name, Guid CustomerId)
            : base(CustomerId, OrderContext.Current.PaymentPlanMetaClass) 
        {
            this.Name = Name;
            Initialize();
        }

        /// <summary>
        /// Internal constructor required by collection implementation.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public PaymentPlan(IDataReader reader)
            : base(OrderContext.Current.PaymentPlanMetaClass, reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentPlan"/> class.
        /// </summary>
        protected PaymentPlan()
            : base(Guid.Empty, OrderContext.Current.PaymentPlanMetaClass)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentPlan"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PaymentPlan(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

        /// <summary>
        /// Initializes this instance with default values.
        /// </summary>
        private void Initialize()
        {
            this.LastTransactionDate = DateTime.MinValue;
            this.IsActive = true;
            this.MaxCyclesCount = 0;
            this.CycleMode = PaymentPlanCycle.None;
            this.CycleLength = 0;
            this.CompletedCyclesCount = 0;
            this.EndDate = DateTime.MaxValue;
        }

        /// <summary>
        /// Loads the payment plan by customer and order group id.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="OrderGroupId">The order group id.</param>
        /// <returns></returns>
        public static PaymentPlan LoadByCustomerAndOrderGroupId(Guid CustomerId, int OrderGroupId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_CustomerAndOrderGroupId", OrderContext.Current.PaymentPlanMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CustomerId", CustomerId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("OrderGroupId", OrderGroupId, DataParameterType.Int));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<PaymentPlan> orders = LoadSearchResults(searchGuid);

            if (orders.Count > 0)
                return orders[0];

            return null;
        }

        /// <summary>
        /// Loads the Payment Plan by customer.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<PaymentPlan> LoadByCustomer(Guid CustomerId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_Customer", OrderContext.Current.PaymentPlanMetaClass.Name);
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
        /// Populates from cart.
        /// </summary>
        /// <param name="cart">The cart.</param>
        public virtual void PopulateFromCart(Cart cart)
        {
            this.Id = (cart.OrderGroupId);
        }

        /// <summary>
        /// Searches for payment plan.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<PaymentPlan> Search(OrderSearch search, out int totalRecords)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            totalRecords = search.Search(searchGuid);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
        }

        /// <summary>
        /// Saves as purchase order. This method should be called when recurring payment is processed. So for instance if it is a monthly plan,
        /// call this method on monthly basis.
        /// </summary>
        public virtual PurchaseOrder SaveAsPurchaseOrder()
        {
            Guid customerId = this.CustomerId;
            int orderGroupId = this.OrderGroupId;

            using (TransactionScope scope = new TransactionScope())
            {
                // Clone the object
                // need to set meta data context before cloning
                MetaDataContext.DefaultCurrent = OrderContext.MetaDataContext;
                PaymentPlan plan = (PaymentPlan)this.Clone();
                plan.SetParent(plan);

                // Second: create new purchase order
                PurchaseOrder purchaseOrder = (PurchaseOrder)OrderContext.Current.PurchaseOrderClassInfo.CreateInstance();
                
                // Third: migrate order group
                purchaseOrder.Initialize((OrderGroup)plan);

                // relate purchase order to this payment plan
                purchaseOrder.ParentOrderGroupId = orderGroupId;

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

            return OrderContext.Current.GetPurchaseOrder(customerId, orderGroupId);
        }

        #region Private Data Help Methods
        /// <summary>
        /// Generates the order number.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <returns></returns>
        private string GenerateOrderNumber(PaymentPlan plan)
        {
            string num = new Random().Next(100, 999).ToString();
            return String.Format("PO{0}{1}", plan.OrderGroupId, num);
        }

        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="SearchGuid">The search GUID.</param>
        /// <returns></returns>
        private static MetaStorageCollectionBase<PaymentPlan> LoadSearchResults(Guid SearchGuid)
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}", OrderContext.Current.PaymentPlanMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SearchSetId", SearchGuid, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataResult result = DataService.LoadDataSet(cmd);

            MetaStorageCollectionBase<PaymentPlan> orders = new MetaStorageCollectionBase<PaymentPlan>();

            PopulateCollection<PaymentPlan>(OrderContext.Current.PaymentPlanClassInfo, orders, result.DataSet);
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
