using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Web.Profile;
using System.Web;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Profile.Search;
using Mediachase.MetaDataPlus;
using System.Web.Hosting;
using System.Security.Principal;
using System.Threading;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile.Dto;
using Mediachase.Commerce.Engine.Caching;
using Mediachase.Commerce.Profile.Data;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Implements operations for and represents the profile context.
    /// </summary>
    public class ProfileContext
	{
		#region Private Fields
		private MetaClass _CustomerAddressMetaClass;
        private MetaClass _AccountMetaClass;
        private MetaClass _OrganizationMetaClass;

        private ClassInfo _CustomerAddressClassInfo;
        private ClassInfo _AccountClassInfo;
        private ClassInfo _OrganizationClassInfo;

        #endregion

        /// <summary>
        /// Contains the constant string used for anonymous user.
        /// </summary>
        public static readonly string Anonymous = "Anonymous";

        #region Meta Classes
        /// <summary>
        /// Gets the customer address meta class.
        /// </summary>
        /// <value>The customer address meta class.</value>
        public MetaClass CustomerAddressMetaClass
        {
            get
            {
                if (_CustomerAddressMetaClass == null)
                {
                    _CustomerAddressMetaClass = MetaClass.Load(ProfileContext.MetaDataContext, ProfileConfiguration.Instance.MetaClasses.CustomerAddressClass.Name);
                }

                return _CustomerAddressMetaClass;
            }
        }

        /// <summary>
        /// Gets the account meta class.
        /// </summary>
        /// <value>The account meta class.</value>
        public MetaClass AccountMetaClass
        {
            get
            {
                if (_AccountMetaClass == null)
                {
                    _AccountMetaClass = MetaClass.Load(ProfileContext.MetaDataContext, ProfileConfiguration.Instance.MetaClasses.AccountClass.Name);
                }

                return _AccountMetaClass;
            }
        }

        /// <summary>
        /// Gets the organization meta class.
        /// </summary>
        /// <value>The organization meta class.</value>
        public MetaClass OrganizationMetaClass
        {
            get
            {
                if (_OrganizationMetaClass == null)
                {
                    _OrganizationMetaClass = MetaClass.Load(ProfileContext.MetaDataContext, ProfileConfiguration.Instance.MetaClasses.OrganizationClass.Name);
                }

                return _OrganizationMetaClass;
            }
        }

        /// <summary>
        /// Gets the customer address class info.
        /// </summary>
        /// <value>The customer address class info.</value>
        internal ClassInfo CustomerAddressClassInfo
        {
            get
            {
                return _CustomerAddressClassInfo;
            }
        }

        /// <summary>
        /// Gets the account class info.
        /// </summary>
        /// <value>The account class info.</value>
        internal ClassInfo AccountClassInfo
        {
            get
            {
                return _AccountClassInfo;
            }
        }

        /// <summary>
        /// Gets the organization class info.
        /// </summary>
        /// <value>The organization class info.</value>
        internal ClassInfo OrganizationClassInfo
        {
            get
            {
                return _OrganizationClassInfo;
            }
        }

        #endregion

        private static volatile ProfileContext _Instance = null;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static ProfileContext Current
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ProfileContext();
                        }
                    }
                }

                return _Instance;
            }
        }

        private static MetaDataContext _mdContext = null;
        /// <summary>
        /// Gets or sets the meta data context.
        /// </summary>
        /// <value>The meta data context.</value>
        public static MetaDataContext MetaDataContext
        {
            get
            {
                if (_mdContext == null)
                    _mdContext = new MetaDataContext(ProfileConfiguration.Instance.Connection.AppDatabase);

                return _mdContext;
            }
            set
            {
                _mdContext = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileContext"/> class.
        /// </summary>
        ProfileContext()
        {
            // Perform auto configuration
            if (ProfileConfiguration.Instance.AutoConfigure && CustomerAddressMetaClass == null)
            {
                // Setup meta data
                ProfileConfiguration.ConfigureMetaData();
            }

            // Load types
            _CustomerAddressClassInfo = new ClassInfo(ProfileConfiguration.Instance.MappedTypes.CustomerAddress.Name);
            _AccountClassInfo = new ClassInfo(ProfileConfiguration.Instance.MappedTypes.Account.Name);
            _OrganizationClassInfo = new ClassInfo(ProfileConfiguration.Instance.MappedTypes.Organization.Name);
        }

        /// <summary>
        /// Gets the user. Results are cached.
        /// </summary>
        /// <value>The user.</value>
        public MembershipUser User
        {
            get
            {
                if (IsProviderEnabled && !String.IsNullOrEmpty(GetCurrentUserName()))
                {
                    string cacheKey = ProfileCache.CreateCacheKey("user", GetCurrentUserName());

                    MembershipUser user = null;
                    object cachedObject = ProfileCache.Get(cacheKey);

                    if (cachedObject != null)
                        user = (MembershipUser)cachedObject;

                    // Load the object
                    if (user == null)
                    {
                        lock (ProfileCache.GetLock(cacheKey))
                        {
                            cachedObject = ProfileCache.Get(cacheKey);
                            if (cachedObject != null)
                            {
                                user = (MembershipUser)cachedObject;
                            }
                            else
                            {
                                user = Membership.GetUser();

                                // Insert to the cache collection
                                ProfileCache.Insert(cacheKey, user, ProfileConfiguration.Instance.Cache.UserTimeout);
                            }
                        }
                    }

                    return user;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public CustomerProfile Profile
        {
            get
            {               
                return HttpContext.Current.Profile as CustomerProfile;
            }
        }

        Guid _UserId = Guid.Empty;
        /// <summary>
        /// Gets or sets the user id. If user is authenticated, it will return PrincipalId. If user is not authenticated
        /// and anonymous identification is enabled, the HttpContext.Current.Request.AnonymousID will be returned.
        /// Otherwise new id is generated and returned.
        /// </summary>
        /// <value>The user id.</value>
        public Guid UserId
        {
            get
            {
                if (User != null)
                {
                    Account account = ProfileContext.Current.GetAccount(true);
                    return account.PrincipalId;
                }
                else
                {
                    // Return anonymous user id
                    if (HttpContext.Current != null)
                    {
                        if (AnonymousIdentificationModule.Enabled)
                        {
                            return new Guid(HttpContext.Current.Request.AnonymousID);
                        }
                    }
                }

                if (_UserId == Guid.Empty)
                    _UserId = Guid.NewGuid();

                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                if (User != null)
                    return User.UserName;
                else
                    return Anonymous;
            }
        }

        /// <summary>
        /// Gets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        public string CustomerName
        {
            get
            {
                string name = String.Empty;
                Account account = GetAccount(false);
                if (account != null)
                {
                    name = account.Name;
                }

                if (String.IsNullOrEmpty(name))
                {
                    if (User != null)
                        name = User.UserName;
                    else
                        name = Anonymous;
                }

                return name;
            }
        }

        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        public Account GetAccount(bool create)
        {
            MembershipUser user = User;
            if (user == null)
                return null;

            Account account = GetAccount(user.ProviderUserKey.ToString());
            if (account == null)
            {
                if (create)
                    account = CreateAccountForUser(user);
            }

            return account;
        }

		/// <summary>
		/// Creates Account for user, if it doesn't exist yet.
		/// </summary>
		/// <returns></returns>
		public Account CreateAccountForUser()
		{
			if (this.Profile == null || this.Profile.Account != null)
				return null;

			MembershipUser user = this.User;
			if (user != null)
			{
				// Now create an account in the ECF 
				Account account = new Account();
				account.ProviderKey = user.ProviderUserKey.ToString();
				account.Name = user.UserName;
				account.AcceptChanges();
                return account;
			}

            return null;
		}

		/// <summary>
		/// Creates Account for user.
		/// </summary>
		/// <returns></returns>
		public Account CreateAccountForUser(MembershipUser user)
		{
			if (user != null)
			{
				Account account = GetAccount(user.ProviderUserKey.ToString());
				if (account == null)
				{
					// Now create an account in the ECF 
					account = new Account();
					account.ProviderKey = user.ProviderUserKey.ToString();
					account.Name = user.UserName;
                    account.State = 2; // active
					account.AcceptChanges();
                    return account;
				}
                return account;
			}

            return null;
		}

        /*
		/// <summary>
		/// Returns account state.
		/// 1 - pending, 2 - active, 3 - suspended, 0 - not set, -1 - not found
		/// </summary>
		/// <returns></returns>
		public int GetAccountState(MembershipUser user)
		{
			int state = -1;
			if (user != null)
			{
				Account account = GetAccount(user.ProviderUserKey.ToString());
				if (account != null)
					state = account.State;
			}

			return state;
		}
         * */

        /// <summary>
        /// Gets the roles for user.
        /// </summary>
        /// <returns></returns>
        public string[] GetRolesForUser()
        {
            if (IsProviderEnabled)
                return Roles.GetRolesForUser();
            else
                return new String[] { AppRoles.EveryoneRole };
        }

        /// <summary>
        /// Gets a value indicating whether this instance is provider enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is provider enabled; otherwise, <c>false</c>.
        /// </value>
        private bool IsProviderEnabled
        {
            get
            {
                return Roles.Enabled;
            }
        }

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetProfileSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = ProfileDataHelper.CreateDataCommand();
			command.CommandText = "GetProfileSchemaVersionNumber";
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
        /// Finds the accounts.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public Account[] FindAccounts(ProfileSearchParameters parameters, ProfileSearchOptions options, out int totalRecords)
        {
            ProfileSearch search = new ProfileSearch(this);
            search.SearchOptions = options;
            search.SearchParameters = parameters;

            MetaStorageCollectionBase<Account> accounts = Account.Search(search, out totalRecords);
            return accounts.ToArray();
        }

        /// <summary>
        /// Finds the accounts.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Account[] FindAccounts(ProfileSearchParameters parameters, ProfileSearchOptions options)
        {
            int totalRecords = 0;
            return FindAccounts(parameters, options, out totalRecords);
        }

        /// <summary>
        /// Finds the organizations.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public Organization[] FindOrganizations(ProfileSearchParameters parameters, ProfileSearchOptions options)
        {
            int totalRecords = 0;
            return FindOrganizations(parameters, options, out totalRecords);
        }

        /// <summary>
        /// Finds the organizations.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="options">The options.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <returns></returns>
        public Organization[] FindOrganizations(ProfileSearchParameters parameters, ProfileSearchOptions options, out int totalRecords)
        {
            ProfileSearch search = new ProfileSearch(this);
            search.SearchOptions = options;
            search.SearchParameters = parameters;

            MetaStorageCollectionBase<Organization> orgs = Organization.Search(search, out totalRecords);
            return orgs.ToArray();
        }

        /// <summary>
        /// Gets the organization. Results are cached.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <returns></returns>
        public Organization GetOrganization(Guid principalId)
        {
            string cacheKey = ProfileCache.CreateCacheKey("org", principalId.ToString());

            Organization organization = null;
            object cachedObject = ProfileCache.Get(cacheKey);

            if (cachedObject != null)
                organization = (Organization)cachedObject;

            // Load the object
            if (organization == null)
            {
                lock (ProfileCache.GetLock(cacheKey))
                {
                    cachedObject = ProfileCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        organization = (Organization)cachedObject;
                    }
                    else
                    {
                        organization = Organization.LoadByPrincipalId(principalId);

                        // Insert to the cache collection
                        ProfileCache.Insert(cacheKey, organization, ProfileConfiguration.Instance.Cache.UserTimeout);
                    }
                }
            }

            return organization;
        }

        /// <summary>
        /// Gets the account. Results are cached.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        /// <returns></returns>
        public Account GetAccount(Guid principalId)
        {
            string cacheKey = ProfileCache.CreateCacheKey("account", principalId.ToString());

            Account account = null;
            object cachedObject = ProfileCache.Get(cacheKey);

            if (cachedObject != null)
                account = (Account)cachedObject;

            // Load the object
            if (account == null)
            {
                lock (ProfileCache.GetLock(cacheKey))
                {
                    cachedObject = ProfileCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        account = (Account)cachedObject;
                    }
                    else
                    {
                        account = Account.LoadByPrincipalId(principalId);

                        // Insert to the cache collection
                        ProfileCache.Insert(cacheKey, account, ProfileConfiguration.Instance.Cache.UserTimeout);
                    }
                }
            }

            return account;
        }

        /// <summary>
        /// Gets the account. Results are cached.
        /// </summary>
        /// <param name="providerKey">The provider key.</param>
        /// <returns></returns>
        public Account GetAccount(string providerKey)
        {
            string cacheKey = ProfileCache.CreateCacheKey("account", providerKey);

            Account account = null;
            object cachedObject = ProfileCache.Get(cacheKey);

            if (cachedObject != null)
                account = (Account)cachedObject;

            // Load the object
            if (account == null)
            {
                lock (ProfileCache.GetLock(cacheKey))
                {
                    cachedObject = ProfileCache.Get(cacheKey);
                    if (cachedObject != null)
                    {
                        account = (Account)cachedObject;
                    }
                    else
                    {
                        account = Account.LoadByProviderKey(providerKey);

                        // Insert to the cache collection
                        ProfileCache.Insert(cacheKey, account, ProfileConfiguration.Instance.Cache.UserTimeout);
                    }
                }
            }

            return account;
        }

        /// <summary>
        /// Deletes the account and related memebership/profile information.
        /// </summary>
        /// <param name="principalId">The principal id.</param>
        public void DeleteAccount(Guid principalId)
        {
            Account acc = GetAccount(principalId);
            if (acc != null)
            {
				MembershipUser user = Membership.GetUser(new Guid(acc.ProviderKey));
				if (user != null)
					Membership.DeleteUser(user.UserName);

                acc.Delete();
                acc.AcceptChanges();
            }
        }

		/// <summary>
		/// Deletes the account and related memebership/profile information.
		/// </summary>
		/// <param name="providerKey">The providerKey.</param>
		public void DeleteAccount(string providerKey)
		{
			Account acc = GetAccount(providerKey);
			if (acc != null)
			{
				acc.Delete();
				acc.AcceptChanges();
			}

			MembershipUser user = Membership.GetUser(new Guid(providerKey));
			if (user != null)
				Membership.DeleteUser(user.UserName);
		}

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentUserName()
        {
            if (HostingEnvironment.IsHosted)
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    return current.User.Identity.Name;
                }
            }
            IPrincipal currentPrincipal = Thread.CurrentPrincipal;
            if ((currentPrincipal != null) && (currentPrincipal.Identity != null))
            {
                return currentPrincipal.Identity.Name;
            }
            return string.Empty;
        }

        #region Permission Methods
        /// <summary>
        /// Checks the permission. Uses caching.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns></returns>
        public bool CheckPermission(string permission)
        {
            return CheckPermission(permission, true);
        }

        /// <summary>
        /// Checks the permission. Results are cached.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <returns></returns>
        public bool CheckPermission(string permission, bool cacheResults)
        {
            if (!ProfileConfiguration.Instance.EnablePermissions)
                return true;

            if (!Roles.Enabled)
                return true;

            if (Roles.GetRolesForUser().Length == 0)
                return false;

            // Always allow admin
            if (Roles.IsUserInRole(AppRoles.AdminRole))
                return true;

            // Now check permissions for all current user roles
            string cacheKey = String.Empty;

            PermissionDto dto = null;

            if (cacheResults)
            {
                cacheKey = String.Format("ecf-pr-{0}", GetCurrentUserName());
                // check cache first
                object cachedObject = CacheHelper.Get(cacheKey);

                if (cachedObject != null)
                    dto = (PermissionDto)cachedObject;
            }

            // Get data from database and cache results if cache is enabled
            if (dto == null)
            {
                string[] roles = GetRolesForUser();
                PermissionAdmin admin = new PermissionAdmin();
                admin.Load(roles);
                dto = admin.CurrentDto;

                if (cacheResults)
                {
                    CacheHelper.Insert(cacheKey, dto, new TimeSpan(0, 0, 30));
                }
            }

            // Now find if the permission we requested is availabe for current user
            // basically if receord for specified permission exists, then it is allowed, otherwise it is denied
            if (dto.RolePermission.Count == 0)
                return false;

            DataRow[] rows = dto.RolePermission.Select(String.Format("permission like '{0}'", permission.Trim()));

            if (rows.Length > 0)
                return true;

            return false;
        }
        #endregion
    }
}
