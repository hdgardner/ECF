using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.IO;
using System.Xml;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Core;
using System.Configuration;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Defines way the sensitive data is persisted in the order system.
    /// The examples of sensitive data are credit card numbers and PIN codes.
    /// </summary>
    [Serializable]
    public enum SensitiveDataPersistance
    {
        /// <summary>
        /// The sensitive data will be removed as soon as transaction is completed.
        /// </summary>
        DoNotPersist = 0,
        /// <summary>
        /// Only parts of the data will be persisted. For example last 4 digits of the credit card number. 
        /// Data will be encrypted when payment plans are created.
        /// </summary>
        Partial = 1,
        /// <summary>
        /// Full data will be persisted in encrypted format.
        /// </summary>
        Encrypted = 2
    }

    //Re-done as a singleton and inheriting from ConfigurationSection rather than IConfigSection.
    //Note that instance properties exist in this class (instead of static properties). Instance properties are required to
    //auto-populate the class using configuration file and the ConfigurationSection base class.
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public class OrderConfiguration : ConfigurationSection
    {
        //these two variables are the basis of the singleton implementation (uses double-checking)
        private static volatile OrderConfiguration _instance;
        private static object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConfiguration"/> class.
        /// </summary>
        private OrderConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static OrderConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (OrderConfiguration)ConfigurationManager.GetSection("CommerceFramework/Orders");
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
        /// Gets the new order status.
        /// </summary>
        /// <value>The new order status.</value>
        [ConfigurationProperty("newOrderStatus", IsRequired = true)]
        public string NewOrderStatus
        {
            get
            {
                return (string)Instance["newOrderStatus"];
            }
        }

        /// <summary>
        /// Gets the sensitive data mode.
        /// </summary>
        /// <value>The sensitive data mode.</value>
        [ConfigurationProperty("sensitiveDataMode", IsRequired = false, DefaultValue=SensitiveDataPersistance.Partial)]
        public SensitiveDataPersistance SensitiveDataMode
        {
            get
            {
                return (SensitiveDataPersistance)Instance["sensitiveDataMode"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether to auto configure the Order System when first initialized.
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
        /// <value>The name of the default meta class.</value>
        public class MetaClassNames : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MetaClassNames"/> class.
            /// </summary>
            public MetaClassNames() { }

            /// <summary>
            /// Gets the purchase order class.
            /// </summary>
            /// <value>The purchase order class.</value>
            [ConfigurationProperty("PurchaseOrderClass", IsRequired = true)]
            public PurchaseOrderMetaClass PurchaseOrderClass
            {
                get
                {
                    return (PurchaseOrderMetaClass)this["PurchaseOrderClass"];
                }
            }

            /// <summary>
            /// Gets the payment plan class.
            /// </summary>
            /// <value>The payment plan class.</value>
            [ConfigurationProperty("PaymentPlanClass", IsRequired = true)]
            public PaymentPlanMetaClass PaymentPlanClass
            {
                get
                {
                    return (PaymentPlanMetaClass)this["PaymentPlanClass"];
                }
            }

            /// <summary>
            /// Gets the shopping cart class.
            /// </summary>
            /// <value>The shopping cart class.</value>
            [ConfigurationProperty("ShoppingCartClass", IsRequired = true)]
            public ShoppingCartMetaClass ShoppingCartClass
            {
                get
                {
                    return (ShoppingCartMetaClass)this["ShoppingCartClass"];
                }
            }

            /// <summary>
            /// Gets the order form class.
            /// </summary>
            /// <value>The order form class.</value>
            [ConfigurationProperty("OrderFormClass", IsRequired = true)]
            public OrderFormMetaClass OrderFormClass
            {
                get
                {
                    return (OrderFormMetaClass)this["OrderFormClass"];
                }
            }

            /// <summary>
            /// Gets the line item class.
            /// </summary>
            /// <value>The line item class.</value>
            [ConfigurationProperty("LineItemClass", IsRequired = true)]
            public LineItemMetaClass LineItemClass
            {
                get
                {
                    return (LineItemMetaClass)this["LineItemClass"];
                }
            }

            /// <summary>
            /// Gets the shipment class.
            /// </summary>
            /// <value>The shipment class.</value>
            [ConfigurationProperty("ShipmentClass", IsRequired = true)]
            public ShipmentMetaClass ShipmentClass
            {
                get
                {
                    return (ShipmentMetaClass)this["ShipmentClass"];
                }
            }

            /// <summary>
            /// Gets the order address class.
            /// </summary>
            /// <value>The order address class.</value>
            [ConfigurationProperty("OrderAddressClass", IsRequired = true)]
            public OrderAddressMetaClass OrderAddressClass
            {
                get
                {
                    return (OrderAddressMetaClass)this["OrderAddressClass"];
                }
            }
        }

        /// <summary>
        /// Purchase order meta class configuration.
        /// </summary>
        public class PurchaseOrderMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PurchaseOrderMetaClass"/> class.
            /// </summary>
            public PurchaseOrderMetaClass() { }

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
        /// Payment plan meta class configuration element.
        /// </summary>
        public class PaymentPlanMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PaymentPlanMetaClass"/> class.
            /// </summary>
            public PaymentPlanMetaClass() { }

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
        /// Shopping cart meta class configuration element.
        /// </summary>
        public class ShoppingCartMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShoppingCartMetaClass"/> class.
            /// </summary>
            public ShoppingCartMetaClass() { }

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
        /// Order Form meta class configuration element.
        /// </summary>
        public class OrderFormMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OrderFormMetaClass"/> class.
            /// </summary>
            public OrderFormMetaClass() { }

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
        /// Line Item meta class configuration element.
        /// </summary>
        public class LineItemMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LineItemMetaClass"/> class.
            /// </summary>
            public LineItemMetaClass() { }

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
        /// Shipment meta class configuration element.
        /// </summary>
        public class ShipmentMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShipmentMetaClass"/> class.
            /// </summary>
            public ShipmentMetaClass() { }

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
        /// Order Address meta class configuration element.
        /// </summary>
        public class OrderAddressMetaClass : ConfigurationElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OrderAddressMetaClass"/> class.
            /// </summary>
            public OrderAddressMetaClass() { }

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
        /// OrderConnection defines the database connection name
        /// </summary>
        [ConfigurationProperty("Connections", IsRequired = true)]
        public OrderConnection Connections
        {
            get
            {
                return (OrderConnection)Instance["Connections"];
            }
        }

        /// <summary>
        /// Configures the meta data.
        /// </summary>
        public static void ConfigureMetaData()
        {
            MemoryStream ms = new MemoryStream(OrderResources.OrderSystem);
            StreamReader reader = new StreamReader(ms);

            MetaInstaller.Restore(OrderContext.MetaDataContext, reader.ReadToEnd());
        }

        /// <summary>
        /// Config settings which define where caching is enabled and timeouts related to it.
        /// </summary>
        [ConfigurationProperty("Cache", IsRequired = true)]
        public CacheConfiguration Cache
        {
            get
            {
                return (CacheConfiguration)Instance["Cache"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement"></see> object is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Configuration.ConfigurationElement"></see> object is read-only; otherwise, false.
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
    /// Order Connection Element
    /// </summary>
    public class OrderConnection : ConfigurationElement
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConnection"/> class.
        /// </summary>
        public OrderConnection() { }

        /// <summary>
        /// Gets or sets the name of the configuration connection string.
        /// </summary>
        /// <value>The name of the configuration connection string.</value>
        [ConfigurationProperty("confConnectionStringName", IsRequired = true)]
        public string ConfigurationConnectionStringName
        {
            get
            {
                return (string)this["confConnectionStringName"];
            }
            set
            {
                this["confConnectionStringName"] = value;
            }
        }

        /// <summary>
        /// Uses ConnectionStringName property to retrieve connection string from
        /// ConnectionStrings portion of app/web.config file
        /// </summary>
        /// <value>The configuration app database.</value>
        public string ConfigurationAppDatabase
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[(string)this["confConnectionStringName"]].ConnectionString;
            }
        }

        /// <summary>
        /// Gets or sets the name of the transaction connection string.
        /// </summary>
        /// <value>The name of the transaction connection string.</value>
        [ConfigurationProperty("transConnectionStringName", IsRequired = true)]
        public string TransactionConnectionStringName
        {
            get
            {
                return (string)this["transConnectionStringName"];
            }
            set
            {
                this["transConnectionStringName"] = value;
            }
        }

        /// <summary>
        /// Uses ConnectionStringName property to retrieve connection string from
        /// ConnectionStrings portion of app/web.config file
        /// </summary>
        /// <value>The transaction app database.</value>
        public string TransactionAppDatabase
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[(string)this["transConnectionStringName"]].ConnectionString;
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

    #region Mapped Types
    /// <summary>
    /// Represents the mapped order types. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class MappedTypes : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappedTypes"/> class.
        /// </summary>
        public MappedTypes() { }

        #region Types
        /// <summary>
        /// Gets the type of the shopping cart.
        /// </summary>
        /// <value>The type of the shopping cart.</value>
        [ConfigurationProperty("ShoppingCartType", IsRequired = true)]
        public ShoppingCartType ShoppingCartType
        {
            get
            {
                return (ShoppingCartType)this["ShoppingCartType"];
            }
        }

        /// <summary>
        /// Gets the type of the purchase order.
        /// </summary>
        /// <value>The type of the purchase order.</value>
        [ConfigurationProperty("PurchaseOrderType", IsRequired = true)]
        public PurchaseOrderType PurchaseOrderType
        {
            get
            {
                return (PurchaseOrderType)this["PurchaseOrderType"];
            }
        }

        /// <summary>
        /// Gets the type of the order form.
        /// </summary>
        /// <value>The type of the order form.</value>
        [ConfigurationProperty("OrderFormType", IsRequired = true)]
        public OrderFormType OrderFormType
        {
            get
            {
                return (OrderFormType)this["OrderFormType"];
            }
        }

        /// <summary>
        /// Gets the type of the payment plan.
        /// </summary>
        /// <value>The type of the payment plan.</value>
        [ConfigurationProperty("PaymentPlanType", IsRequired = true)]
        public PaymentPlanType PaymentPlanType
        {
            get
            {
                return (PaymentPlanType)this["PaymentPlanType"];
            }
        }

        /// <summary>
        /// Gets the type of the line item.
        /// </summary>
        /// <value>The type of the line item.</value>
        [ConfigurationProperty("LineItemType", IsRequired = true)]
        public LineItemType LineItemType
        {
            get
            {
                return (LineItemType)this["LineItemType"];
            }
        }

        /// <summary>
        /// Gets the type of the order group address.
        /// </summary>
        /// <value>The type of the order group address.</value>
        [ConfigurationProperty("OrderGroupAddressType", IsRequired = true)]
        public OrderGroupAddressType OrderGroupAddressType
        {
            get
            {
                return (OrderGroupAddressType)this["OrderGroupAddressType"];
            }
        }

        /// <summary>
        /// Gets the type of the shipment.
        /// </summary>
        /// <value>The type of the shipment.</value>
        [ConfigurationProperty("ShipmentType", IsRequired = true)]
        public ShipmentType ShipmentType
        {
            get
            {
                return (ShipmentType)this["ShipmentType"];
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents the shopping cart type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class ShoppingCartType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartType"/> class.
        /// </summary>
        public ShoppingCartType() { }

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
    /// Represents the purchase order type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class PurchaseOrderType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderType"/> class.
        /// </summary>
        public PurchaseOrderType() { }

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
    /// Represents the order form type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class OrderFormType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderFormType"/> class.
        /// </summary>
        public OrderFormType() { }

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
    /// Represents the payment plan type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class PaymentPlanType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentPlanType"/> class.
        /// </summary>
        public PaymentPlanType() { }

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
    /// Represents teh line item type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class LineItemType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemType"/> class.
        /// </summary>
        public LineItemType() { }

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
    /// Represents teh order group address type. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class OrderGroupAddressType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderGroupAddressType"/> class.
        /// </summary>
        public OrderGroupAddressType() { }

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
    /// Shipment Type configuration element.
    /// </summary>
    public class ShipmentType : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentType"/> class.
        /// </summary>
        public ShipmentType() { }

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
    /// Order Roles
    /// </summary>
    public static class OrderRoles
    {
        private static string _AdminRole = OrderConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerRole = OrderConfiguration.Instance.GetRole("ManagerRole").Value;
        private static string _SchemaManagerRole = OrderConfiguration.Instance.GetRole("SchemaManagerRole").Value;
        private static string _ViewerRole = OrderConfiguration.Instance.GetRole("ViewerRole").Value;

        /// <summary>
        /// Public string literal for the order administrators role.
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
        /// Public string literal for the order managers role.
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
        /// Public string literal for the order schema mangers role.
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
        /// Public string literal for the order viewers role.
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
        /// Gets or sets the shipping collection timeout.
        /// </summary>
        /// <value>The shipping collection timeout.</value>
        [ConfigurationProperty("shippingCollectionTimeout", IsRequired = true)]
        public TimeSpan ShippingCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["shippingCollectionTimeout"];
            }
            set
            {
                this["shippingCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the payment collection timeout.
        /// </summary>
        /// <value>The payment collection timeout.</value>
        [ConfigurationProperty("paymentCollectionTimeout", IsRequired = true)]
        public TimeSpan PaymentCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["paymentCollectionTimeout"];
            }
            set
            {
                this["paymentCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the status collection timeout.
        /// </summary>
        /// <value>The status collection timeout.</value>
        [ConfigurationProperty("statusCollectionTimeout", IsRequired = true)]
        public TimeSpan StatusCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["statusCollectionTimeout"];
            }
            set
            {
                this["statusCollectionTimeout"] = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the country collection timeout.
        /// </summary>
        /// <value>The country collection timeout.</value>
        [ConfigurationProperty("countryCollectionTimeout", IsRequired = true)]
        public TimeSpan CountryCollectionTimeout
        {
            get
            {
                return (TimeSpan)this["countryCollectionTimeout"];
            }
            set
            {
                this["countryCollectionTimeout"] = value.ToString();
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
