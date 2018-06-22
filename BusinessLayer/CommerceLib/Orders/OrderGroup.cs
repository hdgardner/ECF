using System;
using Mediachase.MetaDataPlus.Common;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.MetaDataPlus;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Engine;
using System.Workflow.Runtime;
using System.Workflow.ComponentModel;
using System.Threading;
using Common.Logging;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Order group contains multiple order forms.
    /// </summary>
    [Serializable]
    public abstract class OrderGroup : OrderStorageBase, ISerializable
    {
        #region Private Fields
        private OrderFormCollection _OrderForms;
        private OrderAddressCollection _OrderAddresses;
        private readonly ILog Logger;
        #endregion

        #region Public Collection Properties
        /// <summary>
        /// Gets the order forms.
        /// </summary>
        /// <value>The order forms.</value>
        public OrderFormCollection OrderForms
        {
            get
            {
                return _OrderForms;
            }
        }

        /// <summary>
        /// Gets the order addresses.
        /// </summary>
        /// <value>The order addresses.</value>
        public OrderAddressCollection OrderAddresses
        {
            get
            {
                return _OrderAddresses;
            }
        }
        #endregion

        #region Public Field Properties
        /// <summary>
        /// Gets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                return GetInt32("OrderGroupId");
            }
        }

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        /// <value>The instance id.</value>
        public Guid InstanceId
        {
            get
            {
                return GetGuid("InstanceId");
            }
            set
            {
                this["InstanceId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        /// <value>The application id.</value>
        public Guid ApplicationId
        {
            get
            {
                return GetGuid("ApplicationId");
            }
            set
            {
                this["ApplicationId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the affiliate id.
        /// </summary>
        /// <value>The affiliate id.</value>
        public Guid AffiliateId
        {
            get
            {
                return GetGuid("AffiliateId");
            }
            set
            {
                this["AffiliateId"] = value;
            }
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
        /// Gets or sets the customer id.
        /// </summary>
        /// <value>The customer id.</value>
        public Guid CustomerId
        {
            get
            {
                return GetGuid("CustomerId");
            }
            set
            {
                this["CustomerId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        public string CustomerName
        {
            get
            {
                return GetString("CustomerName");
            }
            set
            {
                this["CustomerName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        /// <value>The address id.</value>
        public string AddressId
        {
            get
            {
                return GetString("AddressId");
            }
            set
            {
                this["AddressId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping total.
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
        /// Gets or sets the billing currency.
        /// </summary>
        /// <value>The billing currency.</value>
        public string BillingCurrency
        {
            get
            {
                return GetString("BillingCurrency");
            }
            set
            {
                this["BillingCurrency"] = value;
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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderGroup"/> class.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
        internal OrderGroup(Guid CustomerId, MetaClass metaClass, IDataReader reader)
            : base(metaClass, reader) 
        {
            Logger = LogManager.GetLogger(GetType());
            Initialize(CustomerId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderGroup"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
        internal OrderGroup(MetaClass metaClass, IDataReader reader)
            : base(metaClass, reader)
        {
            Logger = LogManager.GetLogger(GetType());
            Initialize(Guid.Empty);
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderGroup"/> class.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        /// <param name="metaClass">The meta class.</param>
        internal OrderGroup(Guid CustomerId, MetaClass metaClass)
            : base(metaClass)
        {
            Logger = LogManager.GetLogger(GetType());
            Initialize(CustomerId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderGroup"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrderGroup(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            Logger = LogManager.GetLogger(GetType());
            _OrderForms = (OrderFormCollection)info.GetValue("OrderForms", typeof(OrderFormCollection));
            _OrderAddresses = (OrderAddressCollection)info.GetValue("OrderAddresses", typeof(OrderAddressCollection));
        }

        /// <summary>
        /// Initializes the specified customer id.
        /// </summary>
        /// <param name="CustomerId">The customer id.</param>
        private void Initialize(Guid CustomerId)
        {
            this["OrderGroupId"] = 0;
            this.CustomerId = CustomerId;

            this.InstanceId = Guid.NewGuid();
            this.AddressId = String.Empty;
            this.BillingCurrency = String.Empty;
            this.CustomerName = String.Empty;
            this.Name = String.Empty;
            this.ShippingTotal = 0;
            this.Status = String.Empty;
            this.SubTotal = 0;
            this.TaxTotal = 0;
            this.Total = 0;
            this.HandlingTotal = 0;
            this.ApplicationId = OrderConfiguration.Instance.ApplicationId;

            // Initialize Collections
            _OrderForms = new OrderFormCollection(this);
            _OrderAddresses = new OrderAddressCollection(this);
        }

        /// <summary>
        /// Initializes the object with data from the specified order group.
        /// </summary>
        /// <param name="orderGroup">The order group.</param>
        internal virtual void Initialize(OrderGroup orderGroup)
        {
            _OrderForms = orderGroup.OrderForms;
            _OrderAddresses = orderGroup.OrderAddresses;

            this.SetParent(this);

            // Copy all the fields
            this.AddressId = orderGroup.AddressId;
            this.AffiliateId = orderGroup.AffiliateId;
            this.ApplicationId = orderGroup.ApplicationId;
            this.BillingCurrency = orderGroup.BillingCurrency;
            this.CustomerId = orderGroup.CustomerId;
            this.CustomerName = orderGroup.CustomerName;
            this.HandlingTotal = orderGroup.HandlingTotal;
            // this must be unique, so use the one generated this.InstanceId = orderGroup.InstanceId;
            this.Name = orderGroup.Name;
            this.ProviderId = orderGroup.ProviderId;
            this.ShippingTotal = orderGroup.ShippingTotal;
            this.Status = orderGroup.Status;
            this.SubTotal = orderGroup.SubTotal;
            this.TaxTotal = orderGroup.TaxTotal;
            this.Total = orderGroup.Total;
        }

        #endregion

        #region Protected Static Methods
        /// <summary>
        /// Populates the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classInfo">The class info.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="dataSet">The data set.</param>
        protected static void PopulateCollection<T>(ClassInfo classInfo, MetaStorageCollectionBase<T> collection, DataSet dataSet) where T : OrderGroup
        {
            #region ArgumentNullExceptions
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");
            #endregion

            DataTableCollection tables = dataSet.Tables;

            if (tables == null || tables.Count == 0)
                return;

            // Get the collection that contains ids of all items returned
            DataRowCollection dataRowCol = tables[0].Rows;

            // No ids returned
            if (dataRowCol == null || dataRowCol.Count == 0)
                return;

            int numberOfRecords = dataRowCol.Count;
            int[] idArray = new int[numberOfRecords];

            // Populate array
            for (int index = 0; index < numberOfRecords; index++)
            {
                idArray[index] = (int)dataRowCol[index]["OrderGroupId"];
            }

            // Remove id collection
            tables.RemoveAt(0);

            // Map table names
            foreach (DataTable table in dataSet.Tables)
            {
                if (table.Rows.Count > 0)
                    table.TableName = table.Rows[0]["TableName"].ToString();
            }

            // Cycle through id Collection
            foreach (int id in idArray)
            {
                string filter = String.Format("OrderGroupId = '{0}'", id.ToString());
                
                // Populate the meta object data first
                // Meta data will be always in the second table returned
                DataView view = DataHelper.CreateDataView(dataSet.Tables["OrderGroup"], filter);

                // Only read one record, since we are populating only one object
                // reader.Read();

                // Create instance of the object specified and populate it
                T obj = (T)classInfo.CreateInstance();
                obj.Load(view[0]);
                collection.Add(obj);

                // Populate collections within object
                obj.PopulateCollections(dataSet.Tables, filter);

                /*
                if (dataSet.Tables.Count != 0)
                {
                    throw new ArgumentOutOfRangeException("Tables", "Stored Procedure returned too many tables");
                }
                 * */
            }
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
            base.PopulateCollections(tables, filter);

            // Populate object collections               
            DataView view = DataHelper.CreateDataView(tables["OrderForm"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                // Populate Forms Collection
                OrderForm orderForm = (OrderForm)OrderContext.Current.OrderFormClassInfo.CreateInstance();
                orderForm.Load(row);
                orderForm.PopulateCollectionsInternal(tables, filter);
                OrderForms.Add(orderForm);
            }

            view = DataHelper.CreateDataView(tables["OrderGroupAddress"], filter);

            foreach (DataRowView row in view)
            {
                // Populate Address Collection
                OrderAddress orderAddress = (OrderAddress)OrderContext.Current.OrderAddressClassInfo.CreateInstance();
                orderAddress.Load(row);
                OrderAddresses.Add(orderAddress);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public override void SetParent(object parent)
        {
            OrderAddresses.SetParent((OrderGroup)parent);
            OrderForms.SetParent((OrderGroup)parent);
        }
        #endregion

        #region Save Methods

        /// <summary>
        /// Marks current instance as new which will cause new record to be created in the database for the specified object.
        /// This is useful for creating duplicates of existing objects.
        /// </summary>
        internal override void MarkNew()
        {
            base.MarkNew();
            OrderAddresses.MarkNew();
            OrderForms.MarkNew();
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public override void Delete()
        {
            foreach (OrderForm orderForm in OrderForms)
            {
                orderForm.Delete();
            }

            // Save addresses
            foreach (OrderAddress orderAddress in OrderAddresses)
            {
                orderAddress.Delete();
            }

            base.Delete();
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                // Save base object first
                base.AcceptChanges();

                if (this.ObjectState != MetaObjectState.Deleted)
                {
                    // Populate object primary key here

                    // Save forms
                    OrderForms.AcceptChanges();

                    // Save addresses
                    OrderAddresses.AcceptChanges();
                }

                scope.Complete();
            }
        }
        #endregion

        #region Workflow Methods
        /// <summary>
        /// Runs the specified workflow, exception will be thrown and should be handled by the caller. The execution will be synchronious.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual WorkflowResults RunWorkflow(string name)
        {
            return RunWorkflow2(name, false, true);
        }

        /// <summary>
        /// Runs the specified workflow.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="throwException">if set to <c>true</c> the exception will be thrown and should be handled by the caller.</param>
        /// <returns></returns>
        public virtual WorkflowResults RunWorkflow(string name, bool throwException)
        {
            return RunWorkflow2(name, false, throwException);
        }

        /// <summary>
        /// Runs the workflow2.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="async">if set to <c>true</c> [async].</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception].</param>
        /// <returns></returns>
        private WorkflowResults RunWorkflow2(string name, bool async, bool throwException)
        {
            // Add input parameter
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderGroup", this);

            Logger.Debug(String.Format("Starting the workflow \"{0}\".", name));
            // Execute workflow
            WorkflowManager.ExecuteWorkflow(name, dic, this.InstanceId);

            /*
            if (!async)
            {
                // Wait for workflow to complete
                OrderContext.Current.WorkflowWait();
            }
             * */

            // Check exception
            if (throwException)
            {
                if (OrderContext.Current.WorkflowResults.Exception != null)
                {
                    Logger.Debug(String.Format("Workflow \"{0}\" finished with exception.", name), OrderContext.Current.WorkflowResults.Exception);
                    throw OrderContext.Current.WorkflowResults.Exception;
                    //throw new ApplicationException("Workflow generated an exception, please look at the previous error for more details.", OrderContext.Current.WorkflowResults.Exception);
                }
            }

            Logger.Debug(String.Format("Workflow \"{0}\" finished.", name));
            return OrderContext.Current.WorkflowResults;
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

            info.AddValue("OrderForms", this.OrderForms, typeof(OrderFormCollection));
            info.AddValue("OrderAddresses", this.OrderAddresses, typeof(OrderAddressCollection));
        }

        #endregion
    }
}