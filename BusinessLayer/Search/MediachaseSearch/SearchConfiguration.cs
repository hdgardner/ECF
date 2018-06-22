using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;

namespace Mediachase.Search
{
    /// <summary>
    /// Implemented as a thread-safe singleton class
    /// </summary>
    public class SearchConfiguration : ConfigurationSection
    {
        private static volatile SearchConfiguration _instance;
        private static object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchConfiguration"/> class.
        /// </summary>
        private SearchConfiguration() { }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <value>The instance.</value>
        public static SearchConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = (SearchConfiguration)ConfigurationManager.GetSection("CommerceFramework/Mediachase.Search");
                        }

                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets the indexers.
        /// </summary>
        /// <value>The indexers.</value>
        [ConfigurationProperty("Indexers", IsRequired = true)]
        public IndexerCollection Indexers
        {
            get
            {
                return Instance["Indexers"] as IndexerCollection;
            }
        }
    }

    /// <summary>
    /// Implements the operations for the work flow collection. (Inherits <see cref="ConfigurationElementCollection"/>.)
    /// </summary>
    public class IndexerCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Gets or sets the <see cref="Mediachase.Commerce.Engine.Search.IndexerDefinition"/> at the specified index.
        /// </summary>
        /// <value></value>
        public IndexerDefinition this[int index]
        {
            get
            {
                return base.BaseGet(index) as IndexerDefinition;
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
            return new IndexerDefinition();
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
            return ((IndexerDefinition)element).Name;
        } 
    }

    /// <summary>
    /// Implements operations for the work flow definition. (Inherits <see cref="ConfigurationElement"/>.)
    /// </summary>
    public class IndexerDefinition : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowDefinition"/> class.
        /// </summary>
        public IndexerDefinition() { }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string ClassName
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }


        /// <summary>
        /// Gets or sets the base path. The path were indexer will store application specific index folders. Each application
        /// will have it's own folder created.
        /// </summary>
        /// <value>The base path.</value>
        [ConfigurationProperty("basePath", IsRequired = true)]
        public string BasePath
        {
            get
            {
                return (string)this["basePath"];
            }
            set
            {
                this["basePath"] = value;
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
            return true;
        }
    }
}
