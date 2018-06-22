using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using System.Data;
using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Base class for account and organization.
    /// </summary>
    public abstract class Principal : ProfileStorageBase
    {
        #region Private Fields
        private CustomerAddressCollection _Addresses;
        #endregion

        #region Public Collection Properties
        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <value>The addresses.</value>
        public CustomerAddressCollection Addresses
        {
            get
            {
                return _Addresses;
            }
        }
        #endregion

		#region Public Field Properties
        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        /// <value>The principal id.</value>
		public Guid PrincipalId
		{
			get
			{
				return base.GetGuid("PrincipalId");
			}
			set
			{
				base["PrincipalId"] = value;
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
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
		public string Description
		{
			get
			{
				return GetString("Description");
			}
			set
			{
				this["Description"] = value;
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
				return base.GetGuid("ApplicationId");
			}
			set
			{
				base["ApplicationId"] = value;
			}
		}

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
		public string Type
		{
			get
			{
				return base.GetString("Type");
			}
			set
			{
				base["Type"] = value;
			}
		}

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
		public int State
		{
			get
			{
				return base.GetInt32("State");
			}
			set
			{
				base["State"] = value;
			}
		}
		#endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Principal"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
		internal Principal(MetaClass metaClass, IDataReader reader)
			: base(metaClass, reader)
		{
			Initialize(Guid.Empty);
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Principal"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
		internal Principal(MetaClass metaClass)
			: base(metaClass)
		{
			Initialize(Guid.Empty);
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Principal"/> class.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
		internal Principal(Guid principalId, MetaClass metaClass, IDataReader reader)
			: base(metaClass, reader)
		{
			Initialize(principalId);
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Principal"/> class.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <param name="metaClass">The meta class.</param>
		internal Principal(Guid principalId, MetaClass metaClass)
			: base(metaClass)
		{
			Initialize(principalId);
		}

        /// <summary>
        /// Initializes the specified principal id.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
		private void Initialize(Guid principalId)
		{
			this.PrincipalId = principalId;
			this.ApplicationId = ProfileConfiguration.Instance.ApplicationId;

			_Addresses = new CustomerAddressCollection(this);
		}
        #endregion

        /// <summary>
        /// Accepts the changes.
        /// </summary>
		public override void AcceptChanges()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (this.ObjectState == MetaObjectState.Added)
                {
                    if (this.PrincipalId == Guid.Empty)
                        this.PrincipalId = Guid.NewGuid();

                    this.ApplicationId = ProfileConfiguration.Instance.ApplicationId;
                }

                // Save base object first
                base.AcceptChanges();

                if (this.ObjectState != MetaObjectState.Deleted)
                {
                    // Populate object primary key here

                    // Save addresses
                    Addresses.AcceptChanges();
                }

                scope.Complete();
            }
        }

		#region Public Methods
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
		public override void SetParent(object parent)
		{
			Addresses.SetParent((Principal)this);
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
        protected static void PopulateCollection<T>(ClassInfo classInfo, MetaStorageCollectionBase<T> collection, DataSet dataSet) where T : Principal
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
            Guid[] idArray = new Guid[numberOfRecords];

            // Populate array
            for (int index = 0; index < numberOfRecords; index++)
            {
                idArray[index] = (Guid)dataRowCol[index]["PrincipalId"];
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
            foreach (Guid id in idArray)
            {
                string filter = String.Format("PrincipalId = '{0}'", id.ToString());

                // Populate the meta object data first
                // Meta data will be always in the second table returned
                DataView view = DataHelper.CreateDataView(dataSet.Tables["Principal"], filter);

                // Only read one record, since we are populating only one object
                // reader.Read();

                // Create instance of the object specified and populate it
                T obj = (T)classInfo.CreateInstance();
				if(view.Count>0)
					obj.Load(view[0]);
                collection.Add(obj);

                // Populate collections within object
                obj.PopulateCollections(dataSet.Tables, filter);
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
            DataView view = DataHelper.CreateDataView(tables["CustomerAddress"], filter);

            // Read until we are done, since this is a collection
            foreach (DataRowView row in view)
            {
                // Populate Address Collection
                CustomerAddress address = (CustomerAddress)ProfileContext.Current.CustomerAddressClassInfo.CreateInstance();
                address.Load(row);
                Addresses.Add(address);
            }
        }
        #endregion
    }
}