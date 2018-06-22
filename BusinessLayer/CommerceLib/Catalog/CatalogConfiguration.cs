using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Security;
using System.IO;
using Mediachase.MetaDataPlus.Configurator;
using System.Drawing.Imaging;
using Mediachase.Commerce.Core;
using System.Configuration;
using System.Xml;
using Mediachase.Commerce.Engine.Events;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Catalog
{
    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public sealed class CatalogConfiguration : ConfigurationSection
    {
        //these two variables are the basis of the singleton implementation (uses double-checking)
        private static volatile CatalogConfiguration _instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogConfiguration"/> class.
        /// </summary>
        private CatalogConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static CatalogConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (CatalogConfiguration)ConfigurationManager.GetSection("CommerceFramework/Catalog");
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
        [ConfigurationProperty("Connection", IsRequired=true)]
        public CatalogConnection Connection
        {
            get
            {
                return (CatalogConnection)Instance["Connection"];
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
        /// Configuration element defines Sale Price Types
        /// </summary>
        /// <value>The mapped types.</value>
        [ConfigurationProperty("SalePriceTypes", IsRequired = true)]
        public SalePriceTypeCollection SalePriceTypes
        {
            get
            {
                return (SalePriceTypeCollection)Instance["SalePriceTypes"] as SalePriceTypeCollection;
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

        /// <summary>
        /// Config settings which define the image quality and image encoding
        /// </summary>
        /// <value>The encoding settings.</value>
        [ConfigurationProperty("Encoding", IsRequired = true)]
        public EncodingSettings EncodingSettings
        {
            get
            {
                return (EncodingSettings)Instance["Encoding"];
            }
        }

        /// <summary>
        /// Root config attribute which determines whether to auto configure the Catalog System when first initialized.
        /// </summary>
        /// <value><c>true</c> if [auto configure]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("autoConfigure", IsRequired = true)]
        public bool AutoConfigure
        {
            get
            {
                return (bool)Instance["autoConfigure"];
            }
        }

        /// <summary>
        /// Collection of <see cref="EventDefinition"/> instances defined in the config file
        /// </summary>
        /// <value>The events.</value>
        [ConfigurationProperty("Events", IsRequired = false)]
        public EventCollection Events
        {
            get
            {
                return Instance["Events"] as EventCollection;
            }
        }

        /// <summary>
        /// Retrieve metadata from the meta data engine and store in the MetaInstaller
        /// </summary>
        public static void ConfigureMetaData()
        {
            MemoryStream ms = new MemoryStream(CatalogResources.CatalogSystem);
            StreamReader reader = new StreamReader(ms);
            MetaInstaller.Restore(CatalogContext.MetaDataContext, reader.ReadToEnd());
            reader.Dispose();
        }
    }

    /// <summary>
    /// Config setting which defines the connection string for the catalog system.
    /// </summary>
    public class CatalogConnection : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogConnection"/> class.
        /// </summary>
        public CatalogConnection() { }

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
        /// Gets or sets a value indicating whether to use web services or not. Default is false.
        /// </summary>
        /// <value><c>true</c> if [use web services]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("useWebServices", IsRequired = false, DefaultValue=false)]
        public bool UseWebServices
        {
            get
            {
                return (bool)this["useWebServices"];
            }
            set
            {
                this["useWebServices"] = value;
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
        [ConfigurationProperty("enabled", IsRequired=true, DefaultValue=true)]
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

         //<summary>
         //Configuration attribute which determines when the CatalogCollection values are
         //automatically refreshed in memory (in seconds).
         //</summary>
        /// <summary>
        /// Gets or sets the catalog collection timeout.
        /// </summary>
        /// <value>The catalog collection timeout.</value>
        [ConfigurationProperty("collectionTimeout", IsRequired = true)]
        public TimeSpan CatalogCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["collectionTimeout"];
            }
            set
            {
                this["collectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the CatalogEntry value is
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The catalog entry timeout.</value>
        [ConfigurationProperty("entryTimeout", IsRequired = true)]
        public TimeSpan CatalogEntryTimeout
        {
            get
            {
                return (TimeSpan)this["entryTimeout"];
            }
            set
            {
                this["entryTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the CatalogNode value is
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The catalog node timeout.</value>
        [ConfigurationProperty("nodeTimeout", IsRequired = true)]
        public TimeSpan CatalogNodeTimeout
        {
            get
            {
                return (TimeSpan)this["nodeTimeout"];
            }
            set
            {
                this["nodeTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Configuration attribute which determines when the CatalogSchema value is
        /// automatically refreshed in memory (in seconds).
        /// </summary>
        /// <value>The catalog schema timeout.</value>
        [ConfigurationProperty("schemaTimeout", IsRequired = true)]
        public TimeSpan CatalogSchemaTimeout
        {
            get
            {
                return (TimeSpan)this["schemaTimeout"];
            }
            set
            {
                this["schemaTimeout"] = value.ToString();
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

    /// <summary>
    /// Defines encoding settings - image quality and the default format
    /// </summary>
    public class EncodingSettings : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingSettings"/> class.
        /// </summary>
        public EncodingSettings() { }

        /// <summary>
        /// Gets or sets the image quality percentage.
        /// </summary>
        /// <value>The image quality percentage.</value>
        [ConfigurationProperty("imageQualityPercentage", IsRequired = true)]
        public long ImageQualityPercentage
        {
            get
            {
                return (long)this["imageQualityPercentage"];
            }
            set
            {
                this["imageQualityPercentage"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the default format.
        /// </summary>
        /// <value>The default format.</value>
        [ConfigurationProperty("defaultFormat", IsRequired = true)]
        public ImageFormat DefaultFormat
        {
            get
            {
                return (ImageFormat)this["defaultFormat"];
            }
            set
            {
                this["defaultFormat"] = value.ToString();
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
    /// Collection of SaleTypeDefinition objects
    /// </summary>
    public class SalePriceTypeCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Commerce.Catalog.SalePriceTypeDefinition"/> at the specified index.
        /// </summary>
        /// <value></value>
        public SalePriceTypeDefinition this[int index]
        {
            get
            {
                return base.BaseGet(index) as SalePriceTypeDefinition;
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
            return new SalePriceTypeDefinition();
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
            return ((SalePriceTypeDefinition)element).Key;
        }
    }

    /// <summary>
    /// Definition of sale price element in web.config file
    /// </summary>
    public class SalePriceTypeDefinition : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SalePriceTypeDefinition"/> class.
        /// </summary>
        public SalePriceTypeDefinition() { }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        [ConfigurationProperty("key", IsRequired = true, IsKey=true)]
        public string Key
        {
            get
            {
                return (string)this["key"];
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [ConfigurationProperty("value", IsRequired = true)]
        public int Value
        {
            get
            {
                return (int)this["value"];
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get
            {
                return (string)this["description"];
            }
        }
    }

    /// <summary>
    /// Represents the catalog roles
    /// </summary>
    public static class CatalogRoles
    {
        private static string _CatalogAdminRole = CatalogConfiguration.Instance.GetRole("CatalogAdminRole").Value;
        private static string _CatalogManagerRole = CatalogConfiguration.Instance.GetRole("CatalogManagerRole").Value;
        private static string _CatalogSchemaManagerRole = CatalogConfiguration.Instance.GetRole("CatalogSchemaManagerRole").Value;
        private static string _CatalogViewerRole = CatalogConfiguration.Instance.GetRole("CatalogViewerRole").Value;

        /// <summary>
        /// Represents the string literal for the catalog administrator role
        /// </summary>
        public static string CatalogAdminRole
        {
            get
            {
                return _CatalogAdminRole;
            }
            set
            {
                _CatalogAdminRole = value;
            }
        }

        /// <summary>
        /// Represents the string literal for the catalog manager role
        /// </summary>
        public static string CatalogManagerRole
        {
            get
            {
                return _CatalogManagerRole;
            }
            set
            {
                _CatalogManagerRole = value;
            }
        }

        /// <summary>
        /// Represents the string literal for the catalog schema manager role
        /// </summary>
        public static string CatalogSchemaManagerRole
        {
            get
            {
                return _CatalogSchemaManagerRole;
            }
            set
            {
                _CatalogSchemaManagerRole = value;
            }
        }

        /// <summary>
        /// Represents the string literal for the catalog viewer role
        /// </summary>
        public static string CatalogViewerRole
        {
            get
            {
                return _CatalogViewerRole;
            }
            set
            {
                _CatalogViewerRole = value;
            }
        }
    }
}
