using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using System.Xml;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Core;
using System.Configuration;

namespace Mediachase.Commerce.Marketing
{
    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public sealed class MarketingConfiguration : ConfigurationSection
    {
        //these two variables are the basis of the singleton implementation (uses double-checking)
        private static volatile MarketingConfiguration _instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingConfiguration"/> class.
        /// </summary>
        private MarketingConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static MarketingConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (MarketingConfiguration)ConfigurationManager.GetSection("CommerceFramework/Marketing");
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
        /// Gets the reservation timeout in minutes for promotions.
        /// </summary>
        /// <value>The reservation timeout.</value>
        [ConfigurationProperty("reservationTimeout", IsRequired = false, DefaultValue=30)]
        public int ReservationTimeout
        {
            get
            {
                return (int)Instance["reservationTimeout"];
            }
        }

        /// <summary>
        /// Configuration element defines the database connection string names.
        /// </summary>
        /// <value>The connection.</value>
        [ConfigurationProperty("Connection", IsRequired = true)]
        public MarketingConnection Connection
        {
            get
            {
                return (MarketingConnection)Instance["Connection"];
            }
        }

        /// <summary>
        /// Configuration element defines where caching is enabled and timeouts related to it.
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

        /// <summary>
        /// Configuration element defines the mapping of mapped types to type names specified
        /// in configuration file
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
    /// Configuration element defines name of the connection string used for getting marketing application 
    /// properties.
    /// </summary>
    public class MarketingConnection : ConfigurationElement
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
    /// Configuration element which contains cache enabled and reset values.
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
                this["enabled"] = value.ToString();
            }
        }


        /// <summary>
        /// Gets or sets the promotion collection timeout.
        /// </summary>
        /// <value>The promotion collection timeout.</value>
        [ConfigurationProperty("promotionTimeout", IsRequired = true)]
        public TimeSpan PromotionCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["promotionTimeout"];
            }
            set
            {
                this["promotionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the campaign collection timeout.
        /// </summary>
        /// <value>The campaign collection timeout.</value>
        [ConfigurationProperty("campaignTimeout", IsRequired = true)]
        public TimeSpan CampaignCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["campaignTimeout"];
            }
            set
            {
                this["campaignTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the policy collection timeout.
        /// </summary>
        /// <value>The policy collection timeout.</value>
        [ConfigurationProperty("policyTimeout", IsRequired = true)]
        public TimeSpan PolicyCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["policyTimeout"];
            }
            set
            {
                this["policyTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the segment collection timeout.
        /// </summary>
        /// <value>The segment collection timeout.</value>
        [ConfigurationProperty("segmentTimeout", IsRequired = true)]
        public TimeSpan SegmentCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["segmentTimeout"];
            }
            set
            {
                this["segmentTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the expression collection timeout.
        /// </summary>
        /// <value>The expression collection timeout.</value>
        [ConfigurationProperty("expressionTimeout", IsRequired = true)]
        public TimeSpan ExpressionCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["expressionTimeout"];
            }
            set
            {
                this["expressionTimeout"] = value.ToString();
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
    /// Represents the marketing roles.
    /// </summary>
    public static class MarketingRoles
    {
        private static string _AdminRole = MarketingConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerRole = MarketingConfiguration.Instance.GetRole("ManagerRole").Value;
        private static string _SchemaManagerRole = MarketingConfiguration.Instance.GetRole("SchemaManagerRole").Value;
        private static string _ViewerRole = MarketingConfiguration.Instance.GetRole("ViewerRole").Value;

        /// <summary>
        /// Public string literal for the marketing administrators role.
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
        /// Public string literal for the marketing managers role.
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
        /// Public string literal for the marketing schema managers role.
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
        /// Public string literal for the marketing viewers role.
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

    /// <summary>
    /// Represents the mapped marketing types. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class MappedTypes : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappedTypes"/> class.
        /// </summary>
        public MappedTypes() { }

        #region Types
        /// <summary>
        /// Gets the type of the expression validator.
        /// </summary>
        /// <value>The type of the expression validator.</value>
        [ConfigurationProperty("ExpressionValidatorType", IsRequired = true)]
        public ExpressionValidatorType ExpressionValidatorType
        {
            get
            {
                return (ExpressionValidatorType)this["ExpressionValidatorType"];
            }
        }

        /// <summary>
        /// Gets the type of the promotion entry populate function.
        /// </summary>
        /// <value>The type of the promotion entry populate function.</value>
        [ConfigurationProperty("PromotionEntryPopulateFunctionType", IsRequired = false)]
        public PromotionEntryPopulateFunctionType PromotionEntryPopulateFunctionType
        {
            get
            {
                object val = this["PromotionEntryPopulateFunctionType"];
                if (val == null)
                    return new PromotionEntryPopulateFunctionType();

                return (PromotionEntryPopulateFunctionType)val;
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents the expression validator types. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class ExpressionValidatorType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionValidatorType"/> class.
        /// </summary>
        public ExpressionValidatorType() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "Mediachase.Commerce.Marketing.Validators.PromotionEntryPopulate,Mediachase.Commerce.Marketing.Validators")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }
    }


    /// <summary>
    /// Represents the promotion entry populate function type that is used to populate promotion entry object. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class PromotionEntryPopulateFunctionType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionEntryPopulateFunctionType"/> class.
        /// </summary>
        public PromotionEntryPopulateFunctionType() { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "Mediachase.Commerce.Marketing.Validators.PromotionEntryPopulate,Mediachase.Commerce.Marketing.Validators")]
        public string Name
        {
            get
            {
                object name = this["name"];

                if (name == null)
                    return "Mediachase.Commerce.Marketing.Validators.PromotionEntryPopulate,Mediachase.Commerce.Marketing.Validators";

                return (string)name;
            }
        }
    }
}