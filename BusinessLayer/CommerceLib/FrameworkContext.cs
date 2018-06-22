using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog;
using System.Reflection;
using System.Web;

namespace Mediachase.Commerce
{
    /// <summary>
    /// Global context class for the framework. Provides access to sub systems and some common settings.
    /// </summary>
    public class FrameworkContext
    {
        private static object internalSyncObject = new object();
        private static volatile FrameworkContext _Instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the current framework context.
        /// </summary>
        /// <value>The current.</value>
        public static FrameworkContext Current
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    string key = "Mediachase.Commerce.FrameworkContext";

                    if (context.Items[key] == null)
                        context.Items[key] = new FrameworkContext();

                    return (FrameworkContext)context.Items[key];
                }

                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new FrameworkContext();
                        }
                    }
                }

                return _Instance;
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public ProfileContext Profile
        {
            get
            {
                return ProfileContext.Current;
            }
        }

        /// <summary>
        /// Gets the order system.
        /// </summary>
        /// <value>The order system.</value>
        public OrderContext OrderSystem
        {
            get
            {
                return OrderContext.Current;
            }
        }

        /// <summary>
        /// Gets the catalog system.
        /// </summary>
        /// <value>The catalog system.</value>
        public ICatalogSystem CatalogSystem
        {
            get
            {
                return CatalogContext.Current;
            }
        }

        private DateTime _ContextDate = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the current date time. This property is used to filter out time elements and allows
        /// future or past simulations.
        /// </summary>
        /// <value>The context date time.</value>
        public DateTime CurrentDateTime
        {
            get
            {
                if (_ContextDate == DateTime.MinValue)
                    _ContextDate = DateTime.UtcNow;

                return _ContextDate;
            }
            set
            {
                _ContextDate = value;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The version.</value>
        public static Version Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private static string productVersion;
        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public static string ProductVersion
        {
            get
            {
                lock (internalSyncObject)
                {
                    if (productVersion == null)
                    {
                        Assembly entryAssembly = Assembly.GetExecutingAssembly();
                        if (entryAssembly != null)
                        {
                            object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                            if ((customAttributes != null) && (customAttributes.Length > 0))
                            {
                                productVersion = ((AssemblyInformationalVersionAttribute)customAttributes[0]).InformationalVersion;
                            }
                        }
                        if ((productVersion == null) || (productVersion.Length == 0))
                        {
                            productVersion = Version.ToString();
                            if (productVersion != null)
                            {
                                productVersion = productVersion.Trim();
                            }
                        }
                        if ((productVersion == null) || (productVersion.Length == 0))
                        {
                            productVersion = "1.0.0";
                        }
                    }
                }
                return productVersion;
            }
        }

        private static string productName;
        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public static string ProductName
        {
            get
            {
                lock (internalSyncObject)
                {
                    if (productName == null)
                    {
                        Assembly entryAssembly = Assembly.GetExecutingAssembly();
                        if (entryAssembly != null)
                        {
                            object[] customAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                            if ((customAttributes != null) && (customAttributes.Length > 0))
                            {
                                productName = ((AssemblyProductAttribute)customAttributes[0]).Product;
                            }
                        }
                    }
                }
                return productName;
            }
        }
 


        /// <summary>
        /// Gets the product version description in form of Major.Minor (Build: Build).
        /// </summary>
        /// <value>The product version desc.</value>
        public static string ProductVersionDesc
        {
            get
            {
                string[] version = FrameworkContext.ProductVersion.Split(new char[] { '.' });
                return String.Format("{0}.{1} (build: {2})", version[0], version[1], version[2]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkContext"/> class.
        /// </summary>
        FrameworkContext()
        {
        }
    }
}
