using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Security.Principal;
using System.Web;
using Mediachase.Commerce.Core;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public class CmsConfiguration : ConfigurationSection
	{
		//these two variables are the basis of the singleton implementation (uses double-checking)
		private static volatile CmsConfiguration _instance;
		private static readonly object _lockObject = new object();

        /*private static bool _Initialized = false;
        private static CacheConfiguration _CacheConfiguration = new CacheConfiguration();*/

        /// <summary>
        /// Initializes a new instance of the <see cref="CmsConfiguration"/> class.
        /// </summary>
		private CmsConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
		public static CmsConfiguration Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lockObject)
					{
						if (_instance == null)
						{
							_instance = (CmsConfiguration)ConfigurationManager.GetSection("CommerceFramework/ContentManagement");
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
        /// Config setting which defines the database connection name
        /// </summary>
        /// <value>The connection.</value>
		[ConfigurationProperty("Connection", IsRequired = true)]
		public CmsConnection Connection
		{
			get
			{
				return (CmsConnection)Instance["Connection"];
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
	/// Config setting which defines the database connection name
	/// </summary>
	public class CmsConnection : ConfigurationElement
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CmsConnection"/> class.
        /// </summary>
		public CmsConnection() { }

		/// <summary>
		/// Retrieves logical name of connection string in <CommerceFramework><Application> hierarchy
        /// </summary>
        /// <value>The name of the connection string.</value>
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
	}

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
        /// Configuration attribute which determines when the PageDocument values are
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The page document timeout.</value>
		[ConfigurationProperty("pageDocumentTimeout", IsRequired = true)]
		public TimeSpan PageDocumentTimeout
		{
			get
			{
				return (TimeSpan)this["pageDocumentTimeout"];
			}
			set
			{
				this["pageDocumentTimeout"] = value.ToString();
			}
		}

        /// <summary>
        /// Configuration attribute which determines when the menu/valigation value is
        /// automatically refreshed in memory.
        /// </summary>
        /// <value>The menu timeout.</value>
		[ConfigurationProperty("menuTimeout", IsRequired = true)]
		public TimeSpan MenuTimeout
		{
			get
			{
				return (TimeSpan)this["menuTimeout"];
			}
			set
			{
				this["menuTimeout"] = value.ToString();
			}
		}

        /// <summary>
        /// Configuration attribute which determines when the workflow value is
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The workflow timeout.</value>
		[ConfigurationProperty("workflowTimeout", IsRequired = true)]
		public TimeSpan WorkflowTimeout
		{
			get
			{
				return (TimeSpan)this["workflowTimeout"];
			}
			set
			{
				this["workflowTimeout"] = value.ToString();
			}
		}

        /// <summary>
        /// Configuration attribute which determines when the template collection value is
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The template timeout.</value>
		[ConfigurationProperty("templateTimeout", IsRequired = true)]
		public TimeSpan TemplateTimeout
		{
			get
			{
				return (TimeSpan)this["templateTimeout"];
			}
			set
			{
				this["templateTimeout"] = value.ToString();
			}
		}

        /// <summary>
        /// Configuration attribute which determines when the SitesCollection values are
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The page document timeout.</value>
        [ConfigurationProperty("sitesCollectionTimeout", IsRequired = true)]
        public TimeSpan SitesCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["sitesCollectionTimeout"];
            }
            set
            {
                this["sitesCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the SiteVariables values are
        /// automatically refreshed in memory.
        /// </summary>
        /// <value>The page document timeout.</value>
        [ConfigurationProperty("siteVariablesTimeout", IsRequired = false, DefaultValue = "0:0:30")]
        public TimeSpan SiteVariablesTimeout
        {
            get
            {
                return (TimeSpan)this["siteVariablesTimeout"];
            }
            set
            {
                this["siteVariablesTimeout"] = value.ToString();
            }
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
    /// Contains the roles for CMS permissions
    /// </summary>
    public static class CmsRoles
    {
        private static string _AdminRole = CmsConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerRole = CmsConfiguration.Instance.GetRole("ManagerRole").Value;
        private static string _EditorRole = CmsConfiguration.Instance.GetRole("EditorRole").Value;
        private static string _ViewerRole = CmsConfiguration.Instance.GetRole("ViewerRole").Value;

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

        public static string EditorRole
        {
            get
            {
                return _EditorRole;
            }
            set
            {
                _EditorRole = value;
            }
        }
        
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
}