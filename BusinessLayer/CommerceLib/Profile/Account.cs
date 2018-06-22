using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Profile.Search;
using Mediachase.MetaDataPlus;
using System.Data;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Represents customer account in the system.
    /// </summary>
    public partial class Account : Principal
    {
        #region Public Properties
        /// <summary>
        /// Gets the account id.
        /// </summary>
        /// <value>The account id.</value>
        public int AccountId
        {
            get
            {
                return base.GetInt("Id");
            }
        }

        /// <summary>
        /// Gets or sets the provider key. Typically GUID number.
        /// </summary>
        /// <value>The provider key.</value>
        public string ProviderKey
        {
            get
            {
                return GetString("ProviderKey");
            }
            set
            {
                this["ProviderKey"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>The organization id.</value>
        public int OrganizationId
        {
            get
            {
                if (this["OrganizationId"] != null)
                    return GetInt32("OrganizationId");
                else
                    return 0;
            }
            set
            {
                this["OrganizationId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the customer group.
        /// </summary>
        /// <value>The customer group.</value>
        public string CustomerGroup
        {
            get
            {
                return GetString("CustomerGroup");
            }
            set
            {
                this["CustomerGroup"] = value;
            }
        }

        Organization _Organization = null;
        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <value>The organization.</value>
        public Organization Organization
        {
            get
            {
                if (OrganizationId > 0)
                {
                    if (_Organization == null)
                    {
                        _Organization = Organization.LoadById(OrganizationId);
                    }
                }

                return _Organization;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account() : base(ProfileContext.Current.AccountMetaClass)
        {
            ProviderKey = String.Empty;
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

        /// <summary>
        /// Accepts the changes.
        /// </summary>
		public override void AcceptChanges()
        {
            if (this.ObjectState == MetaObjectState.Added)
            {
                this.Type = "ACCOUNT";
            }
            base.AcceptChanges();
		}

        /// <summary>
        /// Returns CustomerAddress with specified id.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <returns></returns>
		public CustomerAddress FindCustomerAddress(int addressId)
		{
			CustomerAddress retVal = null;
			for (int i = 0; i < this.Addresses.Count; i++)
			{
				CustomerAddress ca = this.Addresses[i];
				if (ca.AddressId == addressId)
				{
					retVal = ca;
					break;
				}
			}
			return retVal;
		}
		#endregion

		#region Static methods
        /// <summary>
        /// Loads the by principal id.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <returns></returns>
		public static Account LoadByPrincipalId(Guid principalId)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_PrincipalId", ProfileContext.Current.AccountMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", ProfileConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("PrincipalId", principalId, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<Account> accounts = LoadSearchResults(searchGuid);
            if (accounts.Count > 0)
                return accounts[0];

            return null;
        }

        /// <summary>
        /// Searches the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public static MetaStorageCollectionBase<Account> Search(ProfileSearch search, out int totalRecords)
        {
            Guid searchGuid = Guid.NewGuid();

            // Perform order search
            totalRecords = search.Search(searchGuid);

            // Load results and return them back
            return LoadSearchResults(searchGuid);
		}
		#endregion

		#region Private Data Help Methods
        /// <summary>
        /// Loads the search results.
        /// </summary>
        /// <param name="SearchGuid">The search GUID.</param>
        /// <returns></returns>
		private static MetaStorageCollectionBase<Account> LoadSearchResults(Guid SearchGuid)
        {
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}", ProfileContext.Current.AccountMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SearchSetId", SearchGuid, DataParameterType.UniqueIdentifier));

            // Might be good idea to signal if there are results at all
            DataResult result = DataService.LoadDataSet(cmd);

            MetaStorageCollectionBase<Account> accounts = new MetaStorageCollectionBase<Account>();

            PopulateCollection<Account>(ProfileContext.Current.AccountClassInfo, accounts, result.DataSet);
            return accounts;
        }
        #endregion

		#region Internal methods
        /// <summary>
        /// Loads the by provider key.
        /// </summary>
        /// <param name="ProviderKey">The provider key.</param>
        /// <returns></returns>
		internal static Account LoadByProviderKey(string ProviderKey)
        {
            Guid searchGuid = Guid.NewGuid();
            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Search_{0}_ProviderKey", ProfileContext.Current.AccountMetaClass.Name);
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", ProfileConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SearchSetId", searchGuid, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("ProviderKey", ProviderKey, DataParameterType.NVarChar));

            // Might be good idea to signal if there are results at all
            DataService.Run(cmd);

            // Load results and return them back
            MetaStorageCollectionBase<Account> accounts = LoadSearchResults(searchGuid);
            if (accounts.Count > 0)
                return accounts[0];

            return null;
		}
		#endregion
	}
}
