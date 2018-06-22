using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using System.Xml;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Core;
using System.Configuration;

namespace Mediachase.Commerce.Profile
{
    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public class ProfileConfiguration : ConfigurationSection
    {
        private static volatile ProfileConfiguration _instance;
        private static object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileConfiguration"/> class.
        /// </summary>
        private ProfileConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static ProfileConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (ProfileConfiguration)ConfigurationManager.GetSection("CommerceFramework/Profile");
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Unique identifier for application.
        /// </summary>
        /// <value>The application id.</value>
        public Guid ApplicationId
        {
            get
            {
                return AppContext.Current.ApplicationId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to auto configure the Profile System when first initialized.
        /// </summary>
        /// <value><c>true</c> if [auto configure]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("autoConfigure", IsRequired = true, DefaultValue=true)]
        public bool AutoConfigure
        {
            get
            {
                return (bool)Instance["autoConfigure"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether [enable permissions].
        /// </summary>
        /// <value><c>true</c> if [enable permissions]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("enablePermissions", IsRequired = false, DefaultValue = false)]
        public bool EnablePermissions
        {
            get
            {
                return (bool)Instance["enablePermissions"];
            }
        }

        /// <summary>
        /// Contains the names of meta classes for orders that are stored in the configuration file
        /// </summary>
        /// <value>The meta classes.</value>
        [ConfigurationProperty("MetaClasses", IsRequired = true)]
        public MetaClassNames MetaClasses
        {
            get
            {
                return (MetaClassNames)Instance["MetaClasses"];
            }
        }

        #region Meta Class Names

        /// <summary>
        /// Gets the name of the default meta class.
        /// </summary>
        /// <value>The name of the default meta classes</value>
        public class MetaClassNames : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MetaClassNames"/> class.
            /// </summary>
            public MetaClassNames() { }

            /// <summary>
            /// Gets the customer address class.
            /// </summary>
            /// <value>The customer address class.</value>
            [ConfigurationProperty("CustomerAddressClass", IsRequired = true)]
            public CustomerAddressMetaClass CustomerAddressClass
            {
                get
                {
                    return (CustomerAddressMetaClass)this["CustomerAddressClass"];
                }
            }

            /// <summary>
            /// Gets the organization class.
            /// </summary>
            /// <value>The organization class.</value>
            [ConfigurationProperty("OrganizationClass", IsRequired = true)]
            public OrganizationMetaClass OrganizationClass
            {
                get
                {
                    return (OrganizationMetaClass)this["OrganizationClass"];
                }
            }

            /// <summary>
            /// Gets the account class.
            /// </summary>
            /// <value>The account class.</value>
            [ConfigurationProperty("AccountClass", IsRequired = true)]
            public AccountMetaClass AccountClass
            {
                get
                {
                    return (AccountMetaClass)this["AccountClass"];
                }
            }
        }

        /// <summary>
        /// Implements operations for and represents the customer address meta class. (Inherits <see cref="ConfigurationElement"/>.)
        /// </summary>
        public class CustomerAddressMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CustomerAddressMetaClass"/> class.
            /// </summary>
            public CustomerAddressMetaClass() { }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            [ConfigurationProperty("name", IsRequired = true)]
            public string Name
            {
                get
                {
                    return (string)this["name"];
                }
            }
        }

        /// <summary>
        /// Implements operations for and represents the organization meta class. (Inherits <see cref="ConfigurationElement"/>.)
        /// </summary>
        public class OrganizationMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OrganizationMetaClass"/> class.
            /// </summary>
            public OrganizationMetaClass() { }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            [ConfigurationProperty("name", IsRequired = true)]
            public string Name
            {
                get
                {
                    return (string)this["name"];
                }
            }
        }

        /// <summary>
        /// Implements operations for and represents the account meta class. (Inherits <see cref="ConfigurationElement"/>.)
        /// </summary>
        public class AccountMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AccountMetaClass"/> class.
            /// </summary>
            public AccountMetaClass() { }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            [ConfigurationProperty("name", IsRequired = true)]
            public string Name
            {
                get
                {
                    return (string)this["name"];
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the mapped types.
        /// </summary>
        /// <value>The mapped types.</value>
        [ConfigurationProperty("MappedTypes", IsRequired = true)]
        public MappedTypes MappedTypes
        {
            get
            {
                return (MappedTypes)Instance["MappedTypes"];
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("Connection", IsRequired = true)]
        public ProfileConnection Connection
        {
            get
            {
                return (ProfileConnection)Instance["Connection"];
            }
        }

        /// <summary>
        /// Config settings which define where caching is enabled and timeouts related to it.
        /// </summary>
        /// <value>The cache.</value>
        [ConfigurationProperty("Cache", IsRequired = true)]
        public CacheConfiguration Cache
        {
            get
            {
                return (CacheConfiguration)Instance["Cache"];
            }
        }

        /// <summary>
        /// Configures the meta data.
        /// </summary>
        public static void ConfigureMetaData()
        {
            MemoryStream ms = new MemoryStream(ProfileResources.ProfileSystem);
            StreamReader reader = new StreamReader(ms);

            MetaInstaller.Restore(ProfileContext.MetaDataContext, reader.ReadToEnd());
        }

        /// <summary>
        /// Configuration element defines the mapping of mapped types to type names specified
        /// in configuration file
        /// </summary>
        /// <value>The mapped types.</value>
        [ConfigurationProperty("Roles", IsRequired = true)]
        public RoleCollection Roles
        {
            get
            {
                return (RoleCollection)Instance["Roles"] as RoleCollection;
            }
        }

        /// <summary>
        /// Returns class names mapped to the event key (name)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public RoleDefinition GetRole(string key)
        {
            if (Instance.Roles != null)
            {
                for (int i = 0; i < Roles.Count; i++)
                {
                    if (Roles[i].Name == key)
                    {
                        return Roles[i];
                    }
                }

                //Role not found, return null
                return null;
            }
            //If no Role have been defined, return null
            return null;
        }
    }

    /// <summary>
    /// Implements operations for and represents the profile connection. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class ProfileConnection : ConfigurationElement
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileConnection"/> class.
        /// </summary>
        public ProfileConnection() { }

        /// <summary>
        /// Retrieves logical name of connection string in CommerceFramework Application hierarchy
        /// </summary>
        /// <value>The connection string name.</value>
        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get
            {
                return (string)this["connectionStringName"];
            }
            set
            {
                this["connectionStringName"] = value;
            }
        }

        /// <summary>
        /// Uses ConnectionStringName property to retrieve connection string from
        /// ConnectionStrings portion of app/web.config file
        /// </summary>
        /// <value>The app database.</value>
        public string AppDatabase
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[(string)this["connectionStringName"]].ConnectionString;
            }
        }

        //make the configuration section editable by overriding this ConfigurationElement property
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly()
        {
            return false;
        }
    }

    /// <summary>
    /// Collection of RoleDefinition objects
    /// </summary>
    public class RoleCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Commerce.WorkflowDefinition"/> at the specified index.
        /// </summary>
        /// <value></value>
        public RoleDefinition this[int index]
        {
            get
            {
                return base.BaseGet(index) as RoleDefinition;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new RoleDefinition();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RoleDefinition)element).Name;
        }
    }

    /// <summary>
    /// Definition of single role element in web.config file
    /// </summary>
    public class RoleDefinition : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleDefinition"/> class.
        /// </summary>
        public RoleDefinition() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
        }
    }

    /// <summary>
    /// Represents the profile roles.
    /// </summary>
	public static class ProfileRoles
	{
        private static string _AdminRole = ProfileConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerRole = ProfileConfiguration.Instance.GetRole("ManagerRole").Value;
        private static string _SchemaManagerRole = ProfileConfiguration.Instance.GetRole("SchemaManagerRole").Value;
        private static string _ViewerRole = ProfileConfiguration.Instance.GetRole("ViewerRole").Value;

        /// <summary>
        /// Public string literal for the profile administrators role.
        /// </summary>
		public static string AdminRole
        {
            get
            {
                return _AdminRole;
            }
            set
            {
                _AdminRole = value;
            }
        }

        /// <summary>
        /// Public string literal for the profile managers role.
        /// </summary>
        public static string ManagerRole
        {
            get
            {
                return _ManagerRole;
            }
            set
            {
                _ManagerRole = value;
            }
        }

        /// <summary>
        /// Public string literal for the profile schema managers role.
        /// </summary>
        public static string SchemaManagerRole
        {
            get
            {
                return _SchemaManagerRole;
            }
            set
            {
                _SchemaManagerRole = value;
            }
        }

        /// <summary>
        /// Public string literal for the profile viewers role.
        /// </summary>
        public static string ViewerRole
        {
            get
            {
                return _ViewerRole;
            }
            set
            {
                _ViewerRole = value;
            }
        }
    }

    #region Mapped Types
    /// <summary>
    /// Implements operations for and represents the mapped types. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class MappedTypes : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappedTypes"/> class.
        /// </summary>
        public MappedTypes() { }

        /// <summary>
        /// Gets the customer address.
        /// </summary>
        /// <value>The customer address.</value>
        [ConfigurationProperty("CustomerAddress", IsRequired = true)]
        public CustomerAddressType CustomerAddress
        {
            get
            {
                return (CustomerAddressType)this["CustomerAddress"];
            }
        }

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <value>The organization.</value>
        [ConfigurationProperty("Organization", IsRequired = true)]
        public OrganizationType Organization
        {
            get
            {
                return (OrganizationType)this["Organization"];
            }
        }

        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <value>The account.</value>
        [ConfigurationProperty("Account", IsRequired = true)]
        public AccountType Account
        {
            get
            {
                return (AccountType)this["Account"];
            }
        }
    }
    #region Types
    /// <summary>
    /// Implements operations for and represents the customer address type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class CustomerAddressType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressType"/> class.
        /// </summary>
        public CustomerAddressType() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }
    }

    /// <summary>
    /// Implements operations for and represents the organization type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class OrganizationType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationType"/> class.
        /// </summary>
        public OrganizationType() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }
    }

    /// <summary>
    /// Implements operations for and represents the account type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class AccountType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountType"/> class.
        /// </summary>
        public AccountType() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// Config settings which define where caching is enabled and timeouts related to it.
    /// </summary>
    public class CacheConfiguration : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        public CacheConfiguration() { }

        /// <summary>
        /// Attribute determines whether in-memory caching is enabled or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("enabled", IsRequired = true, DefaultValue = true)]
        public bool IsEnabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the user collection timeout.
        /// </summary>
        /// <value>The user collection timeout.</value>
        [ConfigurationProperty("userCollectionTimeout", IsRequired = true)]
        public TimeSpan UserCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["userCollectionTimeout"];
            }
            set
            {
                this["userCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the User value is
        /// automatically refreshed in memory.
        /// </summary>
        /// <value>The catalog entry timeout.</value>
        [ConfigurationProperty("userTimeout", IsRequired = true)]
        public TimeSpan UserTimeout
        {
            get
            {
                return (TimeSpan)this["userTimeout"];
            }
            set
            {
                this["userTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the org collection timeout.
        /// </summary>
        /// <value>The org collection timeout.</value>
        [ConfigurationProperty("organizationCollectionTimeout", IsRequired = true)]
        public TimeSpan OrganizationCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["organizationCollectionTimeout"];
            }
            set
            {
                this["organizationCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the Organization value is
        /// automatically refreshed in memory.
        /// </summary>
        /// <value>The catalog entry timeout.</value>
        [ConfigurationProperty("organizationTimeout", IsRequired = true)]
        public TimeSpan OrganizationTimeout
        {
            get
            {
                return (TimeSpan)this["organizationTimeout"];
            }
            set
            {
                this["organizationTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"/> object is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}