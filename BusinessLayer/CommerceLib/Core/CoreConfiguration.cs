using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Security;
using System.IO;
using Mediachase.MetaDataPlus.Configurator;
using System.Drawing.Imaging;
using System.Configuration;
using System.Xml;

namespace Mediachase.Commerce.Core
{
    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public sealed class CoreConfiguration : ConfigurationSection
    {
        private static volatile CoreConfiguration _instance;
        private static object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreConfiguration"/> class.
        /// </summary>
        private CoreConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static CoreConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (CoreConfiguration)ConfigurationManager.GetSection("CommerceFramework/Application");
                        }
                    }
                }                
                return _instance;
            }
        }

        /*
        /// <summary>
        /// Root configuration attribute used to store unique identifier associated with application
        /// </summary>
        [ConfigurationProperty("applicationId", IsRequired = true)]
        public Guid ApplicationId
        {
            get
            {
                return (Guid)Instance["applicationId"]; 
            }
        }
         * */

        /// <summary>
        /// Root configuration attribute which defines default value used for application name
        /// </summary>
        /// <value>The default name of the application.</value>
        [ConfigurationProperty("defaultApplicationName", IsRequired = true)]
        public string DefaultApplicationName
        {
            get
            {
                return (string)Instance["defaultApplicationName"];
            }
            set
            {
                Instance["defaultApplicationName"] = value;
            }

        }

        /// <summary>
        /// Configuration element which contains database connection string name
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("Connection", IsRequired = true)]
        public CoreConnection Connection
        {
            get
            {
                return (CoreConnection)Instance["Connection"];
            }
        }

        /// <summary>
        /// Configuration element which contains cache enabled and reset values.
        /// </summary>
        /// <value>The cache config.</value>
        [ConfigurationProperty("Cache", IsRequired = true)]
        public CacheConfiguration CacheConfig
        {
            get
            {
                return (CacheConfiguration)Instance["Cache"];
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
    /// Configuration element containing the connection string name for the application.
    /// </summary>
    public class CoreConnection : ConfigurationElement
    {
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
                return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            }
        }
    }

    /// <summary>
    /// Configuration element used to define cache properties.
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
        [ConfigurationProperty("enabled", IsRequired = true)]
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
        /// Gets or sets the app collection timeout.
        /// </summary>
        /// <value>The app collection timeout.</value>
        [ConfigurationProperty("appTimeout", IsRequired = true)]
        public TimeSpan AppCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["appTimeout"];
            }
            set
            {
                this["appTimeout"] = value;
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
    /// Represents the application roles.
    /// </summary>
    public static class AppRoles
    {
        private static string _AdminRole = CoreConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerUserRole = CoreConfiguration.Instance.GetRole("ManagerUserRole").Value;
        private static string _EveryoneRole = CoreConfiguration.Instance.GetRole("EveryoneRole").Value;
        private static string _RegisteredRole = CoreConfiguration.Instance.GetRole("RegisteredRole").Value;

        /// <summary>
        /// Public string literal for the admininstrators role.
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
        /// Public string literal for the management users role.
        /// </summary>
        public static string ManagerUserRole
        {
            get
            {
                return _ManagerUserRole;
            }
            set
            {
                _ManagerUserRole = value;
            }
        }

        /// <summary>
        /// Public string literal for the role for everyone.
        /// </summary>
        public static string EveryoneRole
        {
            get
            {
                return _EveryoneRole;
            }
            set
            {
                _EveryoneRole = value;
            }
        }

        /// <summary>
        /// Public string literal for the registered users role.
        /// </summary>
        public static string RegisteredRole
        {
            get
            {
                return _RegisteredRole;
            }
            set
            {
                _RegisteredRole = value;
            }
        }
    }
}
