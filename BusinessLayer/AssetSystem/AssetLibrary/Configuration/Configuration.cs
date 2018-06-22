using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Security.Principal;
using System.Web;
using Mediachase.Commerce.Core;

namespace Mediachase.Ibn.Library
{
	public class AssetConfiguration : ConfigurationSection
	{
		//these two variables are the basis of the singleton implementation (uses double-checking)
		private static volatile AssetConfiguration _instance;
		private static readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetConfiguration"/> class.
        /// </summary>
		private AssetConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
		public static AssetConfiguration Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lockObject)
					{
						if (_instance == null)
						{
							_instance = (AssetConfiguration)ConfigurationManager.GetSection("CommerceFramework/AssetManagement");
						}
					}
				}

				return _instance;
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
    /// Contains the roles for Asset permissions
    /// </summary>
    public static class AssetRoles
    {
        private static string _AdminRole = AssetConfiguration.Instance.GetRole("AdminRole").Value;
        private static string _ManagerRole = AssetConfiguration.Instance.GetRole("ManagerRole").Value;
        private static string _SchemaManagerRole = AssetConfiguration.Instance.GetRole("SchemaManagerRole").Value;
        private static string _ViewerRole = AssetConfiguration.Instance.GetRole("ViewerRole").Value;

        /// <summary>
        /// Public string literal for the asset administrators role.
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
        /// Public string literal for the asset managers role.
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
        /// Public string literal for the asset schema managers role.
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
        /// Public string literal for the asset viewers role.
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
}